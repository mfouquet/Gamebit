using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using SDWebImage;

namespace Gamebit
{
	public partial class EventsViewController : UITableViewController
	{
		#region Class-level Variables
		Dictionary<string, List<Game>> parsedEventsJson;
		NSObject enteredForegroundObserver;
		#endregion

		#region EventsViewController Constructors - EMPTY
		public EventsViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			enteredForegroundObserver = NSNotificationCenter.DefaultCenter.AddObserver (AppDelegate.enteredForeground, 
			                                                                            (notification) => { GetJson (); });
			GetJson ();
		}
		#endregion

		#region ViewDidAppear - EMPTY
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}
		#endregion

		#region ViewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			EventsNavSearchBtn.TouchUpInside += delegate {
				EventsNavCloseBtn.Transform = Utilities.Hide;
				EventsNavSearchBar.Transform = Utilities.Hide;
				EventsNavCloseBtn.Hidden = false;
				EventsNavSearchBar.Hidden = false;
				EventsNavSearchBar.BecomeFirstResponder ();
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					EventsNavSearchBtn.Transform = Utilities.Hide;
					EventsTitleLbl.Transform = Utilities.Hide;

					TableView.Alpha = 0;

					EventsNavCloseBtn.Transform = Utilities.Show;
					EventsNavSearchBar.Transform = Utilities.Show;

				}, null);
			};

			EventsNavSearchBar.SearchButtonClicked += delegate {
				EventsNavSearchBar.ResignFirstResponder ();
				EventsNavSearchLoadingSpinner.Transform = Utilities.Hide;
				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					EventsNavSearchLoadingSpinner.StartAnimating ();
					EventsNavCloseBtn.Transform = Utilities.Hide;
					EventsNavSearchLoadingSpinner.Transform = Utilities.Show;
				}, null);

				DisplaySearchResults ();
			};
			
			EventsNavCloseBtn.TouchUpInside += delegate {
				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					EventsNavSearchBtn.Transform = Utilities.Show;
					EventsTitleLbl.Transform = Utilities.Show;
					TableView.Alpha = 1;

					EventsNavCloseBtn.Transform = Utilities.Hide;
					EventsNavSearchBar.Transform = Utilities.Hide;
					EventsNavSearchBar.ResignFirstResponder (); 
				}, delegate { 
					EventsNavCloseBtn.Hidden = true;
					EventsNavSearchBar.Hidden = true;
					RefreshTable (); });
			};

			RefreshControl = new UIRefreshControl ();
			RefreshControl.TintColor = UIColor.FromRGB (144, 199, 62);
			RefreshControl.ValueChanged += delegate {
				GetJson (true);
			};

			foreach(var subView in EventsNavSearchBar.Subviews) {
				foreach (var subView2nd in subView.Subviews) {
					if (subView2nd.GetType () == typeof(UITextField)) {
						UITextField searchField = (UITextField)subView2nd;
						searchField.Font = UIFont.FromName ("GillSans", 13);
					}
				}
			}
		}
		#endregion

		#region ViewWillDisappear
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			if (enteredForegroundObserver != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (enteredForegroundObserver);
			}
		}
		#endregion

		#region ViewDidDisappear
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			TableView.Alpha = 1;
			EventsNavSearchBtn.Transform = Utilities.Show;
			EventsTitleLbl.Transform = Utilities.Show;
			EventsNavCloseBtn.Hidden = true;
			EventsNavSearchBar.Hidden = true;
			EventsNavSearchLoadingSpinner.StopAnimating ();
		}
		#endregion

		#region DidReceiveMemoryWarning - EMPTY
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		#endregion

		#region PrepareForSegueFunctions
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "EventSegue") {
				var row = 		TableView.IndexPathForSelectedRow;
				var keys = 		parsedEventsJson.Keys.ToArray();
				GameDetailsTableView.SetMainItemHeaderInfo(parsedEventsJson[keys[row.Section]][row.Row], 
				                                           TableView.CellAt(row).ImageView.Image);
			}
		}
		#endregion

		#region Custom Methods
		async void GetJson (bool forceRefresh = false)
		{
			ShowLoading (true);
			await Utilities.GetAsyncJson ("upcomingevents", forceRefresh);
			RefreshTable ();
			ShowLoading (false);
		}

		async void DisplaySearchResults ()
		{
			try {
				Task<string> getSearchJson = Utilities.GetSearchJson("event", EventsNavSearchBar.Text);
				var searchItems = JsonConvert.DeserializeObject<List<Game>>(await getSearchJson);
				parsedEventsJson = new Dictionary<string, List<Game>>();
				parsedEventsJson.Add("Found " + searchItems.Count() + " Result(s)", searchItems);
				TableView.Source = new EventTableSource (parsedEventsJson);
				TableView.Hidden = false;
				if (IsViewLoaded && View.Window != null && EventsNavSearchBar.Alpha.Equals (1)) {
					TableView.ReloadData ();
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						EventsNavCloseBtn.Transform = Utilities.Show;
						EventsNavSearchLoadingSpinner.Transform = Utilities.Hide;
						TableView.Alpha = 1;
					}, delegate { EventsNavSearchLoadingSpinner.StopAnimating (); });
				}
			}
			catch {
				UIAlertView alert = new UIAlertView ("Error", "Problem connecting.\n" +
				                                             "Please check your connection and try again.", null, "OK", null);
				alert.Show ();
				EventsNavCloseBtn.Transform = Utilities.Show;
				EventsNavSearchLoadingSpinner.StopAnimating ();
			}
		}

		void RefreshTable ()
		{
			string releasesJsonString = Utilities.ReadFromFile ("upcomingevents");
			parsedEventsJson = Utilities.ParseJson (releasesJsonString);

			TableView.Source = new EventTableSource (parsedEventsJson);
			TableView.Hidden = false;
			if (IsViewLoaded && View.Window != null) {
				TableView.ReloadData ();
				ShowLoading (false);
			}
		}

		void ShowLoading (bool ShowLoader)
		{
			if (ShowLoader) {
				UIView.Animate (0.4, 0, UIViewAnimationOptions.CurveEaseIn, delegate {
					LoadingText.Alpha = 0;
					LoadingErrorImg.Alpha = 0;
				}, delegate { LoadingSpinner.StartAnimating (); });
			} 
			else {
				UIView.Animate (0.4, 0, UIViewAnimationOptions.CurveEaseOut, delegate {
					LoadingText.Alpha = 0;
					LoadingErrorImg.Alpha = 0;
				}, delegate { LoadingSpinner.StopAnimating (); RefreshControl.EndRefreshing (); });
			}
		}
		#endregion

		#region EventTableSource
		class EventTableSource : UITableViewSource
		{	
			#region Class-level Variables
			protected Dictionary<string, List<Game>> tableItems;
			List<string> tableItemsKeys;
			#endregion
			
			#region EventTableSource Constructors
			public EventTableSource (Dictionary<string, List<Game>> items)
			{
				tableItems = 		items;
				tableItemsKeys = 	items.Keys.ToList ();
			}
			#endregion

			#region Section Methods
			public override int NumberOfSections (UITableView tableView)
			{
				if (tableItemsKeys.Count() == 0) {
					if (!tableItemsKeys.Contains("No Events to Show")) {
						tableItemsKeys.Add("No Events to Show");
					}
				}
				return tableItemsKeys.Count();
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				if (tableItems.Values.Count == 0)
					return 0;
				else
					return tableItems[tableItemsKeys[section]].Count;
			}

			public override float GetHeightForHeader (UITableView tableView, int section)
			{
				return 28f;
			}
			
			public override UIView GetViewForHeader (UITableView tableView, int section)
			{
				UITableViewCell cell = 	tableView.DequeueReusableCell ("SectionView");
				UILabel nameLabel = 	cell.ViewWithTag(123) as UILabel;
				nameLabel.Text = 		tableItemsKeys[section];
				return cell;
			}
			#endregion

			#region GetCell
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = 		tableView.DequeueReusableCell ("EventCell");
				cell.Accessory = 			UITableViewCellAccessory.DisclosureIndicator;
				cell.Tag = 					tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].id;
				cell.TextLabel.Text = 		tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].title;
				cell.DetailTextLabel.Text = tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].title_2 + 
					" - " + tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].platform.Replace (",", "  |  ");
				cell.ImageView.SetImage (new NSUrl (tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].cover_image_url), 
				                         UIImage.FromBundle("placeholder_image.png"), SDWebImageOptions.RefreshCached);
				return cell;
			}
			#endregion
		}
		#endregion
	}
}
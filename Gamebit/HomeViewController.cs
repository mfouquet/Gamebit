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
	public partial class HomeViewController : UITableViewController
	{
		#region Class-level Variables
		Dictionary<string, List<Game>> parsedHomeJson;
		NSObject enteredForegroundObserver;
		#endregion

		#region HomeViewController Constructors - EMPTY
		public HomeViewController (IntPtr handle) : base (handle)
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

			HomeNavSearchBtn.TouchUpInside += delegate {
				HomeNavCloseBtn.Transform = Utilities.Hide;
				HomeNavSearchBar.Transform = Utilities.Hide;
				HomeNavCloseBtn.Hidden = false;
				HomeNavSearchBar.Hidden = false;

				HomeNavSearchBar.BecomeFirstResponder ();

				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					HomeNavSearchBtn.Transform = Utilities.Hide;
					ImgVwHomeLogo.Transform = Utilities.Hide;
					TableView.Alpha = 0;

					HomeNavCloseBtn.Transform = Utilities.Show;
					HomeNavSearchBar.Transform = Utilities.Show;
				}, null);
			};

			HomeNavSearchBar.SearchButtonClicked += delegate {
				HomeNavSearchBar.ResignFirstResponder ();
				HomeNavSearchLoadingSpinner.Transform = Utilities.Hide;

				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					HomeNavSearchLoadingSpinner.StartAnimating ();
					HomeNavCloseBtn.Transform = Utilities.Hide;
					HomeNavSearchLoadingSpinner.Transform = Utilities.Show;
				}, null);

				DisplaySearchResults ();
			};

			HomeNavCloseBtn.TouchUpInside += delegate {
				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					HomeNavSearchBtn.Transform = Utilities.Show;
					ImgVwHomeLogo.Transform = Utilities.Show;
					TableView.Alpha = 1;
					HomeNavCloseBtn.Transform = Utilities.Hide;
					HomeNavSearchBar.Transform = Utilities.Hide;
					HomeNavSearchBar.ResignFirstResponder (); 
				}, delegate { 
					HomeNavCloseBtn.Hidden = true;
					HomeNavSearchBar.Hidden = true;
					RefreshTable (); });
			};

			RefreshControl = new UIRefreshControl ();
			RefreshControl.TintColor = UIColor.FromRGB (144, 199, 62);
			RefreshControl.ValueChanged += delegate {
				GetJson (true);
			};

			foreach (var subView in HomeNavSearchBar.Subviews) {
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
			HomeNavSearchBtn.Transform = Utilities.Show;
			ImgVwHomeLogo.Transform = Utilities.Show;
			HomeNavCloseBtn.Hidden = true;
			HomeNavSearchBar.Hidden = true;
			HomeNavSearchLoadingSpinner.StopAnimating ();
		}
		#endregion

		#region DidReceiveMemoryWarning - EMPTY
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		#endregion

		#region PrepareForSegue Functions
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier.Equals("HomeSegue"))  {
				var row = 		TableView.IndexPathForSelectedRow;
				var keys = 		parsedHomeJson.Keys.ToArray();
				GameDetailsTableView.SetMainItemHeaderInfo(parsedHomeJson[keys[row.Section]][row.Row], 
				                                           TableView.CellAt(row).ImageView.Image);
			}
		}
		#endregion

		#region Custom Methods
		async void GetJson (bool forceRefresh = false)
		{
			ShowLoading (true);
			await Utilities.GetAsyncJson ("home", forceRefresh);
			RefreshTable ();
			ShowLoading (false);
		}

		async void DisplaySearchResults ()
		{
			try {
				Task<string> getSearchJson = Utilities.GetSearchJson("", HomeNavSearchBar.Text);
				var searchItems = JsonConvert.DeserializeObject<List<Game>>(await getSearchJson);
				parsedHomeJson = new Dictionary<string, List<Game>>();
				parsedHomeJson.Add("Found " + searchItems.Count() + " Result(s)", searchItems);
				TableView.Source = new HomeTableSource (parsedHomeJson);
				TableView.Hidden = false;
				if (IsViewLoaded && View.Window != null && HomeNavSearchBar.Alpha.Equals (1)) {
					TableView.ReloadData ();
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						HomeNavCloseBtn.Transform = Utilities.Show;
						HomeNavSearchLoadingSpinner.Transform = Utilities.Hide;
						TableView.Alpha = 1;
					}, delegate { HomeNavSearchLoadingSpinner.StopAnimating (); });
				}
			}
			catch {
				UIAlertView alert = new UIAlertView ("Error", "Problem connecting.\n" +
				                                             "Please check your connection and try again.", null, "OK", null);
				alert.Show ();
				HomeNavSearchLoadingSpinner.StopAnimating ();
				HomeNavCloseBtn.Transform = Utilities.Show;
			}
		}

		void RefreshTable ()
		{
			string releasesJsonString = Utilities.ReadFromFile ("home");
			parsedHomeJson = Utilities.ParseJson (releasesJsonString);

			TableView.Source = new HomeTableSource (parsedHomeJson);
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

		#region HomeTableSource
		class HomeTableSource : UITableViewSource 
		{
			#region Class-level Variables
			protected Dictionary<string, List<Game>> tableItems;
			List<string> tableItemsKeys;
			#endregion
			
			#region ReleaseTableSource Constructors
			public HomeTableSource (Dictionary<string, List<Game>> items)
			{
				tableItems = 		items; 
				tableItemsKeys = 	items.Keys.ToList ();
			}
			#endregion

			#region Section Methods
			public override int NumberOfSections (UITableView tableView)
			{
				if (tableItemsKeys.Count() == 0) {
					if (!tableItemsKeys.Contains("No Games to Show")) {
						tableItemsKeys.Add("No Games to Show");
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
				UITableViewCell cell = tableView.DequeueReusableCell ("SectionView");
				UILabel nameLabel = cell.ViewWithTag (123) as UILabel;
				switch (tableItemsKeys[section]) {
				case "releases":
					nameLabel.Text = "In Stores Now";
					break;
				case "downloadables":
					nameLabel.Text = "Available for Download";
					break;
				case "events":
					nameLabel.Text = "Upcoming Events";
					break;
				default:
					nameLabel.Text = tableItemsKeys [section];
					break;
				}
				return cell;
			}
			#endregion

			#region GetCell
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				UITableViewCell cell = 	tableView.DequeueReusableCell ("HomeCell");
				cell.Accessory = 		UITableViewCellAccessory.DisclosureIndicator;
				cell.Tag = 				tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].id;
				cell.TextLabel.Text = 	tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].title;

				if (!String.IsNullOrEmpty (tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].title_2)) {
					cell.DetailTextLabel.Text = tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].title_2 + 
						" - " + tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].platform.Replace (",", "  |  ");
				} else {
					cell.DetailTextLabel.Text = tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].platform.Replace (",", "  |  ");
				}
			
				cell.ImageView.SetImage (new NSUrl (tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].cover_image_url), 
				                         UIImage.FromBundle("placeholder_image.png"), SDWebImageOptions.RefreshCached);
				return cell;
			}
			#endregion
		}
		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
	public partial class ReleasesViewController : UITableViewController
	{
		#region Class-level Variables
		int selectedSegmentBtn = 0;
		Dictionary<string, List<Game>> parsedReleasesJson;
		NSObject enteredForegroundObserver;
		#endregion

		#region ReleasesViewController Constructors - EMPTY
		public ReleasesViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetReleaseTypeButtonDisplay ();

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
			ReleasesSearchBtn.TouchUpInside += delegate {
				ReleasesNavCloseSearchBtn.Transform = Utilities.Hide;
				ReleasesSearchBar.Transform = Utilities.Hide;
				ReleasesNavCloseSearchBtn.Hidden = false;
				ReleasesSearchBar.Hidden = false;
				ReleasesSearchBar.BecomeFirstResponder ();
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					ReleasesSearchBtn.Transform = Utilities.Hide;
					ReleasesInStoresBtn.Transform = Utilities.Hide;
					ReleasesUpcomingBtn.Transform = Utilities.Hide;
					TableView.Alpha = 0;

					ReleasesNavCloseSearchBtn.Transform = Utilities.Show;
					ReleasesSearchBar.Transform = Utilities.Show;
				}, null);
			};

			ReleasesSearchBar.SearchButtonClicked += delegate {
				ReleasesSearchBar.ResignFirstResponder ();
				ReleasesNavSearchLoadingSpinner.Transform = Utilities.Hide;
				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					ReleasesNavSearchLoadingSpinner.StartAnimating ();
					ReleasesNavCloseSearchBtn.Transform = Utilities.Hide;
					ReleasesNavSearchLoadingSpinner.Transform = Utilities.Show;
				}, null);

				DisplaySearchResults ();
			};

			ReleasesNavCloseSearchBtn.TouchUpInside += delegate {
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					ReleasesSearchBtn.Transform = Utilities.Show;
					SetReleaseTypeButtonDisplay ();
					TableView.Alpha = 1;
					ReleasesNavCloseSearchBtn.Transform = Utilities.Hide;
					ReleasesSearchBar.Transform = Utilities.Hide;
					ReleasesSearchBar.ResignFirstResponder (); 
				}, delegate { 
					ReleasesNavCloseSearchBtn.Hidden = true;
					ReleasesSearchBar.Hidden = true;
					RefreshTable (); });
			};

			ReleasesInStoresBtn.TouchUpInside += delegate {
				selectedSegmentBtn = 0;
				TableView.Source = null;
				TableView.ReloadData ();
				GetJson ();
				UIView.AnimateNotify (0.2, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					SetReleaseTypeButtonDisplay ();
				}, null);
			};
			
			ReleasesUpcomingBtn.TouchUpInside += delegate {
				selectedSegmentBtn = 1;
				TableView.Source = null;
				TableView.ReloadData ();
				GetJson ();
				UIView.AnimateNotify (0.2, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					SetReleaseTypeButtonDisplay ();
				}, null);
			};

			RefreshControl = new UIRefreshControl ();
			RefreshControl.TintColor = UIColor.FromRGB (144, 199, 62);
			RefreshControl.ValueChanged += delegate {
				GetJson (true);
			};

			foreach(var subView in ReleasesSearchBar.Subviews) {
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

			ReleasesSearchBtn.Hidden = false;
			SetReleaseTypeButtonDisplay ();
			ReleasesInStoresBtn.Hidden = false;
			ReleasesUpcomingBtn.Hidden = false;
			TableView.Alpha = 1;
			ReleasesNavCloseSearchBtn.Hidden = true;
			ReleasesSearchBar.Hidden = true;
			ReleasesSearchBtn.Transform = Utilities.Show;
			ReleasesNavSearchLoadingSpinner.StopAnimating ();
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
			if (segue.Identifier.Equals("ReleaseSegue")) {
				var row = 		TableView.IndexPathForSelectedRow;
				var keys = 		parsedReleasesJson.Keys.ToArray();
				GameDetailsTableView.SetMainItemHeaderInfo(parsedReleasesJson[keys[row.Section]][row.Row], 
				                                           TableView.CellAt(row).ImageView.Image);
			}
		}
		#endregion

		#region Custom Methods
		void SetReleaseTypeButtonDisplay ()
		{
			ReleasesInStoresBtn.Alpha = selectedSegmentBtn.Equals (0) ? 1f : 0.4f;
			ReleasesInStoresBtn.Transform = selectedSegmentBtn.Equals (0) ? 
				CGAffineTransform.MakeScale (1f, 1f) : CGAffineTransform.MakeScale (0.75f, 0.75f);


			ReleasesUpcomingBtn.Alpha = selectedSegmentBtn.Equals (1) ? 1f : 0.4f;
			ReleasesUpcomingBtn.Transform = selectedSegmentBtn.Equals (1) ?
				CGAffineTransform.MakeScale (1f, 1f) : CGAffineTransform.MakeScale (0.75f, 0.75f);
		}

		async void GetJson (bool forceRefresh = false)
		{
			ShowLoading (true);
			await Utilities.GetAsyncJson (selectedSegmentBtn.Equals(0) ? "currentreleases" : "upcomingreleases", forceRefresh);
			RefreshTable ();
			ShowLoading (false);
		}

		async void DisplaySearchResults ()
		{
			try {
				Task<string> getSearchJson = Utilities.GetSearchJson("release", ReleasesSearchBar.Text);
				var searchItems = JsonConvert.DeserializeObject<List<Game>>(await getSearchJson);
				parsedReleasesJson = new Dictionary<string, List<Game>>();
				parsedReleasesJson.Add("Found " + searchItems.Count() + " Result(s)", searchItems);
				TableView.Source = new ReleaseTableSource (parsedReleasesJson);
				TableView.Hidden = false;
				if (IsViewLoaded && View.Window != null && ReleasesSearchBar.Alpha.Equals (1)) {
					TableView.ReloadData ();
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						ReleasesNavCloseSearchBtn.Transform = Utilities.Show;
						ReleasesNavSearchLoadingSpinner.Transform = Utilities.Hide;
						TableView.Alpha = 1;
					}, delegate { ReleasesNavSearchLoadingSpinner.StopAnimating (); });
				}
			}
			catch {
				UIAlertView alert = new UIAlertView ("Error", "Problem connecting.\n" +
				                                             "Please check your connection and try again.", null, "OK", null);
				alert.Show ();
				ReleasesNavCloseSearchBtn.Transform = Utilities.Show;
				ReleasesNavSearchLoadingSpinner.StopAnimating ();
			}
		}

		void RefreshTable ()
		{
			string releasesJsonString = Utilities.ReadFromFile (selectedSegmentBtn.Equals (0) ? "currentreleases" : "upcomingreleases");
			parsedReleasesJson = Utilities.ParseJson (releasesJsonString);

			TableView.Source = new ReleaseTableSource (parsedReleasesJson);
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

		#region ReleaseTableSource
		class ReleaseTableSource : UITableViewSource  
		{
			#region Class-level Variables
			protected Dictionary<string, List<Game>> tableItems;
			List<String> tableItemsKeys;
			#endregion
			
			#region ReleaseTableSource Constructors
			public ReleaseTableSource (Dictionary<string, List<Game>> items)
			{
				tableItems = 		items; 
				tableItemsKeys = 	items.Keys.ToList ();
			}
			#endregion

			#region Section Methods
			public override int NumberOfSections (UITableView tableView)
			{
				if (tableItemsKeys.Count().Equals (0)) {
					if (!tableItemsKeys.Contains("No Releases to Show")) {
						tableItemsKeys.Add("No Releases to Show");
					}
				}
				return tableItemsKeys.Count();
			}

			public override int RowsInSection (UITableView tableview, int section)
			{
				return tableItems.Values.Count.Equals (0) ? 0 : tableItems [tableItemsKeys [section]].Count;
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
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = 		tableView.DequeueReusableCell ("ReleaseCell");
				cell.Accessory = 			UITableViewCellAccessory.DisclosureIndicator;
				cell.Tag = 					tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].id;
				cell.TextLabel.Text = 		tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].title;
				cell.DetailTextLabel.Text = tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].platform.Replace (",", "  |  ");
				cell.ImageView.SetImage (new NSUrl (tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].cover_image_url), 
				                         UIImage.FromBundle("placeholder_image.png"), SDWebImageOptions.RefreshCached);
				return cell;
			}
			#endregion
		}
		#endregion
	}
}
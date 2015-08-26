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
	public partial class DownloadableViewController : UITableViewController
	{
		#region Class-level Variables
		int selectedSegmentBtn = 0;
		Dictionary<string, List<Game>> parsedDownloadsJson;
		NSObject enteredForegroundObserver;
		#endregion

		#region DownloadableViewController Constructors - EMPTY
		public DownloadableViewController (IntPtr handle) : base (handle)
		{	
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetDownloadTypeButtonDisplay ();

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
			DownloadsNavSearchBtn.TouchUpInside += delegate {
				DownloadsNavCloseBtn.Transform = Utilities.Hide;
				DownloadsNavSearchBar.Transform = Utilities.Hide;
				DownloadsNavCloseBtn.Hidden = false;
				DownloadsNavSearchBar.Hidden = false;
				DownloadsNavSearchBar.BecomeFirstResponder ();
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					DownloadsNavSearchBtn.Transform = Utilities.Hide;
					DownloadsNavOutNowBtn.Transform = Utilities.Hide;
					DownloadsNavUpcomingBtn.Transform = Utilities.Hide;

					TableView.Alpha = 0;

					DownloadsNavCloseBtn.Transform = Utilities.Show;
					DownloadsNavSearchBar.Transform = Utilities.Show;
				}, null);
			};

			DownloadsNavSearchBar.SearchButtonClicked += delegate {
				DownloadsNavSearchBar.ResignFirstResponder ();
				DownloadsNavSearchLoadingSpinner.Transform = Utilities.Hide;
				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					DownloadsNavSearchLoadingSpinner.StartAnimating ();
					DownloadsNavCloseBtn.Transform = Utilities.Hide;
					DownloadsNavSearchLoadingSpinner.Transform = Utilities.Show;
				}, null);

				DisplaySearchResults();
			};

			DownloadsNavCloseBtn.TouchUpInside += delegate {
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					DownloadsNavSearchBtn.Transform = Utilities.Show;
					SetDownloadTypeButtonDisplay ();

					TableView.Alpha = 1;

					DownloadsNavCloseBtn.Transform = Utilities.Hide;
					DownloadsNavSearchBar.Transform = Utilities.Hide;
					DownloadsNavSearchBar.ResignFirstResponder ();
				}, delegate { 
					DownloadsNavCloseBtn.Hidden = true;
					DownloadsNavSearchBar.Hidden = true;	 
					RefreshTable (); });
			};
			
			DownloadsNavOutNowBtn.TouchUpInside += delegate {
				selectedSegmentBtn = 0;
				TableView.Source = null;
				TableView.ReloadData ();
				GetJson ();
				UIView.AnimateNotify (0.2, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					SetDownloadTypeButtonDisplay ();
				}, null);
			};
			
			DownloadsNavUpcomingBtn.TouchUpInside += delegate {
				selectedSegmentBtn = 1;
				TableView.Source = null;
				TableView.ReloadData ();
				GetJson ();
				UIView.AnimateNotify (0.2, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					SetDownloadTypeButtonDisplay ();
				}, null);
			};

			RefreshControl = new UIRefreshControl ();
			RefreshControl.TintColor = UIColor.FromRGB (144, 199, 62);
			RefreshControl.ValueChanged += delegate {
				GetJson (true);
			};

			foreach(var subView in DownloadsNavSearchBar.Subviews) {
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
			DownloadsNavSearchBtn.Hidden = false;
			SetDownloadTypeButtonDisplay ();
			DownloadsNavOutNowBtn.Hidden = false;
			DownloadsNavUpcomingBtn.Hidden = false;
			TableView.Alpha = 1;
			DownloadsNavCloseBtn.Hidden = true;
			DownloadsNavSearchBar.Hidden = true;
			DownloadsNavSearchBtn.Transform = Utilities.Show;
			DownloadsNavSearchLoadingSpinner.StopAnimating ();
		}
		#endregion

		#region PrepareForSegue Functions
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "DownloadableSegue") {
				var row = 		TableView.IndexPathForSelectedRow;
				var keys = 		parsedDownloadsJson.Keys.ToArray();
				GameDetailsTableView.SetMainItemHeaderInfo(parsedDownloadsJson[keys[row.Section]][row.Row], 
					                                       TableView.CellAt(row).ImageView.Image);
			}
		}
		#endregion

		#region Custom Methods
		void SetDownloadTypeButtonDisplay ()
		{
			DownloadsNavOutNowBtn.Alpha = selectedSegmentBtn.Equals (0) ? 1f : 0.4f;
			DownloadsNavOutNowBtn.Transform = selectedSegmentBtn.Equals (0) ? 
				CGAffineTransform.MakeScale (1f, 1f) : CGAffineTransform.MakeScale (0.75f, 0.75f);


			DownloadsNavUpcomingBtn.Alpha = selectedSegmentBtn.Equals (1) ? 1f : 0.4f;
			DownloadsNavUpcomingBtn.Transform = selectedSegmentBtn.Equals (1) ?
				CGAffineTransform.MakeScale (1f, 1f) : CGAffineTransform.MakeScale (0.75f, 0.75f);
		}

		async void GetJson (bool forceRefresh = false)
		{
			ShowLoading (true);
			await Utilities.GetAsyncJson (selectedSegmentBtn == 0 ? "currentdownloadables" : "upcomingdownloadables", forceRefresh);
			RefreshTable ();
			ShowLoading (false);
		}

		async void DisplaySearchResults ()
		{
			try {
				Task<string> getSearchJson = Utilities.GetSearchJson("downloadable", DownloadsNavSearchBar.Text);
				var searchItems = JsonConvert.DeserializeObject<List<Game>>(await getSearchJson);
				parsedDownloadsJson = new Dictionary<string, List<Game>>();
				parsedDownloadsJson.Add("Found " + searchItems.Count() + " Result(s)", searchItems);
				TableView.Source = new DownloadableTableSource (parsedDownloadsJson);
				TableView.Hidden = false;
				if (IsViewLoaded && View.Window != null && DownloadsNavSearchBar.Alpha.Equals (1)) {
					TableView.ReloadData ();
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						DownloadsNavCloseBtn.Transform = Utilities.Show;
						DownloadsNavSearchLoadingSpinner.Transform = Utilities.Hide;
						TableView.Alpha = 1;
					}, delegate { DownloadsNavSearchLoadingSpinner.StopAnimating (); });
				}
			}
			catch {
				UIAlertView alert = new UIAlertView ("Error", "Problem connecting.\n" +
				                                             "Please check your connection and try again.", null, "OK", null);
				alert.Show ();
				DownloadsNavCloseBtn.Transform = Utilities.Show;
				DownloadsNavSearchLoadingSpinner.StopAnimating ();
			}
		}

		void RefreshTable ()
		{
			string downloadsJsonString = Utilities.ReadFromFile (selectedSegmentBtn.Equals (0) ? "currentdownloadables" : "upcomingdownloadables");
			parsedDownloadsJson = Utilities.ParseJson (downloadsJsonString);

			TableView.Source = new DownloadableTableSource (parsedDownloadsJson);
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

		#region DownloadableTableSource
		class DownloadableTableSource : UITableViewSource
		{	
			#region Class-level Variables
			protected Dictionary<string, List<Game>> tableItems;
			List<string> tableItemsKeys;
			#endregion
			
			#region DownloadableTableSource Constructors
			public DownloadableTableSource (Dictionary<string, List<Game>> items)
			{
				tableItems = 		items; 
				tableItemsKeys = 	items.Keys.ToList ();
			}
			#endregion

			#region Section Methods
			public override int NumberOfSections (UITableView tableView)
			{
				if (tableItemsKeys.Count() == 0) {
					if (!tableItemsKeys.Contains("No Downloads to Show")) {
						tableItemsKeys.Add("No Downloads to Show");
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
				UITableViewCell cell = 		tableView.DequeueReusableCell ("DownloadCell");
				cell.Accessory = 			UITableViewCellAccessory.DisclosureIndicator;
				cell.Tag = 					(int)tableItems[tableItemsKeys[indexPath.Section]][indexPath.Row].id;
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
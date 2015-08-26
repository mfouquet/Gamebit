using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.Social;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using MonoTouch.ObjCRuntime;
using SDWebImage;

namespace Gamebit
{
	public partial class GameDetailsTableView : UITableViewController
	{
		#region Class-level Variables
		static Game _item;
		static UIImage _coverImage;
		Game game, parentItem;
		bool isLoaded;
		SLComposeViewController slComposer;
		UIActionSheet actionSheet;
		List<string> _orderUrls;
		#endregion

		#region GameDetailsTableView Constructors
		public GameDetailsTableView (IntPtr handle) : base (handle)
		{	
		}
		#endregion

		#region Segue Functions
		public static void SetMainItemHeaderInfo (Game game, UIImage coverImage) 
		{
			_item = game;
			_coverImage = coverImage;
		}

		public static void SetParentItemHeaderInfo (Game item) 
		{
			_item.id = item.id;
			_item.cover_image_url = item.cover_image_url;
			_item.title = item.title;
			_item.release_date = item.release_date;
			_item.platform = item.platform;
			_item.esrb_rating = item.esrb_rating;
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			var mx = new UIInterpolatingMotionEffect ("center.x", UIInterpolatingMotionEffectType.TiltAlongHorizontalAxis) {
				MinimumRelativeValue = new NSNumber (-15),
				MaximumRelativeValue = new NSNumber (15)
			};
			var my = new UIInterpolatingMotionEffect ("center.y", UIInterpolatingMotionEffectType.TiltAlongVerticalAxis) {
				MinimumRelativeValue = new NSNumber (-15),
				MaximumRelativeValue = new NSNumber (15)
			};
			ImgVwBackground.AddMotionEffect (mx);
			ImgVwBackground.AddMotionEffect (my);

			if (!isLoaded) {
				NavigationItem.HidesBackButton = true;
				LoadMainItemInfo ();
				if (_coverImage == null) {
					DetailsCover.SetImage (new NSUrl (_item.cover_image_url), UIImage.FromBundle ("placeholder_image.png"), 
					                       SDWebImageOptions.RetryFailed, null);
				} else {
					DetailsCover.Image = _coverImage;
					DetailsCover.Layer.BorderWidth = 1;
					DetailsCover.Layer.BorderColor = UIColor.White.CGColor;
				}
				GetItemJson ();
			}
		}
		#endregion

		#region ViewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			BackButton.TouchUpInside += delegate { 
				this.NavigationController.PopViewControllerAnimated (true);
			};

			DetailsShowSocialNetworkViewBtn.TouchUpInside += delegate {
				DetailsNavReminderBtn.Transform = Utilities.Hide;
				DetailsSocialNetworkCloseBtn.Transform = Utilities.Hide;
				TweetBtn.Transform = Utilities.Hide;
				PostBtn.Transform = Utilities.Hide;
				DetailsNavReminderBtn.Transform = Utilities.Hide;
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					DetailsShowSocialNetworkViewBtn.Transform = Utilities.Hide;
					DetailsNavTitleLbl.Transform = Utilities.Hide;
					DetailsShowSocialNetworkViewBtn.Transform = Utilities.Hide;
					BackButton.Transform = Utilities.Hide;

					DetailsSocialNetworkCloseBtn.Hidden = false;
					TweetBtn.Hidden = false;
					PostBtn.Hidden = false;
					if (Convert.ToDateTime (_item.release_date) > DateTime.Now) {
						DetailsNavReminderBtn.Hidden = false;
						DetailsNavReminderBtn.Transform = Utilities.Show;
					}
					DetailsSocialNetworkCloseBtn.Transform = Utilities.Show;
					TweetBtn.Transform = Utilities.Show;
					PostBtn.Transform = Utilities.Show;
					}, null);
			};
				
			DetailsSocialNetworkCloseBtn.TouchUpInside += delegate {
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					DetailsShowSocialNetworkViewBtn.Transform = Utilities.Show;
					DetailsNavTitleLbl.Transform = Utilities.Show;
					DetailsShowSocialNetworkViewBtn.Transform = Utilities.Show;
					BackButton.Transform = Utilities.Show;
					if (Convert.ToDateTime(_item.release_date) > DateTime.Now) {
						DetailsNavReminderBtn.Transform = Utilities.Hide;
					}
					DetailsSocialNetworkCloseBtn.Transform = Utilities.Hide;
					TweetBtn.Transform = Utilities.Hide;
					PostBtn.Transform = Utilities.Hide;
				}, delegate { DetailsNavReminderBtn.Hidden = true; });
			};

			TweetBtn.TouchUpInside += delegate {
				slComposer = SLComposeViewController.FromService (SLServiceType.Twitter);

				if (!String.IsNullOrEmpty (game.hashtag)) {
					slComposer.SetInitialText ("#" + game.hashtag);
				}

				slComposer.CompletionHandler += (result) => {
					slComposer.DismissViewController (true, null);
				};
				PresentViewController (slComposer, true, null);
			};

			PostBtn.TouchUpInside += delegate {  	
				slComposer = SLComposeViewController.FromService (SLServiceType.Facebook);
				if (!String.IsNullOrEmpty (game.title)) {
					slComposer.SetInitialText (game.title);
				}

				slComposer.CompletionHandler += (result) => {
					slComposer.DismissViewController (true, null);
				};
				PresentViewController (slComposer, true, null);
			};
		}
		#endregion

		#region ViewWillDisappear
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			if (slComposer != null) {
				slComposer.Dispose ();
			}

			if (actionSheet != null) {
				actionSheet.Dispose ();
			}
		}
		#endregion

		#region DidReceiveMemoryWarning - EMPTY
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		#endregion

		#region PrepareForSegue Functions
		public override bool ShouldPerformSegue (string segueIdentifier, NSObject sender)
		{
			if (segueIdentifier == "WebsiteSegue" || segueIdentifier == "TrailerSegue" || segueIdentifier == "OrderSegue") {
				switch (segueIdentifier) {
				case "OrderSegue":
					if (NSUserDefaults.StandardUserDefaults.BoolForKey ("UseNativeApps")) {
						if (game.order_links.Count > 1) {
							actionSheet = new UIActionSheet ("Choose a platform");
							if (actionSheet.ButtonCount <= 0) {
								foreach (var link in game.order_links) {
									actionSheet.AddButton (link.platform);
									_orderUrls.Add (link.order_url);
								}
								actionSheet.AddButton("Cancel");
								actionSheet.CancelButtonIndex = _orderUrls.Count;
							}

							actionSheet.ShowInView(ParentViewController.View);
							actionSheet.Clicked += delegate(object a, UIButtonEventArgs b) {
								if (b.ButtonIndex == actionSheet.CancelButtonIndex) {
									this.DismissViewController (true, null);
								} else {
									OpenSharedWebApplication (_orderUrls [b.ButtonIndex]);
								}
								DetailsOrderCell.Selected = false;
								_orderUrls.Clear ();
							};

							return false;
						} else {
							foreach (var link in game.order_links) {
								OpenSharedWebApplication (link.order_url);
								DetailsOrderCell.Selected = false;
								return false;
							}
						}
					} else {
						return true;
					}
					break;
				case "TrailerSegue":
					if (NSUserDefaults.StandardUserDefaults.BoolForKey ("UseNativeApps")) {
						OpenSharedWebApplication (game.trailer_url);
						DetailsTrailerCell.Selected = false;
						return false;
					} else {
						return true;
					}
				case "WebsiteSegue":
					if (NSUserDefaults.StandardUserDefaults.BoolForKey ("UseNativeApps")) {
						OpenSharedWebApplication (game.game_url);
						DetailsWebsiteCell.Selected = false;
						return false;
					} else {
						return true;
					}
				}
				return false;
			} else {
				return true;
			}
		} 

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			switch (segue.Identifier) {
			case "WebsiteSegue":
				WebViewController.SetWebViewVariables (game, "Website");
				break;
			case "TrailerSegue":
				WebViewController.SetWebViewVariables (game, "Trailer");
				break;
			case "OrderSegue":
				WebViewController.SetWebViewVariables (game, "Order");
				break;
			case "GameSummarySegue":
				SummaryViewController.SetSummary (game, DetailsCover.Image);
				break;
			case "MainGameSegue":
				_coverImage = null;
				GameDetailsTableView.SetParentItemHeaderInfo (parentItem);
				break;
			case "Screenshot1Segue":
				ScreenshotViewController.SetURL (0, game.screenshots);
				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Fade);
				break;
			case "Screenshot2Segue":
				ScreenshotViewController.SetURL (1, game.screenshots);
				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Fade);
				break;
			case "Screenshot3Segue":
				ScreenshotViewController.SetURL (2, game.screenshots);
				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Fade);
				break;
			case "Screenshot4Segue":
				ScreenshotViewController.SetURL (3, game.screenshots);
				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Fade);
				break;
			case "Screenshot5Segue":
				ScreenshotViewController.SetURL (4, game.screenshots);
				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Fade);
				break;
			case "ReminderSegue":
				ReminderTableViewController.SetMainItemHeaderInfo (game);
				UIApplication.SharedApplication.SetStatusBarHidden (true, UIStatusBarAnimation.Fade);
				break;
			}
		}

		void OpenSharedWebApplication (string Url)
		{
			NSUrl _url = new NSUrl (Url);
			UIApplication.SharedApplication.OpenUrl(_url);
			DetailsTrailerCell.Selected = false;
			DetailsOrderCell.Selected = false;
		}

		#endregion

		#region Custom Methods
		async void GetItemJson () 
		{
			try {
				string itemText = await Utilities.GetItemJson (_item.id);
				game = JsonConvert.DeserializeObject<Game>(itemText);
				LoadItemDetails ();
			}
			catch {
				UIAlertView alert = new UIAlertView ("Error", "Problem connecting.\n" +
				                                             "Please check your connection and try again.", null, "OK", null);
				alert.Show ();
				alert.Dismissed += delegate {
					this.NavigationController.PopViewControllerAnimated (true);
				};
			}
		}

		async void GetParentItemJson ()
		{
			try {
				string parentItemText = await Utilities.GetItemJson (game.parent_game_id);
				parentItem = JsonConvert.DeserializeObject<Game> (parentItemText);
				DetailsMainTitleCell.Accessory = 				UITableViewCellAccessory.DisclosureIndicator;
				LblMainGame.Text =					 			"Main Game Info";
				DetailsMainTitleCell.UserInteractionEnabled = 	true;
			}
			catch {
				DetailsMainTitleCell.Accessory = 				UITableViewCellAccessory.None;
				DetailsMainTitleCell.TextLabel.Text = 			"Error Loading";
				DetailsMainTitleCell.TextLabel.Enabled = 		false;
				DetailsMainTitleCell.UserInteractionEnabled = 	false;

				UIAlertView alert = new UIAlertView ("Error", "Problem getting main game info.\n" +
				                                             "Please check your connection and try again later.", null, "OK", null);
				alert.Show ();
			}
		}

		void LoadMainItemInfo ()
		{
			if (_item.type.ToUpper().Equals("RELEASE")) {
				DetailsTitle.Text = _item.title;
				DetailsRelease.Text = _item.release_date;
				DetailsPlatform.Text = _item.platform.Replace (",", "  |  ");
				DetailsRating.Hidden = true;
				LoadRatingImage ();
			} else if (_item.type.ToUpper().Equals("DOWNLOADABLE")) {
				DetailsTitle.Text = _item.title;
				DetailsRelease.Text = _item.title_2;
				DetailsPlatform.Text = _item.release_date;
				DetailsRating.Text = _item.platform.Replace (",", "  |  ");
				DetailsRatingImage.Hidden = true;
			} else if (_item.type.ToUpper().Equals("EVENT")) {
				DetailsTitle.Text = _item.title;
				DetailsRelease.Text = _item.title_2;
				DetailsPlatform.Text = _item.release_date + " to";
				DetailsRating.Text = _item.event_end_date;
				DetailsRatingImage.Hidden = true;
			}
		}

		void LoadRatingImage ()
		{
			DetailsRatingImage.Image = UIImage.FromBundle (String.Format("{0}.png", _item.esrb_rating.ToLower()));
		}

		void LoadItemDetails ()
		{
			if (!String.IsNullOrEmpty(game.cover_image_url_hires)) {
				LoadHiResCoverImage ();
			}

			if (String.IsNullOrEmpty(game.description)) {
				DetailsDescription.TextLabel.Text = "No description available";
				DetailsDescription.TextLabel.Enabled = false;
			} else {
				DetailsDescription.TextLabel.Text = game.description;
			}

			if (String.IsNullOrEmpty(game.game_url)) {
				FreezeCells (DetailsWebsiteCell, LblWebsite, "No website available");
			} 

			if (String.IsNullOrEmpty(game.trailer_url)) {
				FreezeCells (DetailsTrailerCell, LblTrailer, "No trailer available");
			} 

			if (game.screenshots.Count > 0) {
				LoadThumbnails ();
				LoadBackgroundImage (game.screenshots [0].screenshot_url);
			} else {
				NoScreenshotLabel.Text = 					"No screenshots available";
				ScreenshotBtn1.Hidden = 					true;
				ScreenshotBtn2.Hidden = 					true;
				ScreenshotBtn3.Hidden = 					true;
				ScreenshotBtn4.Hidden = 					true;
				ScreenshotBtn5.Hidden = 					true;
				DetailsScreenshots.UserInteractionEnabled = false;
			}
			
			if (game.order_links.Count.Equals(0)) {
				FreezeCells (DetailsOrderCell, LblOrder, "No order links available");
			} else {
				_orderUrls = new List<string>();
			}

			if (game.parent_game_id <= 0 || game.parent_game_id == null) {
				FreezeCells (DetailsMainTitleCell, LblMainGame, "No main game info available");
			} else {
				GetParentItemJson();
			}

			DetailsDescription.Transform = Utilities.Hide;
			DetailsWebsiteCell.Transform = Utilities.Hide;
			DetailsTrailerCell.Transform = Utilities.Hide;
			DetailsScreenshots.Transform = Utilities.Hide;
			DetailsOrderCell.Transform = Utilities.Hide;
			DetailsMainTitleCell.Transform = Utilities.Hide;
			DetailsShowSocialNetworkViewBtn.Transform = Utilities.Hide;
			UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
				DetailsSpinner.Transform = Utilities.Hide;
				DetailsDescription.Hidden = 	false;
				DetailsWebsiteCell.Hidden = 	false;
				DetailsTrailerCell.Hidden = 	false;
				DetailsScreenshots.Hidden = 	false;
				DetailsOrderCell.Hidden = 		false;
				DetailsMainTitleCell.Hidden = 	false;
				DetailsShowSocialNetworkViewBtn.Hidden = false;
				DetailsShowSocialNetworkViewBtn.Transform = Utilities.Show;
				DetailsDescription.Transform = Utilities.Show;
				DetailsWebsiteCell.Transform = Utilities.Show;
				DetailsTrailerCell.Transform = Utilities.Show;
				DetailsScreenshots.Transform = Utilities.Show;
				DetailsOrderCell.Transform = Utilities.Show;
				DetailsMainTitleCell.Transform = Utilities.Show;
			}, delegate { 
				isLoaded = true; 
				DetailsSpinner.StopAnimating (); });
		}

		void FreezeCells (UITableViewCell cell, UILabel label, string textLabel)
		{
			cell.UserInteractionEnabled = 	false;
			label.Enabled = false;
			label.Text = textLabel;
		}

		void LoadHiResCoverImage ()
		{
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer();
			tapGesture.NumberOfTapsRequired = 1;
			tapGesture.AddTarget(this, new Selector("ShowHiResCover"));
			DetailsCover.AddGestureRecognizer(tapGesture);
		}

		[Export("ShowHiResCover")]
		public void OnSingleTap (UIGestureRecognizer sender)
		{
			UIImageView hiResImageView = new UIImageView (UIScreen.MainScreen.ApplicationFrame);
			hiResImageView.BackgroundColor = UIColor.Black;
			hiResImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			hiResImageView.SetImage (new NSUrl (_item.cover_image_url_hires), UIImage.FromBundle ("placeholder_image.png"), 
				SDWebImageOptions.RetryFailed, null);

			View.Add (hiResImageView);
		}

		async void LoadThumbnails ()
		{
			Task<List<UIImage>> thumbnailList = DownloadThumbnails ();

			List<UIImage> thumbnailImages = await thumbnailList;

			NoScreenshotLabel.Hidden = true;

			UIView.Animate (0.2, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
				if (thumbnailImages.Count > 0) {
					ScreenshotBtn1.SetImage (thumbnailImages[0], UIControlState.Normal);
					ScreenshotBtn1.Alpha = 1;
				}
			}, delegate {
				UIView.Animate (0.2, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
					if (thumbnailImages.Count > 1) {
						ScreenshotBtn2.SetImage (thumbnailImages[1], UIControlState.Normal);
						ScreenshotBtn2.Alpha = 1;
					}
			}, delegate {
					UIView.Animate (0.2, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
						if (thumbnailImages.Count > 2) {
							ScreenshotBtn3.SetImage (thumbnailImages[2], UIControlState.Normal);
							ScreenshotBtn3.Alpha = 1;
						}
					}, delegate {
						UIView.Animate (0.2, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
							if (thumbnailImages.Count > 3) {
								ScreenshotBtn4.SetImage (thumbnailImages[3], UIControlState.Normal);
								ScreenshotBtn4.Alpha = 1;
							}
						}, delegate {
								UIView.Animate (0.2, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
								if (thumbnailImages.Count > 4) {
									ScreenshotBtn5.SetImage (thumbnailImages[4], UIControlState.Normal);
									ScreenshotBtn5.Alpha = 1;
								}
							}, null);
						});
					});
				});
			});
			DetailsScreenshots.UserInteractionEnabled = true; 
		}

		async Task<List<UIImage>> DownloadThumbnails ()
		{
			List<UIImage> thumbnailImages = new List<UIImage> ();

			foreach (var thumbnail in game.screenshots) {
				Task<UIImage> image = LoadImage (thumbnail.thumbnail_url);
				thumbnailImages.Add (await image);
			}

			return thumbnailImages;
		}

		async Task<UIImage> LoadImage (string Url)
		{
			UIImage image = new UIImage ();
			
			if (String.IsNullOrEmpty (Url)) {
				image = UIImage.FromBundle ("Images/placeholder_image.png");
				return image;
			}
			using (var httpClient = new HttpClient()) {
				byte[] data = await httpClient.GetByteArrayAsync (Url);
				image = data == null ? UIImage.FromBundle ("Images/placeholder_image.png") : UIImage.LoadFromData(NSData.FromArray(data));
			}
			
			return image;
		}

		async void LoadBackgroundImage (string Url)
		{
			UIImage image = new UIImage ();

			if (String.IsNullOrEmpty (Url)) {
				image = UIImage.FromBundle ("Images/placeholder_image.png");
			}
			else {
				using (var httpClient = new HttpClient()) {
					byte[] data = await httpClient.GetByteArrayAsync (Url);
					image = data == null ? UIImage.FromBundle ("Images/placeholder_image.png") : UIImage.LoadFromData(NSData.FromArray(data));
				}
			}
			ImgVwBackground.Image = image;

			UIView.Animate (0.5, 0, UIViewAnimationOptions.CurveEaseIn, delegate {
				ImgVwBackground.Alpha = 1;
			}, null);
		}
		#endregion
	}
}
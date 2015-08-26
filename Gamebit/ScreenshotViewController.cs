using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Threading;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.Social;
using MonoTouch.UIKit;

namespace Gamebit
{
	public partial class ScreenshotViewController : UIViewController
	{	
		#region Class-level Variables
		static int currentImageIndex = 0;
		static List<Game.Screenshot> screenshots;
		bool isShowing = false;
		SLComposeViewController slComposer;
		#endregion

		#region ScreenshowViewController Constructors
		public ScreenshotViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Segue Functions
		public static void SetURL(int imageIndex, List<Game.Screenshot> screenshotUrls)
		{
			currentImageIndex = imageIndex;
			screenshots = 		screenshotUrls;
		}
		#endregion

		#region ViewWillAppear - EMPTY
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}
		#endregion

		#region ViewDidAppear
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			DownloadImage();
		}
		#endregion

		#region ViewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TweetButton.TouchUpInside += delegate { 
				slComposer = SLComposeViewController.FromService (SLServiceType.Twitter);
				
				if (Screenshot.Image != null) {
					slComposer.AddImage (Screenshot.Image);
				}
				
				slComposer.CompletionHandler += (result) => {
					slComposer.DismissViewController (true, null); 
				};
				PresentViewController (slComposer, true, null);
			};

			PostButton.TouchUpInside += delegate { 		
				slComposer = SLComposeViewController.FromService (SLServiceType.Facebook);

				if (Screenshot.Image != null) {
					slComposer.AddImage (Screenshot.Image);
				}
				
				slComposer.CompletionHandler += (result) => {
					slComposer.DismissViewController (true, null);
				};
				PresentViewController (slComposer, true, null);
			};

			#region Menu Buttons
			CloseButton.TouchUpInside += delegate {
				UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Fade);
				DismissViewController(true, null);
			};

			PrevBtn.TouchUpInside += delegate {
				SwipeImage ("Right");
			};

			NextBtn.TouchUpInside += delegate {
				SwipeImage ("Left");
			};
			#endregion	
			
			#region Gesture Recognizers
			UITapGestureRecognizer tapGesture = new UITapGestureRecognizer();
			tapGesture.NumberOfTapsRequired = 1;
			tapGesture.AddTarget(this, new Selector("MenuDisplay"));
			Screenshot.AddGestureRecognizer(tapGesture);

			UISwipeGestureRecognizer swipeLeftGesture = new UISwipeGestureRecognizer();
			swipeLeftGesture.AddTarget(this, new Selector ("SwipeLeftGesture"));
			swipeLeftGesture.Direction = UISwipeGestureRecognizerDirection.Left;
			View.AddGestureRecognizer (swipeLeftGesture);

			UISwipeGestureRecognizer swipeRightGesture = new UISwipeGestureRecognizer();
			swipeRightGesture.AddTarget(this, new Selector ("SwipeRightGesture"));
			swipeRightGesture.Direction = UISwipeGestureRecognizerDirection.Right;
			View.AddGestureRecognizer (swipeRightGesture);

			UISwipeGestureRecognizer swipeDownGesture = new UISwipeGestureRecognizer();
			swipeDownGesture.AddTarget(this, new Selector ("SwipeDownGesture"));
			swipeDownGesture.Direction = UISwipeGestureRecognizerDirection.Down;
			View.AddGestureRecognizer (swipeDownGesture);
			#endregion	
		}
		#endregion

		#region ViewWillDisappear
		public override void ViewWillDisappear (bool animated)
		{
			if (slComposer != null) {
				slComposer.Dispose ();
			}

			base.ViewWillDisappear (animated);
		}
		#endregion

		#region ViewDidDisappear - EMPTY
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		#endregion

		#region DidReceiveMemoryWarning - EMPTY
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		#endregion

		#region Gesture Recognizers
		[Export("MenuDisplay")]
		public void OnSingleTap (UIGestureRecognizer sender)
		{
			if (!isShowing) {
				TweetButton.Transform = CGAffineTransform.MakeScale(0f, 0f);
				PostButton.Transform = CGAffineTransform.MakeScale(0f, 0f);
				CloseButton.Transform = CGAffineTransform.MakeScale(0f, 0f);
				PrevBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
				NextBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
				TweetButton.Hidden = false;
				PostButton.Hidden = false;
				CloseButton.Hidden = false;
				PrevBtn.Hidden = false;
				NextBtn.Hidden = false;

				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					TweetButton.Transform = CGAffineTransform.MakeScale(1f, 1f);
					PostButton.Transform = CGAffineTransform.MakeScale(1f, 1f);
					CloseButton.Transform = CGAffineTransform.MakeScale(1f, 1f);
					PrevBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
					NextBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
					Screenshot.Alpha = 0.6f;
				}, delegate { isShowing = true; });

			} else {
				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					TweetButton.Transform = CGAffineTransform.MakeScale(0f, 0f);
					PostButton.Transform = CGAffineTransform.MakeScale(0f, 0f);
					CloseButton.Transform = CGAffineTransform.MakeScale(0f, 0f);
					PrevBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
					NextBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
					Screenshot.Alpha = 1;
				}, delegate { 
					isShowing = false; 
				});
			}
		}

		[Export("SwipeLeftGesture")]
		public void OnSwipeLeft (UIGestureRecognizer sender)
		{
			SwipeImage ("Left");
		}

		[Export("SwipeRightGesture")]
		public void OnSwipeRight (UIGestureRecognizer sender)
		{
			SwipeImage ("Right");
		}

		[Export("SwipeDownGesture")]
		public void OnSwipeDown (UIGestureRecognizer sender)
		{
			UIApplication.SharedApplication.SetStatusBarHidden (false, UIStatusBarAnimation.Fade);
			DismissViewController(true, null);
		}
		#endregion

		#region Custom Methods
		async void DownloadImage ()
		{
			using (var httpClient = new HttpClient()) {
				var imageBytes = httpClient.GetByteArrayAsync(new Uri(screenshots[currentImageIndex].screenshot_url));
				Screenshot.Image = UIImage.LoadFromData(NSData.FromArray(await imageBytes));
				Screenshot.Transform = CGAffineTransform.MakeTranslation(0f, 0f);
				Screenshot.ContentMode = UIViewContentMode.ScaleAspectFit;
				ScreenshotSpinner.StopAnimating();
			}
		}

		void SwipeImage (string direction)
		{
			UIView.Animate (0.5, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
				if (direction == "Right") {
					Screenshot.Transform = CGAffineTransform.MakeTranslation (+500f, 0f);
				} else {
					Screenshot.Transform = CGAffineTransform.MakeTranslation (-500f, 0f);
				}
			}, delegate { 
				ScreenshotSpinner.StartAnimating();
				if (direction == "Right") {
					currentImageIndex = currentImageIndex == screenshots.Count - 1 ? 0 : ++currentImageIndex;
				} else {
					currentImageIndex = currentImageIndex == 0 ? screenshots.Count - 1 : --currentImageIndex;
				}
				DownloadImage ();
			});
		}
		#endregion
	}
}

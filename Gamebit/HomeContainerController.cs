using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.iAd;
using MonoTouch.Security;

namespace Gamebit
{
	public partial class HomeContainerController : UIViewController
	{
		static UIImageView purchaseAdRemovalView = 	new UIImageView ();
		static ADBannerView adBannerView = 			new ADBannerView ();
		static UIImageView statusBarView = 			new UIImageView ();

		public HomeContainerController (IntPtr handle) : base (handle)
		{
		}

		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}
		
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (!Utilities.HasPurchasedAdRemoval ()) {
				View.AddSubview (statusBarView);
				View.AddSubview (purchaseAdRemovalView);
				View.AddSubview (adBannerView); 
				adBannerView.AdLoaded += new EventHandler (adBannerView_AdLoaded);
				adBannerView.ActionFinished += new EventHandler (adBannerView_ActionFinished);
				adBannerView.FailedToReceiveAd += new EventHandler<AdErrorEventArgs> (adBannerView_FailedToReceiveAd);
				statusBarView.Image = UIImage.FromBundle ("navbar-background.png");
				purchaseAdRemovalView.Image = UIImage.FromBundle ("ad-removal-banner.png");
				UIView.Animate (0.2, 0.2, UIViewAnimationOptions.CurveEaseIn, delegate {
					SetAdBannerView ();
					ShrinkContainerView ();
				}, null);
			} else {
				statusBarView.Dispose ();
				purchaseAdRemovalView.Dispose ();
				adBannerView.Dispose ();
			}
		}

		void adBannerView_AdLoaded (object sender, EventArgs e)
		{
			if (!Utilities.HasPurchasedAdRemoval ()) {
				if (adBannerView.Hidden == true) { 
					UIView.Animate (0.2, 0, UIViewAnimationOptions.CurveEaseIn, delegate {
						SetAdBannerView ();
					}, null);
				}
			} else {
				EnlargeContainerView ();
				statusBarView.Hidden = true;
				adBannerView.Hidden = true;
				purchaseAdRemovalView.Hidden = true;
				statusBarView.Dispose ();
				adBannerView.Dispose ();
				purchaseAdRemovalView.Dispose ();
			}
		}

		void adBannerView_ActionFinished (object sender, EventArgs e)
		{
			SetAdBannerView();
		}

		void adBannerView_FailedToReceiveAd (object sender, AdErrorEventArgs e)
		{
			if (!Utilities.HasPurchasedAdRemoval ()) {
				if (adBannerView != null) {
					if (adBannerView.Hidden == false) { 
						UIView.Animate (0.2, 0, UIViewAnimationOptions.CurveEaseOut, delegate {
							SetAdBannerView ();
						}, null);
					}
				}
			} else {
				EnlargeContainerView ();
				statusBarView.Hidden = true;
				adBannerView.Hidden = true;
				purchaseAdRemovalView.Hidden = true;
				statusBarView.Dispose ();
				adBannerView.Dispose ();
				purchaseAdRemovalView.Dispose ();
			}
		}

		void SetAdBannerView ()
		{
			if (adBannerView.BannerLoaded) {
				statusBarView.Hidden = 			false;
				statusBarView.Frame = 			new RectangleF (0, 0, 320, 20);
				adBannerView.Hidden = 			false;
				adBannerView.Frame = 			new RectangleF (0, 20, 320, 50);
				purchaseAdRemovalView.Hidden = 	true;
				purchaseAdRemovalView.Frame = 	new RectangleF (0, -70, 320, 50);
			} else {
				statusBarView.Hidden = 			false;
				statusBarView.Frame = 			new RectangleF (0, 0, 320, 20);
				adBannerView.Hidden = 			true;
				adBannerView.Frame = 			new RectangleF (0, -70, 320, 50);
				purchaseAdRemovalView.Hidden = 	false;
				purchaseAdRemovalView.Frame = 	new RectangleF (0, 20, 320, 50);
			}

			ShrinkContainerView ();
		}

		void EnlargeContainerView ()
		{
			homeContainerView.Frame = new RectangleF (0, 0, this.View.Frame.Size.Width, this.View.Frame.Size.Height);
		}

		void ShrinkContainerView ()
		{
			homeContainerView.Frame = new RectangleF (0, 70, this.View.Frame.Size.Width, this.View.Frame.Size.Height - 70);
		}
	}
}

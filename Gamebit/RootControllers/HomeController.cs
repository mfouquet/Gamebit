using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Gamebit
{
	public partial class HomeController : UITabBarController
	{
		public HomeController (IntPtr handle) : base (handle)
		{

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SelectedIndex = NSUserDefaults.StandardUserDefaults.IntForKey("SelectedTab");
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB (87, 130, 23);
			UINavigationBar.Appearance.SetBackgroundImage (UIImage.FromBundle ("navbar-background.png"), UIBarMetrics.Default);

			UITabBar.Appearance.TintColor = UIColor.FromRGB (87, 170, 23);

			ViewControllerSelected += delegate { 
				NSUserDefaults.StandardUserDefaults.SetInt(SelectedIndex, "SelectedTab");
				NSUserDefaults.StandardUserDefaults.Synchronize(); 
			};

		}
	}
}

using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Gamebit
{
	public partial class SettingsViewController : UITableViewController
	{
		public SettingsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			if (Utilities.HasPurchasedAdRemoval ()) {
				RemoveAdsCellLabel.Text = 	"Ad Removal Purchased";
				RemoveAdsCellImage.Image = 	UIImage.FromBundle ("settings_ad_removal_purchased-icon.png");
			}

			NativeAppSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("UseNativeApps");

			NativeAppSwitch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (NativeAppSwitch.On, "UseNativeApps");
				NSUserDefaults.StandardUserDefaults.Synchronize (); 
			};
		}
	}
}

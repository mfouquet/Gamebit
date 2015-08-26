using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Gamebit
{
	public partial class SystemsViewController : UITableViewController
	{
		#region SystemsViewController Constructors
		public SystemsViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			NavigationItem.HidesBackButton = true;
			N3DSSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("3ds");
			PCSwitch.On = 	NSUserDefaults.StandardUserDefaults.BoolForKey ("pc");
			PS3Switch.On = 	NSUserDefaults.StandardUserDefaults.BoolForKey ("ps3");
			PS4Switch.On = 	NSUserDefaults.StandardUserDefaults.BoolForKey ("ps4");
			VitaSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("vita");
			WiiUSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("wiiu");
			X360Switch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("xb360");
			XBoneSwitch.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("xbone");
		}
		#endregion

		#region ViewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavBackButton.TouchUpInside += delegate {
				this.NavigationController.PopViewControllerAnimated (true);
			};

			N3DSSwitch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (N3DSSwitch.On, "3ds"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
			PCSwitch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (PCSwitch.On, "pc"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
			PS3Switch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (PS3Switch.On, "ps3"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
			PS4Switch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (PS4Switch.On, "ps4"); 
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
			VitaSwitch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (VitaSwitch.On, "vita");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
			WiiUSwitch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (WiiUSwitch.On, "wiiu");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
			X360Switch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (X360Switch.On, "xb360");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
			XBoneSwitch.ValueChanged += delegate { 
				NSUserDefaults.StandardUserDefaults.SetBool (XBoneSwitch.On, "xbone");
				NSUserDefaults.StandardUserDefaults.Synchronize ();
			};
		}
		#endregion

		#region ViewWillDisappear - EMPTY
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}
		#endregion
	}
}

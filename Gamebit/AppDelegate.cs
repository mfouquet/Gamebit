using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.Security;
using MonoTouch.UIKit;

namespace Gamebit
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window {
			get;
			set;
		}

		public static NSString enteredForeground = new NSString("enteredForeground");

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			NSUserDefaults.StandardUserDefaults.RegisterDefaults (NSDictionary.FromFile ("Info.plist"));
			return true;
		} 

		public override void DidEnterBackground (UIApplication application)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		public override void WillEnterForeground (UIApplication application)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName (enteredForeground, null);
		}

		public override void WillTerminate (UIApplication application)
		{
		}

		public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
		{
			UIAlertView alert = new UIAlertView ("Gamebit Reminder", notification.AlertBody, null, "OK", null);
			alert.Show();
		}
	}
}


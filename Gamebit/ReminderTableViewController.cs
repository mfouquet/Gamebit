using System;
using MonoTouch.EventKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace Gamebit
{
	public partial class ReminderTableViewController : UITableViewController
	{
		#region Class-level Variables
		static Game _game;
		#endregion

		public ReminderTableViewController (IntPtr handle) : base (handle)
		{
		}

		#region Segue Functions
		public static void SetMainItemHeaderInfo (Game game) 
		{
			_game = game;
		}
		#endregion

		#region ViewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UINavigationBar.Appearance.BarTintColor = UIColor.GroupTableViewBackgroundColor;
			UINavigationBar.Appearance.SetBackgroundImage (null, UIBarMetrics.Default);
			datepckrDate.MinimumDate = DateTime.Today;
			datepckrDate.MaximumDate = DateTime.Today.AddDays (720);
			datepckrDate.SetDate (Convert.ToDateTime(_game.release_date + " 9:00:00 AM"), true);

			btnCancel.TouchUpInside += delegate {
				this.DismissViewController (true, null);
				UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB (87, 130, 23);
				UINavigationBar.Appearance.SetBackgroundImage (UIImage.FromBundle ("navbar-background.png"), UIBarMetrics.Default);
			};

			btnAdd.TouchUpInside += delegate {
				if (!swtchGamebitNot.On && !swtchCalendar.On)
				{
					new UIAlertView ( "Unable to Add", 
					                 "Please select how you would like to be notified", null,
					                 "OK", null).Show ();
				}
				if (swtchGamebitNot.On) {
					var keys = new object[] { 
						"coverImage", "itemId" 
					};
					var objects = new object[] { 
						_game.cover_image_url, _game.id 
					};
					var userInfo = NSDictionary.FromObjectsAndKeys (objects, keys);
					UILocalNotification notification = new UILocalNotification {
						FireDate = datepckrDate.Date,
						TimeZone = NSTimeZone.LocalTimeZone,
						AlertBody = _game.title_2 == "" ? 
						_game.title + "\n" + _game.release_date : 
							_game.title + "\n" + _game.title_2 + "\n" + _game.release_date,
							RepeatInterval = 0, 
							UserInfo = userInfo
					}; 
					UIApplication.SharedApplication.ScheduleLocalNotification (notification);
					if (swtchGamebitNot.On && !swtchCalendar.On)
					{
						ReminderAddedAnimation ();
					}
				}
				if (swtchCalendar.On) {
					App.Common.EventStore.RequestAccess (EKEntityType.Event, 
					                                      (bool granted, NSError e) => {
						if (granted) {
							EKEvent newEvent = EKEvent.FromStore ( App.Common.EventStore );
							newEvent.Availability = EKEventAvailability.Free;
							newEvent.StartDate = Convert.ToDateTime(_game.release_date + " 9:00:00 AM");
							newEvent.EndDate = newEvent.StartDate.AddSeconds(3600);
							newEvent.Title = String.IsNullOrEmpty(_game.title_2) ? _game.title : _game.title_2 + _game.title;
							newEvent.Notes = _game.description;
							newEvent.Url = new NSUrl(_game.game_url);
							newEvent.Calendar = App.Common.EventStore.DefaultCalendarForNewEvents;
							NSError er;
							App.Common.EventStore.SaveEvent ( newEvent, EKSpan.ThisEvent, out er );
							BeginInvokeOnMainThread ( () =>
							                         ReminderAddedAnimation ());
						}
						else {
							BeginInvokeOnMainThread ( () =>
							new UIAlertView ( "Access Denied", 
							                 "User Denied Access to Calendar Data", null,
							                 "OK", null).Show ());
						}
					});
				}
			};

		}
		#endregion

		void ReminderAddedAnimation ()
		{
			lblReminderAdded.Transform = Utilities.Hide;
			lblReminderAdded.Hidden = false;
			UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
				btnCancel.Transform = Utilities.Hide;
				btnAdd.Transform = Utilities.Hide;
				lblReminder.Transform = Utilities.Hide;
				lblReminderAdded.Transform = Utilities.Show;
			}, delegate {
				System.Threading.Thread.Sleep(3000);
				this.DismissViewController (true, null);
				UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB (87, 130, 23);
				UINavigationBar.Appearance.SetBackgroundImage (UIImage.FromBundle ("navbar-background.png"), UIBarMetrics.Default);
				}
			);
		}
	}
}

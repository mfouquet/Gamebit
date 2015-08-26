// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Gamebit
{
	[Register ("ReminderTableViewController")]
	partial class ReminderTableViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnAdd { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnCancel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIDatePicker datepckrDate { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblReminder { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblReminderAdded { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch swtchCalendar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch swtchGamebitNot { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAdd != null) {
				btnAdd.Dispose ();
				btnAdd = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (datepckrDate != null) {
				datepckrDate.Dispose ();
				datepckrDate = null;
			}

			if (lblReminder != null) {
				lblReminder.Dispose ();
				lblReminder = null;
			}

			if (swtchCalendar != null) {
				swtchCalendar.Dispose ();
				swtchCalendar = null;
			}

			if (swtchGamebitNot != null) {
				swtchGamebitNot.Dispose ();
				swtchGamebitNot = null;
			}

			if (lblReminderAdded != null) {
				lblReminderAdded.Dispose ();
				lblReminderAdded = null;
			}
		}
	}
}

// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("AdViewController")]
	partial class AdViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton NavBackButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel AdRemovalLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton AdRemovalButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton AdRemovalRestoreButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NavBackButton != null) {
				NavBackButton.Dispose ();
				NavBackButton = null;
			}

			if (AdRemovalLabel != null) {
				AdRemovalLabel.Dispose ();
				AdRemovalLabel = null;
			}

			if (AdRemovalButton != null) {
				AdRemovalButton.Dispose ();
				AdRemovalButton = null;
			}

			if (AdRemovalRestoreButton != null) {
				AdRemovalRestoreButton.Dispose ();
				AdRemovalRestoreButton = null;
			}
		}
	}
}

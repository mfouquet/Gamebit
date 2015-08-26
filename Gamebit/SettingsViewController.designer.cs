// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableViewCell RemoveAdsCell { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch NativeAppSwitch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView RemoveAdsCellImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel RemoveAdsCellLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (RemoveAdsCell != null) {
				RemoveAdsCell.Dispose ();
				RemoveAdsCell = null;
			}

			if (NativeAppSwitch != null) {
				NativeAppSwitch.Dispose ();
				NativeAppSwitch = null;
			}

			if (RemoveAdsCellImage != null) {
				RemoveAdsCellImage.Dispose ();
				RemoveAdsCellImage = null;
			}

			if (RemoveAdsCellLabel != null) {
				RemoveAdsCellLabel.Dispose ();
				RemoveAdsCellLabel = null;
			}
		}
	}
}

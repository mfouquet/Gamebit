// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("EventsViewController")]
	partial class EventsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView LoadingErrorImg { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView EventsNavSearchLoadingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel EventsTitleLbl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar EventsNavSearchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton HomeNavSearchBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton EventsNavSearchBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton EventsNavCloseBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView LoadingView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LoadingText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView LoadingSpinner { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadingErrorImg != null) {
				LoadingErrorImg.Dispose ();
				LoadingErrorImg = null;
			}

			if (EventsNavSearchLoadingSpinner != null) {
				EventsNavSearchLoadingSpinner.Dispose ();
				EventsNavSearchLoadingSpinner = null;
			}

			if (EventsTitleLbl != null) {
				EventsTitleLbl.Dispose ();
				EventsTitleLbl = null;
			}

			if (EventsNavSearchBar != null) {
				EventsNavSearchBar.Dispose ();
				EventsNavSearchBar = null;
			}

			if (HomeNavSearchBtn != null) {
				HomeNavSearchBtn.Dispose ();
				HomeNavSearchBtn = null;
			}

			if (EventsNavSearchBtn != null) {
				EventsNavSearchBtn.Dispose ();
				EventsNavSearchBtn = null;
			}

			if (EventsNavCloseBtn != null) {
				EventsNavCloseBtn.Dispose ();
				EventsNavCloseBtn = null;
			}

			if (LoadingView != null) {
				LoadingView.Dispose ();
				LoadingView = null;
			}

			if (LoadingText != null) {
				LoadingText.Dispose ();
				LoadingText = null;
			}

			if (LoadingSpinner != null) {
				LoadingSpinner.Dispose ();
				LoadingSpinner = null;
			}
		}
	}
}

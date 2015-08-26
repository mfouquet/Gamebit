// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("ReleasesViewController")]
	partial class ReleasesViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView LoadingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LoadingErrorImg { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LoadingText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView ReleasesNavSearchLoadingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ReleasesNavCloseSearchBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel ReleasesNavSplitterLbl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ReleasesSearchBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar ReleasesSearchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ReleasesInStoresBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ReleasesUpcomingBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableViewCell SectionHeader { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SectionTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView LoadingView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadingSpinner != null) {
				LoadingSpinner.Dispose ();
				LoadingSpinner = null;
			}

			if (LoadingErrorImg != null) {
				LoadingErrorImg.Dispose ();
				LoadingErrorImg = null;
			}

			if (LoadingText != null) {
				LoadingText.Dispose ();
				LoadingText = null;
			}

			if (ReleasesNavSearchLoadingSpinner != null) {
				ReleasesNavSearchLoadingSpinner.Dispose ();
				ReleasesNavSearchLoadingSpinner = null;
			}

			if (ReleasesNavCloseSearchBtn != null) {
				ReleasesNavCloseSearchBtn.Dispose ();
				ReleasesNavCloseSearchBtn = null;
			}

			if (ReleasesNavSplitterLbl != null) {
				ReleasesNavSplitterLbl.Dispose ();
				ReleasesNavSplitterLbl = null;
			}

			if (ReleasesSearchBtn != null) {
				ReleasesSearchBtn.Dispose ();
				ReleasesSearchBtn = null;
			}

			if (ReleasesSearchBar != null) {
				ReleasesSearchBar.Dispose ();
				ReleasesSearchBar = null;
			}

			if (ReleasesInStoresBtn != null) {
				ReleasesInStoresBtn.Dispose ();
				ReleasesInStoresBtn = null;
			}

			if (ReleasesUpcomingBtn != null) {
				ReleasesUpcomingBtn.Dispose ();
				ReleasesUpcomingBtn = null;
			}

			if (SectionHeader != null) {
				SectionHeader.Dispose ();
				SectionHeader = null;
			}

			if (SectionTitle != null) {
				SectionTitle.Dispose ();
				SectionTitle = null;
			}

			if (LoadingView != null) {
				LoadingView.Dispose ();
				LoadingView = null;
			}
		}
	}
}

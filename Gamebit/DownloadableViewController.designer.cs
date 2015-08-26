// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("DownloadableViewController")]
	partial class DownloadableViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView LoadingErrorImg { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView DownloadsNavSearchLoadingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DownloadsNavSplitterLbl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DownloadsNavOutNowBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DownloadsNavUpcomingBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar DownloadsNavSearchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DownloadsNavSearchBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DownloadsNavCloseBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DownloadablesOutNowBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton DownloadablesUpcomingBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView LoadingView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LoadingText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView LoadingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISegmentedControl DownloadableSgmntdBtn { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadingErrorImg != null) {
				LoadingErrorImg.Dispose ();
				LoadingErrorImg = null;
			}

			if (DownloadsNavSearchLoadingSpinner != null) {
				DownloadsNavSearchLoadingSpinner.Dispose ();
				DownloadsNavSearchLoadingSpinner = null;
			}

			if (DownloadsNavSplitterLbl != null) {
				DownloadsNavSplitterLbl.Dispose ();
				DownloadsNavSplitterLbl = null;
			}

			if (DownloadsNavOutNowBtn != null) {
				DownloadsNavOutNowBtn.Dispose ();
				DownloadsNavOutNowBtn = null;
			}

			if (DownloadsNavUpcomingBtn != null) {
				DownloadsNavUpcomingBtn.Dispose ();
				DownloadsNavUpcomingBtn = null;
			}

			if (DownloadsNavSearchBar != null) {
				DownloadsNavSearchBar.Dispose ();
				DownloadsNavSearchBar = null;
			}

			if (DownloadsNavSearchBtn != null) {
				DownloadsNavSearchBtn.Dispose ();
				DownloadsNavSearchBtn = null;
			}

			if (DownloadsNavCloseBtn != null) {
				DownloadsNavCloseBtn.Dispose ();
				DownloadsNavCloseBtn = null;
			}

			if (DownloadablesOutNowBtn != null) {
				DownloadablesOutNowBtn.Dispose ();
				DownloadablesOutNowBtn = null;
			}

			if (DownloadablesUpcomingBtn != null) {
				DownloadablesUpcomingBtn.Dispose ();
				DownloadablesUpcomingBtn = null;
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

			if (DownloadableSgmntdBtn != null) {
				DownloadableSgmntdBtn.Dispose ();
				DownloadableSgmntdBtn = null;
			}
		}
	}
}

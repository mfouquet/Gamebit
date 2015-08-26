// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("SummaryViewController")]
	partial class SummaryViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView SummaryRatingImg { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NavBackButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView SummaryTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView SummaryCover { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SummaryTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SummaryRelease { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SummaryPlatform { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SummaryRating { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SummaryText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SummaryRatingImg != null) {
				SummaryRatingImg.Dispose ();
				SummaryRatingImg = null;
			}

			if (NavBackButton != null) {
				NavBackButton.Dispose ();
				NavBackButton = null;
			}

			if (SummaryTextView != null) {
				SummaryTextView.Dispose ();
				SummaryTextView = null;
			}

			if (SummaryCover != null) {
				SummaryCover.Dispose ();
				SummaryCover = null;
			}

			if (SummaryTitle != null) {
				SummaryTitle.Dispose ();
				SummaryTitle = null;
			}

			if (SummaryRelease != null) {
				SummaryRelease.Dispose ();
				SummaryRelease = null;
			}

			if (SummaryPlatform != null) {
				SummaryPlatform.Dispose ();
				SummaryPlatform = null;
			}

			if (SummaryRating != null) {
				SummaryRating.Dispose ();
				SummaryRating = null;
			}

			if (SummaryText != null) {
				SummaryText.Dispose ();
				SummaryText = null;
			}
		}
	}
}

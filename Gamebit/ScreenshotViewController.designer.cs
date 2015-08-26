// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("ScreenshotViewController")]
	partial class ScreenshotViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView Screenshot { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton TweetButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PostButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel ButtonSplitter { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView ScreenshotSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PrevBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NextBtn { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Screenshot != null) {
				Screenshot.Dispose ();
				Screenshot = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (TweetButton != null) {
				TweetButton.Dispose ();
				TweetButton = null;
			}

			if (PostButton != null) {
				PostButton.Dispose ();
				PostButton = null;
			}

			if (ButtonSplitter != null) {
				ButtonSplitter.Dispose ();
				ButtonSplitter = null;
			}

			if (ScreenshotSpinner != null) {
				ScreenshotSpinner.Dispose ();
				ScreenshotSpinner = null;
			}

			if (PrevBtn != null) {
				PrevBtn.Dispose ();
				PrevBtn = null;
			}

			if (NextBtn != null) {
				NextBtn.Dispose ();
				NextBtn = null;
			}
		}
	}
}

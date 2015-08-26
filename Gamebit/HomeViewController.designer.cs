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
	[Register ("HomeViewController")]
	partial class HomeViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton HomeNavCloseBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar HomeNavSearchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton HomeNavSearchBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView HomeNavSearchLoadingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel HomeTitleLbl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ImgVwHomeLogo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LoadingErrorImg { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView LoadingSpinner { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel LoadingText { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView LoadingView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadingErrorImg != null) {
				LoadingErrorImg.Dispose ();
				LoadingErrorImg = null;
			}

			if (HomeNavSearchLoadingSpinner != null) {
				HomeNavSearchLoadingSpinner.Dispose ();
				HomeNavSearchLoadingSpinner = null;
			}

			if (HomeTitleLbl != null) {
				HomeTitleLbl.Dispose ();
				HomeTitleLbl = null;
			}

			if (HomeNavSearchBar != null) {
				HomeNavSearchBar.Dispose ();
				HomeNavSearchBar = null;
			}

			if (HomeNavCloseBtn != null) {
				HomeNavCloseBtn.Dispose ();
				HomeNavCloseBtn = null;
			}

			if (HomeNavSearchBtn != null) {
				HomeNavSearchBtn.Dispose ();
				HomeNavSearchBtn = null;
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

			if (ImgVwHomeLogo != null) {
				ImgVwHomeLogo.Dispose ();
				ImgVwHomeLogo = null;
			}
		}
	}
}

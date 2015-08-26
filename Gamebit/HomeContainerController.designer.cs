using MonoTouch.Foundation;

namespace Gamebit
{
	[Register ("HomeContainerController")]
	partial class HomeContainerController
	{
		[Outlet]
		MonoTouch.UIKit.UIView homeContainerView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (homeContainerView != null) {
				homeContainerView.Dispose ();
				homeContainerView = null;
			}
		}
	}
}

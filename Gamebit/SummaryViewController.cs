using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Gamebit
{
	public partial class SummaryViewController : UIViewController
	{
		#region Class-level Variables
		static Game _game;
		static UIImage _coverImage;
		#endregion
		
		#region SummaryViewController Constructors
		public SummaryViewController (IntPtr handle) : base (handle)
		{	
		}
		#endregion

		#region Segue Functions
		public static void SetSummary (Game game, UIImage coverImage) 
		{
			_game = game;
			_coverImage = coverImage;
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationItem.HidesBackButton = true;
			LoadMainItemInfo ();
		}
		#endregion

		#region ViewDidAppear - EMPTY
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}
		#endregion

		#region ViewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavBackButton.TouchUpInside += delegate {
				this.NavigationController.PopViewControllerAnimated (true);
			};
		}
		#endregion

		#region ViewWillDisappear
		public override void ViewWillDisappear (bool animated)
		{
			if (_coverImage != null) {
				_coverImage.Dispose ();
			}

			base.ViewWillDisappear (animated);
		}
		#endregion

		#region ViewDidDisappear - EMPTY
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		#endregion

		#region DidReceiveMemoryWarning - EMPTY
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		#endregion
	
		#region Custom Methods
		
		void LoadMainItemInfo ()
		{
			if (_game.type.ToUpper() == "RELEASE") {
				SummaryTitle.Text = _game.title;
				SummaryRelease.Text = _game.release_date;
				SummaryPlatform.Text = _game.platform.Replace (",", "  |  ");
				SummaryRating.Hidden = true;
				FindRatingImage ();
			} else if (_game.type.ToUpper() == "DOWNLOADABLE") {
				SummaryTitle.Text = _game.title;
				SummaryRelease.Text = _game.title_2;
				SummaryPlatform.Text = _game.release_date;
				SummaryRating.Text = _game.platform.Replace (",", "  |  ");
				SummaryRatingImg.Hidden = true;
			} else if (_game.type.ToUpper() == "EVENT"){
				SummaryTitle.Text = _game.title;
				SummaryRelease.Text = _game.title_2;
				SummaryPlatform.Text = _game.release_date + " to";
				SummaryRating.Text = _game.event_end_date;
				SummaryRatingImg.Hidden = true;
			}
			SummaryCover.Image = _coverImage;
			SummaryTextView.Text = _game.description;
		}
		
		void FindRatingImage ()
		{
			SummaryRatingImg.Image = UIImage.FromBundle (String.Format("{0}.png", _game.esrb_rating.ToLower()));
		}
		#endregion
	
	}
}

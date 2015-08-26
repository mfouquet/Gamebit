using System;
using System.Collections.Generic;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Net;
using System.Web;
using MonoTouch.CoreGraphics;
using MonoTouch.Social;
using MonoTouch.AVFoundation;

namespace Gamebit
{
	public partial class WebViewController : UIViewController
	{
		#region Class-level Variables
		UIActionSheet actionSheet;
		static Game _game;
		static string _url, _title;
		bool isLoaded = false;
		SLComposeViewController slComposer;
		#endregion

		#region WebViewController Constructors
		public WebViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion
			
		#region Segue Functions
		public static void SetWebViewVariables(Game gameInfo, string title)
		{
			_game = 	gameInfo;
			_title = 	title;
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			AVAudioSession audioSession = AVAudioSession.SharedInstance();
			audioSession.Init ();
			//audioSession.Category = AVAudioSession.CategoryPlayback;
			audioSession.SetActive (true, new NSError ());
			//MonoTouch.AudioToolbox.AudioSession.Initialize ();

			WebLoadingSpinner.StopAnimating ();
			NavigationItem.HidesBackButton = true;
			WebTitle.Text = _title;

			WebView.ScalesPageToFit = true;

			switch (_title) {
			case "Website":
				_url = _game.game_url;
				LoadWebUrl(_game.game_url);
				break;
			case "Trailer":
				_url = _game.trailer_url;
				LoadWebUrl(_game.trailer_url);
				break;
			case "Order":
				if (_game.order_links.Count == 1) {
					_url = _game.order_links[0].order_url;
					LoadWebUrl(_url);
				} else {
					actionSheet = new UIActionSheet ("Choose a platform");
					foreach (var link in _game.order_links) {
						actionSheet.AddButton (link.platform);
					}

					actionSheet.AddButton("Cancel");
					actionSheet.CancelButtonIndex = _game.order_links.Count;
					actionSheet.ShowInView(ParentViewController.View);
					
					actionSheet.Clicked += delegate (object a, UIButtonEventArgs b) {
						if (b.ButtonIndex == actionSheet.CancelButtonIndex) {
							this.DismissViewController (true, null);
							this.NavigationController.PopViewControllerAnimated (true);
						} else {
							_url = _game.order_links[b.ButtonIndex].order_url;
							LoadWebUrl (_url);
						}
					};
				}
				break;
			}
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

			NavBackBtn.TouchUpInside += delegate {
				this.NavigationController.PopViewControllerAnimated (true);
			};

			WebShowSocialNetworksBtn.TouchUpInside += delegate {
				WebNavCloseBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
				WebTweetBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
				WebPostBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
				WebNavCloseBtn.Hidden = false;
				WebTweetBtn.Hidden = false;
				WebPostBtn.Hidden = false;
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					WebTitle.Transform = CGAffineTransform.MakeScale(0f, 0f);
					WebShowSocialNetworksBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
					NavBackBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);

					WebNavCloseBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
					WebTweetBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
					WebPostBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
				}, null);
			};

			WebNavCloseBtn.TouchUpInside += delegate {
				UIView.AnimateNotify (0.4, 0, 0.65f, 0.0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					WebNavCloseBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
					WebTweetBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
					WebPostBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);

					WebTitle.Transform = CGAffineTransform.MakeScale(1f, 1f);
					WebShowSocialNetworksBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
					NavBackBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
				}, null);
			};

			WebTweetBtn.TouchUpInside += delegate {	
				slComposer = SLComposeViewController.FromService (SLServiceType.Twitter);
				slComposer.AddUrl (new NSUrl (_url));

				slComposer.CompletionHandler += (result) => {
					slComposer.DismissViewController (true, null); 
				};
				PresentViewController (slComposer, true, null);
			};

			WebPostBtn.TouchUpInside += delegate {		
				slComposer = SLComposeViewController.FromService (SLServiceType.Facebook);
				slComposer.AddUrl (new NSUrl (_url));

				slComposer.CompletionHandler += (result) => {
					slComposer.DismissViewController (true, null);
				};
				PresentViewController (slComposer, true, null);
			};
			
			WebBackBtn.TouchUpInside += delegate { 
				WebView.GoBack (); 
			};

			WebForwardBtn.TouchUpInside += delegate { 
				WebView.GoForward (); 
			};

			WebToolBarRefreshBtn.TouchUpInside += delegate { 
				if (WebTweetBtn.Hidden == false) {
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						WebNavCloseBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebTweetBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebPostBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebTitle.Transform = CGAffineTransform.MakeScale(1f, 1f);
						NavBackBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
						WebLoadingSpinner.Transform = CGAffineTransform.MakeScale(1f, 1f);
						WebLoadingSpinner.StartAnimating ();
					}, null);
				}
				else {
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						WebShowSocialNetworksBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebLoadingSpinner.Transform = CGAffineTransform.MakeScale(1f, 1f);
						WebLoadingSpinner.StartAnimating ();
					}, null);
				}

				WebView.Reload (); 
				if (!isLoaded) {
					LoadWebUrl(_url);
				}
			};
			
			WebView.LoadStarted += delegate { 
				if (WebTweetBtn.Hidden == false) {
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						WebNavCloseBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebTweetBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebPostBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebTitle.Transform = CGAffineTransform.MakeScale(1f, 1f);
						NavBackBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
						WebLoadingSpinner.Transform = CGAffineTransform.MakeScale(1f, 1f);
						WebLoadingSpinner.StartAnimating ();
					}, null);
				}
				else {
					UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
						WebShowSocialNetworksBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
						WebLoadingSpinner.Transform = CGAffineTransform.MakeScale(1f, 1f);
						WebLoadingSpinner.StartAnimating ();
					}, null);
				}
			};

			WebView.LoadFinished += delegate {
				WebShowSocialNetworksBtn.Hidden = false;
				WebShowSocialNetworksBtn.Transform = CGAffineTransform.MakeScale(0f, 0f);
				UIView.AnimateNotify (0.4, 0, 0.65f, 0f, UIViewAnimationOptions.CurveEaseIn, delegate {
					WebShowSocialNetworksBtn.Transform = CGAffineTransform.MakeScale(1f, 1f);
					WebLoadingSpinner.Transform = CGAffineTransform.MakeScale(0f, 0f);
				}, delegate {
					WebLoadingSpinner.StopAnimating ();
					isLoaded = true;
				});
			};

			WebView.LoadError += delegate (object sender, UIWebErrorArgs e) { 
				if (!e.Error.ToString ().Contains ("NSURLErrorDomain")) {
					WebLoadingSpinner.StopAnimating ();
					WebShowSocialNetworksBtn.Alpha = 1;
					UIAlertView alert = new UIAlertView ("Error", "Problem connecting.\n" +
					                                             "Please check your connection and try again.", null, "OK", null);
					alert.Show (); 
				}
			};
		}
		#endregion

		#region ViewWillDisappear
		public override void ViewWillDisappear (bool animated)
		{
			if (actionSheet != null) {
				actionSheet.Dispose ();
			}

			if (slComposer != null) {
				slComposer.Dispose ();
			}
			_url = null;

			base.ViewWillDisappear (animated);
		}
		#endregion

		#region DidReceiveMemoryWarning - EMPTY
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		#endregion
			
		#region Custom Methods
		void LoadFinished ()
		{
			WebLoadingSpinner.StopAnimating ();
			WebShowSocialNetworksBtn.Alpha = 1;
			isLoaded = true;
		}

		void LoadWebUrl (string Url)
		{
			NSUrl urlAddress = new NSUrl (Url);
			NSUrlRequest urlRequest = new NSUrlRequest (urlAddress);
			WebView.LoadRequest (urlRequest);
		}

		void FadeDetailsInfo (int AlphaValue)
		{
			UIView.Animate (0.4, 0, UIViewAnimationOptions.CurveEaseInOut, delegate {
				WebView.Alpha = AlphaValue;
				NavBackBtn.Alpha = AlphaValue;
				WebNavCloseBtn.Alpha = AlphaValue;
				WebTweetBtn.Alpha = AlphaValue;
				WebPostBtn.Alpha = AlphaValue;
				WebBackBtn.Alpha = AlphaValue;
				WebForwardBtn.Alpha = AlphaValue;
				WebToolBarRefreshBtn.Alpha = AlphaValue;
			}, null);
		}
		#endregion	
	}
}


using System;
using System.Collections.Generic;
using MonoTouch.StoreKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Security;

namespace Gamebit
{
	public partial class AdViewController : UITableViewController
	{
		#region Class-level Variables
		public static string adRemovalProductId = "com.gamebitapp.storekit.adremoval";
		List<string> product;
		bool pricesLoaded = false;
		NSObject priceObserver, requestObserver;
		AdManager iap;
		#endregion

		#region AdviewController Constructors
		public AdViewController (IntPtr handle) : base (handle)
		{
			product = 	new List<string> () { adRemovalProductId };
			iap = 		new AdManager ();
		}
		#endregion

		#region ViewWillAppear
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationItem.HidesBackButton = true;

			AdRemovalButton.Layer.CornerRadius = 10.0f;
			AdRemovalButton.ClipsToBounds = true;

			AdRemovalRestoreButton.Layer.CornerRadius = 10.0f;
			AdRemovalRestoreButton.ClipsToBounds = true;

			if (!Utilities.HasPurchasedAdRemoval ()) {
				if (iap.CanMakePayments ()) {
					if (!pricesLoaded) {
						iap.RequestProductData (product);
					}
				} else {
					UpdateLabelAndButton ("Please ensure Purchases are enabled in the Settings menu.", false);
				}

				priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (AdManager.InAppPurchaseManagerProductsFetchedNotification,
			                                                                (notification) => {
					var info = notification.UserInfo;
					var NSadRemovalProductId = new NSString (adRemovalProductId);

					if (info != null) {
						pricesLoaded = 	true;
						var product = 	(SKProduct)info.ObjectForKey (NSadRemovalProductId);

						UpdateLabelAndButton ("Love the app, but hate the Ads? Tap below and they're gone for good." +
							"\n\n$" + product.Price, true);
					} else {
						UpdateLabelAndButton ("Unable to access store.  Please check your connection and try again.", false);
					}
				});


				priceObserver = NSNotificationCenter.DefaultCenter.AddObserver (AdManager.InAppPurchaseManagerTransactionSucceededNotification,
			                                                                (notification) => {
					UpdateLabelAndButton ("Ad removal purchased.\nIf you need to restore your purchase, tap the Restore Purchase button below.", true);
				});

				requestObserver = NSNotificationCenter.DefaultCenter.AddObserver (AdManager.InAppPurchaseManagerRequestFailedNotification,
			                                                                  (notification) => {
					UpdateLabelAndButton ("Unable to access store.  Please check your connection and try again.", false);
				});
			} else {
				UpdateLabelAndButton ("Ad removal purchased.\nIf you need to restore your purchase, tap the Restore Purchase button below.", true);
			}
		}
		#endregion

		#region ViewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavBackButton.TouchUpInside += delegate {
				this.NavigationController.PopViewControllerAnimated (true);
			};

			AdRemovalButton.TouchUpInside += (sender, e) => {
				iap.PurchaseProduct (adRemovalProductId);
			};

			AdRemovalRestoreButton.TouchUpInside += (sender, e) => {
				iap.Restore ();
			};
		}
		#endregion

		#region ViewWillDisappear
		public override void ViewWillDisappear (bool animated)
		{
			if (priceObserver != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (priceObserver);
			}

			if (priceObserver != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (requestObserver);
			}

			if (iap != null) {
				iap.Dispose ();
			}

			base.ViewWillDisappear (animated);
		}
		#endregion

		void UpdateLabelAndButton (string labelText, bool enableButton)
		{
			AdRemovalButton.Enabled = enableButton;
			AdRemovalRestoreButton.Enabled = enableButton;
			AdRemovalLabel.Text = labelText;
		}

		public static void Purchase (string productId)
		{
			var s = new SecRecord (SecKind.GenericPassword) {
				Label = "Item Label",
				Description = "Item description",
				Account = "Account",
				Service = "Service",
				Comment = "Your comment here",
				ValueData = NSData.FromString ("none"),
				Generic = NSData.FromString ("PurchasedAdRemoval", NSStringEncoding.UTF8)
			};

			var err = SecKeyChain.Add (s);
		}
	}
}

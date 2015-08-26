using System;
using System.Linq;
using MonoTouch.StoreKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Gamebit {
	internal class CustomPaymentObserver : SKPaymentTransactionObserver {
		private AdManager theManager;
		
		public CustomPaymentObserver(AdManager manager)
		{
			theManager = manager;
		}

		public override void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction[] transactions)
		{
			foreach (SKPaymentTransaction transaction in transactions)
			{
				switch (transaction.TransactionState)
				{
				case SKPaymentTransactionState.Purchasing:
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
					break;
				case SKPaymentTransactionState.Purchased:
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					theManager.CompleteTransaction (transaction);
					InvokeOnMainThread (() => {
						UIAlertView alert = new UIAlertView ("Note!", 
						                                     		 "It may take a few moments for the ad removal to register as purchased.",
						                                     		 null, "OK", null);
						alert.Show ();
					});
					break;
				case SKPaymentTransactionState.Failed:
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					theManager.FailedTransaction (transaction);
					break;
				case SKPaymentTransactionState.Restored:
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
					theManager.RestoreTransaction (transaction);
					break;
				default:
					break;
				}
			}
		}
		
		public override void PaymentQueueRestoreCompletedTransactionsFinished (SKPaymentQueue queue)
		{
			if (queue.Transactions.Count() > 0) {
				InvokeOnMainThread (() => {
					AdViewController.Purchase (AdViewController.adRemovalProductId);
					UIAlertView alert = new UIAlertView ("Note!", 
					                                             "It may take a few moments for the ad removal to register as purchased.",
					                                             null, "OK", null);
					alert.Show ();
				});
			} else {
				InvokeOnMainThread (() => {
					UIAlertView alert = new UIAlertView ("Error", 
					                                     		 "No prior purchase to restore.",
					                                    		 null, "Ok", null);
					alert.Show ();
				});
			}
		}

		public override void RestoreCompletedTransactionsFailedWithError (SKPaymentQueue queue, NSError error)
		{
			InvokeOnMainThread (() => {
				UIAlertView alert = new UIAlertView ("Error", 
			                                    			 "Unable to restore purchase.",
			                                   				 null, "Ok", null);
				alert.Show ();
			});
		}
	}
}


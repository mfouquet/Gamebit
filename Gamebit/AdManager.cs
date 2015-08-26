using System;
using System.Collections.Generic;
using MonoTouch.StoreKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Gamebit {
	public class AdManager : SKProductsRequestDelegate {
		public static NSString InAppPurchaseManagerProductsFetchedNotification = 		new NSString 
																						("InAppPurchaseManagerProductsFetchedNotification");
		public static NSString InAppPurchaseManagerTransactionFailedNotification = 		new NSString 
																						("InAppPurchaseManagerTransactionFailedNotification");
		public static NSString InAppPurchaseManagerTransactionSucceededNotification = 	new NSString 
																						("InAppPurchaseManagerTransactionSucceededNotification");
		public static NSString InAppPurchaseManagerRequestFailedNotification = 			new NSString 
																						("InAppPurchaseManagerRequestFailedNotification");
		SKProductsRequest productsRequest;
		CustomPaymentObserver theObserver;
		SKProduct[] products;
		
		public static NSAction Done { get; set; }

		public AdManager ()
		{
			theObserver = new CustomPaymentObserver (this);
			SKPaymentQueue.DefaultQueue.AddTransactionObserver (theObserver);
		}

		public bool CanMakePayments ()
		{
			return SKPaymentQueue.CanMakePayments;	
		}

		public void RequestProductData (List<string> productIds)
		{
			var array = new NSString[productIds.Count];
			for (var i = 0; i < productIds.Count; i++) {
				array[i] = new NSString (productIds[i]);
			}
			NSSet productIdentifiers = 	NSSet.MakeNSObjectSet<NSString> (array);			
			productsRequest  = 			new SKProductsRequest(productIdentifiers);
			productsRequest.Delegate = 	this;
			productsRequest.Start ();
		}

		public override void ReceivedResponse (SKProductsRequest request, SKProductsResponse response)
		{
			products = response.Products;
			
			NSDictionary userInfo = null;
			if (products.Length > 0) {
				NSObject[] productIdsArray = new NSObject[response.Products.Length];
				NSObject[] productsArray = new NSObject[response.Products.Length];
				for (int i = 0; i < response.Products.Length; i++) {
					productIdsArray[i] = new NSString (response.Products[i].ProductIdentifier);
					productsArray[i] = response.Products[i];
				}
				userInfo = NSDictionary.FromObjectsAndKeys (productsArray, productIdsArray);
			}
			NSNotificationCenter.DefaultCenter.PostNotificationName (InAppPurchaseManagerProductsFetchedNotification,this,userInfo);
		}
		
		public void PurchaseProduct (string appStoreProductId)
		{
			SKPayment payment = SKPayment.PaymentWithProduct (products[0]);	
			SKPaymentQueue.DefaultQueue.AddPayment (payment);
		}

		public void CompleteTransaction (SKPaymentTransaction transaction)
		{
			var productId = transaction.Payment.ProductIdentifier;
			AdViewController.Purchase(productId);
			FinishTransaction (transaction, true);
		}

		public void RestoreTransaction (SKPaymentTransaction transaction)
		{
			var productId = transaction.OriginalTransaction.Payment.ProductIdentifier;
			AdViewController.Purchase(productId);
			FinishTransaction (transaction, true);
		}

		public void FailedTransaction (SKPaymentTransaction transaction)
		{
			FinishTransaction (transaction,false);
		}

		public void FinishTransaction (SKPaymentTransaction transaction, bool wasSuccessful)
		{
			SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
			
			using (var pool = new NSAutoreleasePool ()) {
				NSDictionary userInfo = NSDictionary.FromObjectsAndKeys(new NSObject[] { transaction },new NSObject[] { new NSString ("transaction") });
				if (wasSuccessful) {
					NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerTransactionSucceededNotification,this,userInfo);
				} else {
					NSNotificationCenter.DefaultCenter.PostNotificationName(InAppPurchaseManagerTransactionFailedNotification,this,userInfo);
				}
			}
		}

		public override void RequestFailed (SKRequest request, NSError error)
		{
			using (var pool = new NSAutoreleasePool ()) {
				NSDictionary userInfo = NSDictionary.FromObjectsAndKeys(new NSObject[] { error },new NSObject[] { new NSString ("error") });
				NSNotificationCenter.DefaultCenter.PostNotificationName (InAppPurchaseManagerRequestFailedNotification,this,userInfo);
			}
		}

		public void Restore ()
		{
			SKPaymentQueue.DefaultQueue.RestoreCompletedTransactions();
		}
	}
}
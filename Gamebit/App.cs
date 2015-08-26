using System;
using MonoTouch.EventKit;
using MonoTouch.CoreGraphics;
using MonoTouch.Social;

namespace Gamebit
{
	public class App
	{
		public static App Common
		{
			get { return common; }
		}
		private static App common;


		public EKEventStore EventStore
		{
			get { return eventStore; }
		}
		protected EKEventStore eventStore;


		static App ()
		{
			common = new App();
		}
		protected App () 
		{
			eventStore = new EKEventStore ();
		}
	}
}


using System;
using System.Collections.Generic;
using System.IO;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Net;
using System.Web;

namespace Gamebit
{
	public class Game 
	{
		public int id { 
			get; 
			set; 
		}

		public string type { 
			get; 
			set; 
		}

		public string title { 
			get; 
			set; 
		}

		public string title_2 { 
			get; 
			set; 
		}

		public int? parent_game_id { 
			get; 
			set; 
		}

		public string developer { 
			get; 
			set; 
		}

		public string genre { 
			get; 
			set; 
		}

		public string platform { 
			get; 
			set; 
		}

		public string release_date { 
			get; 
			set; 
		}

		public string event_end_date { 
			get; 
			set; 
		}

		public string esrb_rating { 
			get; 
			set; 
		}

		public string description { 
			get; 
			set; 
		}

		public string cover_image_url_hires { 
			get; 
			set; 
		}

		public string cover_image_url { 
			get; 
			set; 
		}

		public string trailer_url { 
			get; 
			set; 
		}

		public string game_url { 
			get; 
			set; 
		}

		public string review_score { 
			get; 
			set; 
		}

		public string hashtag { 
			get; 
			set; 
		}

		public List<OrderUrl> order_links { 
			get; 
			set; 
		}

		public List<Screenshot> screenshots { 
			get; 
			set; 
		}

		public class OrderUrl 
		{
			public string platform { 
				get; 
				set; 
			}

			public string order_url { 
				get; 
				set; 
			}
		}
		
		public class Screenshot 
		{
			public string screenshot_url { 
				get; 
				set; 
			}

			public string thumbnail_url { 
				get; 
				set; 
			}
		}

		public Game ()
		{
			order_links = new List<OrderUrl>();
			screenshots = new List<Screenshot>();
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.Security;
using MonoTouch.Social;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using MonoTouch.CoreGraphics;

namespace Gamebit
{
	public class Utilities
	{
		const string baseAddress = 	"http://www.itsgone.com/";
		static string baseDir  = 	Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "..");
		public static CGAffineTransform Show = CGAffineTransform.MakeScale(1f, 1f);
		public static CGAffineTransform Hide = CGAffineTransform.MakeScale(0f, 0f);

		public async static Task GetAsyncJson (string JsonUrl, bool forceRefresh)
		{
			try {
				if (String.IsNullOrEmpty (ReadFromFile (JsonUrl)) || forceRefresh) {
					using (var handler = new HttpClientHandler{ Credentials = new NetworkCredential("gamebitapp", "gone") }) {
						using (var httpClient = new HttpClient(handler)) {
							string body = await httpClient.GetStringAsync (String.Format ("{0}/{1}?limit=100", baseAddress, JsonUrl));
							WriteToFile (JsonUrl, body);
						}
					}
				}
			}
			catch {
				UIAlertView alert = new UIAlertView ("Error", "Problem connecting.\n" +
				                                             "Please check your connection \n" +
				                                             "and pull down to try again.", null, "OK", null);
				alert.Show ();
			}
		}

		public static void GetJson (string JsonUrl, bool forceRefresh)
		{
			using (var _webClient = 		new WebClient ()) {
				_webClient.Credentials = 	new NetworkCredential ("gamebitapp", "gone");
				_webClient.Proxy = 			null;
				_webClient.BaseAddress = 	baseAddress + "/" + JsonUrl;
				_webClient.QueryString.Add ("limit", "100");
				_webClient.DownloadStringCompleted += (sender, e) => {
					if (e.Error == null) {
						WriteToFile(JsonUrl, e.Result);
					}
				};

				_webClient.DownloadStringAsync(new Uri(_webClient.BaseAddress));
			}
		}

		public async static Task<string> GetSearchJson (string type, string term)
		{
			try {
				using (var handler = new HttpClientHandler { Credentials = new NetworkCredential("gamebitapp", "gone") }) {
					using (var httpClient = new HttpClient(handler)) {
						Task<string> searchJson = httpClient.GetStringAsync(String.Format ("{0}/search?type={1}&term={2}",
						                                                                   baseAddress, type, term));
						return await searchJson;
					}
				}
			}
			catch {
				return null;
			}
		}

		public async static Task<string> GetItemJson (int? id)
		{
			try {
				using (var handler = new HttpClientHandler{ Credentials = new NetworkCredential("gamebitapp", "gone") }) {
					using (var httpClient = new HttpClient(handler)) {
						Task<string> itemJson = httpClient.GetStringAsync(String.Format ("{0}/details?id={1}", baseAddress, id.ToString()));
						return await itemJson;
					}
				}
			}
			catch {
				return null;
			}
		}

		static void WriteToFile (string type, string json)
		{
			if (!Directory.Exists (Path.Combine (baseDir, "Library/Caches/Jsons/"))) {
				Directory.CreateDirectory (Path.Combine (baseDir, "Library/Caches/Jsons/"));
			}
			File.WriteAllText (Path.Combine (baseDir, String.Format ("Library/Caches/Jsons/{0}Json.txt", type)), json);
			File.SetLastWriteTime (Path.Combine (baseDir, String.Format ("Library/Caches/Jsons/{0}Json.txt", type)), DateTime.Now);
		}

		public static string ReadFromFile (string type)
		{
			string text = "";
			if (!Directory.Exists (Path.Combine (baseDir, "Library/Caches/Jsons/"))) {
				return text;
			}

			if (File.Exists(Path.Combine (baseDir, String.Format ("Library/Caches/Jsons/{0}Json.txt", type)))) {
				if (File.GetLastWriteTime (Path.Combine (baseDir, String.Format ("Library/Caches/Jsons/{0}Json.txt", type))).DayOfYear <
					DateTime.Now.DayOfYear) {
					return text;
				}

				text = File.ReadAllText (Path.Combine (baseDir, String.Format ("Library/Caches/Jsons/{0}Json.txt", type)));
			}

			return text;
		}

		public static Dictionary<string, List<Game>> ParseJson(string json)
		{
			var platforms = Utilities.BuildPlatformQueryString ().ToUpper ().Split(',').ToList ();

			if (!String.IsNullOrEmpty (json) && !json.Equals ("[]")) {
				Dictionary<string, List<Game>> parsedJson =
					JsonConvert.DeserializeObject<Dictionary<string,List<Game>>> (json);

				foreach (var p in parsedJson) {
					p.Value.RemoveAll (game => game.platform.ToUpper ()
					                   .Split (',').ToList ().Except (platforms).Count () == 0);
				}

				parsedJson = (from kv in parsedJson
				              where kv.Value.Count > 0
				              select kv).ToDictionary (kv => kv.Key, kv => kv.Value);
				return parsedJson;
			}
			else {
				Dictionary<string, List<Game>> empty = new Dictionary<string, List<Game>> ();
				return empty;
			}
		}

		public static string BuildPlatformQueryString()
		{
			string systems = "";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("3ds"))
				systems += "3ds ";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("pc"))
				systems += "pc ";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("ps3"))
				systems += "ps3 ";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("ps4"))
				systems += "ps4 ";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("vita"))
				systems += "vita ";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("wiiu"))
				systems += "wiiu ";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("xb360"))
				systems += "xb360 ";
			if (!NSUserDefaults.StandardUserDefaults.BoolForKey ("xbone"))
				systems += "xbone";

			return String.IsNullOrEmpty(systems) ? "none" : systems.Trim().Replace(" ", ",");
		}

		public static bool HasPurchasedAdRemoval()
		{
			var rec = new SecRecord (SecKind.GenericPassword) {
				Generic = NSData.FromString ("PurchasedAdRemoval", NSStringEncoding.UTF8)
			};

			SecStatusCode res;
			SecKeyChain.QueryAsRecord (rec, out res);
			return res == SecStatusCode.Success;
		}
	}
}

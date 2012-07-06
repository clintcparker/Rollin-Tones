using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using WrapNetflix;

namespace hackathon.Infrastructure
{
	public class RRNetflix
	{
		public const string ConsumerKey = "e99kjwajtpcyjfpgvt3amzts";
		public const string SharedSecret = "MkCYUAgRjc";


		public HttpServerUtility Server;
		public string GetAccessTokenResponse(string token, string tokenSecret)
		{
			var client = new WebClient();
			string baseuristr = "http://api.netflix.com/oauth/access_token";
			Uri baseUri = new Uri(baseuristr);
			var OAuth = new OAuthBase();
			var timeStamp = OAuth.GenerateTimeStamp();
			var nonce = OAuth.GenerateNonce();
			var QStr = string.Format("?oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method=HMAC-SHA1&oauth_timestamp={2}&oauth_version=1.0" + "&oauth_token={3}", ConsumerKey, nonce, timeStamp, token);
			var baseUrlwithQStr = baseuristr + QStr;
			var oauth_signature = OAuth.GenerateSignature(baseUri, ConsumerKey, SharedSecret, token,
														  tokenSecret, "GET", timeStamp, nonce,
														  out baseuristr, out QStr);

			
			var finalUrl = string.Format("{0}&oauth_signature={1}", baseUrlwithQStr, OAuth.UrlEncode(oauth_signature));

			for (var a = 0; a <= 20; a++)
			{
				try
				{
					return client.DownloadString(finalUrl);
				}
				catch (Exception e)
				{
					if (a == 20)
					{
						throw e;
					}
				}
			}
			return "";

			//http://api.netflix.com/oauth/access_token?oauth_consumer_key=e99kjwajtpcyjfpgvt3amzts&oauth_nonce=2652002&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1341449398&oauth_version=1.0oauth_token=ftkkv968chcywzr9ugr4h7ze&oauth_signature=IujYO5+zuuNp+zN11Xe+K9Puhps=
			//http://api.netflix.com/oauth/access_token?oauth_consumer_key=e99kjwajtpcyjfpgvt3amzts&oauth_nonce=249946915344920&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1341447989&oauth_token=yaqg4cfepwn7zvpq7hzka9v8&oauth_version=1.0&oauth_signature=JdH%2F4xaKl4nHdyP%2F%2FYouofAJCTQ%3D

		}

		public string GetRequestTokenResponse()
		{

			var client = new WebClient();
			string baseuristr = "http://api.netflix.com/oauth/request_token";
			Uri baseUri = new Uri(baseuristr); 
			var OAuth = new OAuthBase();
			var timeStamp = OAuth.GenerateTimeStamp();
			var nonce = OAuth.GenerateNonce();
			var QStr = string.Format("?oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method=HMAC-SHA1&oauth_timestamp={2}&oauth_version=1.0", ConsumerKey, nonce, timeStamp);
			var baseUrlwithQStr = baseuristr + QStr;
			var oauth_signature = OAuth.GenerateSignature(baseUri, ConsumerKey, SharedSecret, "", "", "GET", timeStamp, nonce, out baseuristr, out QStr);

			var finalUrl = string.Format("{0}&oauth_signature={1}", baseUrlwithQStr, OAuth.UrlEncode(oauth_signature));

			for (var a = 0; a < 5; a++)
			{
				try
				{
					return client.DownloadString(finalUrl);
				}
				catch (Exception e)
				{
					if (a == 4)
					{
						throw e;
					}
				}
			}
			return "";
		}

		public string GetUserResponse(string token, string tokenSecret, string userid)
		{

			var client = new WebClient();
			string baseuristr = string.Format("http://api-public.netflix.com/users/{0}", userid);
			Uri baseUri = new Uri(baseuristr);
			var OAuth = new OAuthBase();
			var timeStamp = OAuth.GenerateTimeStamp();
			var nonce = OAuth.GenerateNonce();
			var QStr = string.Format("?oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method=HMAC-SHA1&oauth_timestamp={2}&oauth_version=1.0" + "&oauth_token={3}", ConsumerKey, nonce, timeStamp, token);
			var baseUrlwithQStr = baseuristr + QStr;
			var oauth_signature = OAuth.GenerateSignature(baseUri, ConsumerKey, SharedSecret, token,
														  tokenSecret, "GET", timeStamp, nonce,
														  out baseuristr, out QStr);


			var finalUrl = string.Format("{0}&oauth_signature={1}", baseUrlwithQStr, OAuth.UrlEncode(oauth_signature));

			for (var a = 0; a <= 20; a++)
			{
				try
				{
					return client.DownloadString(finalUrl);
				}
				catch (Exception e)
				{
					if (a == 20)
					{
						throw e;
					}
				}
			}
			return "";
		}

		public WrapNetflix.User GetUser(string token, string tokenSecret)
		{
			var nc = new NetflixConnection(ConsumerKey, SharedSecret);
			var at = new AccessToken(GetAccessTokenResponse(token, tokenSecret));
			var u = new WrapNetflix.User(at, nc);
			return u;
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;
using Lastfm.Services;
using hackathon.Models;

namespace hackathon.Infrastructure
{
	public class RRLastfm
	{
		private const string API_KEY = "6e99426a65cf9052c7ba0222331bb784";
		private const string API_SECRET = "32a88644630e50ad086faf5a500c21f5";

		public static List<hackathon.Models.Rtone.Song> GetRecommendations(List<hackathon.Models.Rtone.Song> oSoundTrack)
		{
			Session session = new Session(API_KEY, API_SECRET);
			var RSoundTrack = new List<Rtone.Song>();

			foreach (var song in oSoundTrack)
			{
				if (song.Artist == null || song.Artist.Trim() == string.Empty)
				{
					continue;
				}
				Rtone.Song similarSong = GetSimilarTracks(song.Artist, song.Title);

				if (similarSong != null)
				{
					RSoundTrack.Add(similarSong);
				}
			}
			return RSoundTrack;
		}

		public static string GetSongPlayLink(Rtone.Song song)
		{
			var OAuth = new OAuthBase();
			string apiUrl =
				string.Format(
					"http://ws.audioscrobbler.com/2.0/?method=track.getsimilar&artist={0}&track={1}&api_key={2}&limit=1&autocorrect=1",
					OAuth.UrlEncode(song.Artist), OAuth.UrlEncode(song.Title), RRLastfm.API_KEY);

			WebRequest webRequest = WebRequest.Create(apiUrl);

			var xmlDoc = new XmlDocument();

			var client = new WebClient();
			try
			{
				var xmlString = client.DownloadString(apiUrl);
				xmlDoc.LoadXml(xmlString);
				XmlNodeList urlNodeList = xmlDoc.GetElementsByTagName("url");

				song.PlayLink = urlNodeList[0].InnerText;

				return urlNodeList[0].InnerText;
			}
			catch (Exception e)
			{
				return "";
			}
		}

		public static Rtone.Song GetSimilarTracks(string artist, string trackTitle)
		{
			var OAuth = new OAuthBase();
			string apiUrl =
				string.Format(
					"http://ws.audioscrobbler.com/2.0/?method=track.getsimilar&artist={0}&track={1}&api_key={2}&limit=1&autocorrect=1", OAuth.UrlEncode(artist), OAuth.UrlEncode(trackTitle), RRLastfm.API_KEY);

			WebRequest webRequest = WebRequest.Create(apiUrl);

			var xmlDoc = new XmlDocument();

			var client = new WebClient();
			try
			{
				var xmlString = client.DownloadString(apiUrl);
				xmlDoc.LoadXml(xmlString);
				XmlNodeList artistNodeList = xmlDoc.GetElementsByTagName("artist");
				XmlNodeList urlNodeList = xmlDoc.GetElementsByTagName("url");
				XmlNodeList trackNodeList = xmlDoc.GetElementsByTagName("track");

				var song = new Rtone.Song();
				song.Artist = artistNodeList[0]["name"].InnerText;
				song.PlayLink = urlNodeList[0].InnerText;
				song.Title = trackNodeList[0]["name"].InnerText;

				return song;
			}
			catch (Exception e)
			{
				return GetSimilarTrackByArtist(artist, trackTitle);
			}
		}

		public static Rtone.Song GetSimilarTrackByArtist(string artist, string trackTitle)
		{
			var OAuth = new OAuthBase();
			string apiUrl =
				string.Format(
					"http://ws.audioscrobbler.com/2.0/?method=artist.gettoptracks?method=track.getsimilar&artist={0}&api_key={1}&limit=1&autocorrect=1",
					OAuth.UrlEncode(artist), RRLastfm.API_KEY);

			WebRequest webRequest = WebRequest.Create(apiUrl);

			var xmlDoc = new XmlDocument();

			var client = new WebClient();
			try
			{
				var xmlString = client.DownloadString(apiUrl);
				xmlDoc.LoadXml(xmlString);
				XmlNodeList artistNodeList = xmlDoc.GetElementsByTagName("artist");
				XmlNodeList urlNodeList = xmlDoc.GetElementsByTagName("url");
				XmlNodeList trackNodeList = xmlDoc.GetElementsByTagName("track");


				if (trackNodeList[0]["name"].InnerText.ToLower() == trackTitle.ToLower())
				{
					return null;
				}

				var song = new Rtone.Song
				           	{
				           		Artist = artistNodeList[0]["name"].InnerText,
				           		PlayLink = urlNodeList[0].InnerText,
				           		Title = trackNodeList[0]["name"].InnerText
				           	};



				return song;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
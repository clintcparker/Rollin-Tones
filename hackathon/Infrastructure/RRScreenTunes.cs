using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using hackathon.Models;

namespace hackathon.Infrastructure
{
	public static class JsonTools
	{
		#region Dynamic Json Deserializer Stuff

		/// <summary>
		/// Deserialize to a DataContract
		/// </summary>
		/// <typeparam name="T">DataContract type</typeparam>
		/// <param name="json">valid json string mapping to T</param>
		/// <returns>Populated T from json</returns>
		public static T DeserializeToDataContract<T>(string json)
		{
			var obj = Activator.CreateInstance<T>();
			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				var serializer = new DataContractJsonSerializer(obj.GetType());
				obj = (T)serializer.ReadObject(ms);
				return obj;
			}
		}

		/// <summary>
		/// Class to convert a json string to a dynamic object
		/// </summary>
		private sealed class DynamicJsonConverter : JavaScriptConverter
		{
			public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}

				return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
			}

			public static object Deserialize(string json)
			{
				var jss = new JavaScriptSerializer();
				jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

				return jss.Deserialize(json, typeof(object));
			}

			public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override IEnumerable<Type> SupportedTypes
			{
				get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
			}

			#region Nested type: DynamicJsonObject

			private sealed class DynamicJsonObject : DynamicObject
			{
				private readonly IDictionary<string, object> _Dictionary;

				public DynamicJsonObject(IDictionary<string, object> dictionary)
				{
					if (dictionary == null)
						throw new ArgumentNullException("dictionary");
					_Dictionary = dictionary;
				}

				public override string ToString()
				{
					var sb = new StringBuilder("{");
					ToString(sb);
					return sb.ToString();
				}

				private void ToString(StringBuilder sb)
				{
					var firstInDictionary = true;
					foreach (var pair in _Dictionary)
					{
						if (!firstInDictionary)
							sb.Append(",");
						firstInDictionary = false;
						var value = pair.Value;
						var name = pair.Key;
						if (value is string)
						{
							sb.AppendFormat("\"{0}\":\"{1}\"", name, value);
						}
						else if (value is IDictionary<string, object>)
						{
							new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
						}
						else if (value is ArrayList)
						{
							sb.Append(name + ":[");
							var firstInArray = true;
							foreach (var arrayValue in (ArrayList)value)
							{
								if (!firstInArray)
									sb.Append(",");
								firstInArray = false;
								if (arrayValue is IDictionary<string, object>)
									new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
								else if (arrayValue is string)
									sb.AppendFormat("\"{0}\"", arrayValue);
								else
									sb.AppendFormat("{0}", arrayValue);

							}
							sb.Append("]");
						}
						else
						{
							sb.AppendFormat("{0}:{1}", name, value);
						}
					}
					sb.Append("}");
				}

				public override bool TryGetMember(GetMemberBinder binder, out object result)
				{
					if (!_Dictionary.TryGetValue(binder.Name, out result))
					{
						// return null to avoid exception.  caller can check for null this way...
						result = null;
						return true;
					}

					var dictionary = result as IDictionary<string, object>;
					if (dictionary != null)
					{
						result = new DynamicJsonObject(dictionary);
						return true;
					}

					var arrayList = result as ArrayList;
					if (arrayList != null && arrayList.Count > 0)
					{
						if (arrayList[0] is IDictionary<string, object>)
						{
							result =
								new List<object>(
									arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
						}
						else
						{
							result = new List<object>(arrayList.Cast<object>());
						}
					}

					return true;
				}
			}

			#endregion
		}

		/// <summary>
		/// Converts json string to a dynamic object
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public static dynamic ParseToDynamic(string json)
		{
			return DynamicJsonConverter.Deserialize(json);
		}
		#endregion Dynamic Json Deserializer Stuff
	}

	[DataContract]
	internal class ScreenTunesMovie
	{
		[DataMember(Name = "id")]
		public int ID { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "tv")]
		public bool IsTVShow { get; set; }

		[DataMember(Name = "year")]
		public string Year { get; set; }
	}

	[DataContract]
	internal class ScreenTunesSong
	{
		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "artist")]
		public string Artist { get; set; }

		internal static Rtone.Song ConvertToRTonesSong(ScreenTunesSong screenTunesSong)
		{
			Rtone.Song song = new Rtone.Song
			                  	{
									Title = screenTunesSong.Title,
									Artist = screenTunesSong.Artist
			                  	};

			return song;
		}
	}
       
	public class RRScreenTunes
	{
		public static readonly string ScreenTunesGetProgramByName = "http://api.screentunes.com/n/";
		public static readonly string ScreenTunesGetSoundtrackByS = "http://api.screentunes.com/i/";

		public static List<hackathon.Models.Rtone.Song> GetOSoundTrack(Models.Rtone.Movie movie)
		{

			WebRequest webRequest = WebRequest.Create(ScreenTunesGetProgramByName + movie.Title);

			List<Rtone.Song> originalRToneSongs = new List<Rtone.Song>();

			try
			{
				using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
				{
					if (reader.Peek() > 0)
					{
						string response = reader.ReadToEnd();
						object[] screenTuneProgramResults = (JsonTools.ParseToDynamic(response) as object[]);

						if (screenTuneProgramResults != null)
						{
							List<ScreenTunesMovie> screenTunesMovies =
								screenTuneProgramResults.Select(m => JsonTools.DeserializeToDataContract<ScreenTunesMovie>(m.ToString())).ToList();

							ScreenTunesMovie screenTunesMovie = GetProperProgramFromResponse(screenTunesMovies, movie);

							if (screenTunesMovie != null)
							{
								List<ScreenTunesSong> screenTunesSongs = GetOriginalSongs(screenTunesMovie);

								originalRToneSongs = screenTunesSongs.ConvertAll(ScreenTunesSong.ConvertToRTonesSong).ToList();

								/*foreach (Rtone.Song originalRToneSong in originalRToneSongs)
								{
									originalRToneSong.PlayLink = RRLastfm.GetSongPlayLink(originalRToneSong);
								}*/
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}

			return originalRToneSongs;
		}

		private static ScreenTunesMovie GetProperProgramFromResponse(List<ScreenTunesMovie> screenTunesMovies, Models.Rtone.Movie movie)
		{
			return screenTunesMovies.FirstOrDefault(m => m.Title.ToLower() == movie.Title.ToLower() && (Int32.Parse(m.Year) >= (movie.Year - 1) && Int32.Parse(m.Year) <= (movie.Year + 1)));
		}

		private static List<ScreenTunesSong> GetOriginalSongs(ScreenTunesMovie screenTunesMovie)
		{
			WebRequest webRequest = WebRequest.Create(ScreenTunesGetSoundtrackByS + screenTunesMovie.ID);

			List<ScreenTunesSong> screenTunesSongs = new List<ScreenTunesSong>();

			try
			{
				using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
				{
					if (reader.Peek() > 0)
					{
						string response = reader.ReadToEnd();
						object[] screenTuneSongsResults = (JsonTools.ParseToDynamic(response) as object[]);

						if (screenTuneSongsResults != null)
						{

							foreach (object[] songResultArray in screenTuneSongsResults)
							{
								var song = new ScreenTunesSong();

								if (songResultArray[0] == null || songResultArray[1] == null)
								{
									continue;
								}

								song.Title = songResultArray[0].ToString();
								song.Artist = GetArtistFromScreenTuneResult(songResultArray);

								screenTunesSongs.Add(song);
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}

			return screenTunesSongs;
		}

		private static string GetArtistFromScreenTuneResult(object[] screenTuneSong)
		{
			string artist = screenTuneSong[1].ToString();

			if (artist.Trim() == string.Empty)
			{
				if (screenTuneSong.Length >= 4)
				{
					string[] artists = screenTuneSong[3].ToString().Split('|');

					artist =
						artists[0].Replace("Written by", "").Replace("Written and Performed by", "").Replace("Performed by", "").Replace(
							"Words and Music by", "").Replace("Music by", "").Trim();
				}
			}

			return artist;
		}
	}
}
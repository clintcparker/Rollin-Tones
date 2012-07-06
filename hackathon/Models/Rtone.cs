using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WrapNetflix;
using hackathon.Infrastructure;

namespace hackathon.Models
{
	public class Rtone
	{
		public class Movie
		{
			public string Title { get; set; }
			public string ImgLink { get; set; }
			public List<Song> OSoundTrack { get; set; }
			public List<Song> RSoundTrack { get; set; }
			public int? Year { get; set; }


			public static Movie GenerateFromRHI(RentalHistoryItem rhi)
			{
				var m = new Movie();
				m.Title = rhi.Title;
				var ImgUri = rhi.get_BoxArt(BoxArtSize.Large);
				m.ImgLink = ImgUri.AbsoluteUri;
				m.Year = rhi.ReleaseYear;
				m.OSoundTrack = RRScreenTunes.GetOSoundTrack(m);

				if (m.OSoundTrack != null && m.OSoundTrack.Count > 0)
				{
					m.RSoundTrack = RRLastfm.GetRecommendations(m.OSoundTrack);
				}

				return m;
			}
		}

		public class Song
		{
			public string Title { get; set; }
			public string Artist { get; set; }
			public string PlayLink { get; set; }

		}
	}
}

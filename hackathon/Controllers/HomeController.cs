 using System;
using System.Collections.Generic;
using System.Linq;
 using System.Net;
 using System.Web;
using System.Web.Mvc;
 using System.Xml;
 using WrapNetflix;
using WrapNetflix.My;
using WrapNetflix.My.Resources;
 using hackathon.Infrastructure;
 using hackathon.Models;

namespace hackathon.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/

		public ActionResult Index()
		{

			List<Rtone.Movie> Movies = new List<Rtone.Movie>();
			if (Session["User"] == null && Session["1stSecret"] != null)
			{
				return RedirectToAction("SignOut");
			}

			if (Session["User"] != null)
			{

				WrapNetflix.User u = (WrapNetflix.User)Session["User"];
				var rha = u.GetRentalHistory(RentalHistoryType.Watched, 10);
				foreach (var m in rha.Where(m => m.TitleRef.Contains("/movies/")))
				{
					if (Movies.Count == 5)
					{
						break;
					}

					var movie = Rtone.Movie.GenerateFromRHI(m);
					if (movie.OSoundTrack != null && movie.OSoundTrack.Count > 0)
					{
						Movies.Add(movie);
					}
				}
			}
			else
			{


				Movies.Add(TheBigLebowski());
				Movies.Add(Adventureland());
				
				Movies.Add(PullingJohn());
			}

			return View(Movies);

		}

		
		public ActionResult SignIn()
		{
			var rrNetflix = new RRNetflix();
			if (Request.QueryString["oauth_token"] != null)
			{
				var u = rrNetflix.GetUser(Request.QueryString["oauth_token"],Session["1stSecret"].ToString());
				Session["User"] = u;
				var xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(rrNetflix.GetUserResponse(u.Token, u.TokenSecret, u.UserId));
				XmlNodeList a = xmlDoc.GetElementsByTagName("first_name");
				XmlNodeList b = xmlDoc.GetElementsByTagName("last_name");
				var Name = string.Format("{0} {1}", a[0].InnerText, b[0].InnerText);
				Session["Name"] = Name;
				return RedirectToAction("Index");
			}
			var qscoll = HttpUtility.ParseQueryString(rrNetflix.GetRequestTokenResponse());
			Session["1stSecret"] = qscoll["oauth_token_secret"];

			return Redirect(
				string.Format(
					"https://api-user.netflix.com/oauth/login?oauth_token={0}&oauth_consumer_key={1}&application_name=Rollin+Tones&oauth_callback={2}",
					qscoll["oauth_token"], RRNetflix.ConsumerKey,
					Server.UrlEncode("http://" +Request.ServerVariables["HTTP_HOST"] + "/Home/SignIn")));

			//		https://api-user.netflix.com/oauth/login?oauth_token=z7gtwrsw9g5hx88bj6v4h2jq&oauth_consumer_key=e99kjwajtpcyjfpgvt3amzts&application_name=Rollin+Tones&oauth_callback=http%3A%2F%2Fdeveloper.netflix.com%2Fwalkthrough%23bottom
		}

		public ActionResult SignOut()
		{
			Session.Abandon();
			return RedirectToAction("Index");
		}

		private List<Rtone.Movie> DefaultMovies()
		{
			List<Rtone.Movie> movies = new List<Rtone.Movie>();
			var netflixConnection = new WrapNetflix.NetflixConnection(RRNetflix.ConsumerKey, RRNetflix.SharedSecret);

			const string imageUrlPre = "http://cdn-0.nflximg.com";

			var searchedMovie = netflixConnection.Catalog.TitleSearch("Watchmen", 1).FirstOrDefault();

			if (searchedMovie != null)
			{
				var movie = new Rtone.Movie
				            	{
									ImgLink = imageUrlPre + searchedMovie.get_BoxArt(BoxArtSize.Large).AbsolutePath,
									Title = searchedMovie.Title,
									Year = searchedMovie.ReleaseYear
				            	};

				movie.OSoundTrack = RRScreenTunes.GetOSoundTrack(movie);
				movie.RSoundTrack = RRLastfm.GetRecommendations(movie.OSoundTrack);

				movies.Add(movie);
			}

			searchedMovie = netflixConnection.Catalog.TitleSearch("walk the line", 1).FirstOrDefault();

			if (searchedMovie != null)
			{
				var movie = new Rtone.Movie
				{
					ImgLink = imageUrlPre + searchedMovie.get_BoxArt(BoxArtSize.Large).AbsolutePath,
					Title = searchedMovie.Title,
					Year = searchedMovie.ReleaseYear
				};

				movie.OSoundTrack = RRScreenTunes.GetOSoundTrack(movie);
				movie.RSoundTrack = RRLastfm.GetRecommendations(movie.OSoundTrack);

				movies.Add(movie);
			}

			return movies;
		}

		private Rtone.Movie TheBigLebowski()
		{
			var m1 = new Rtone.Movie();
			m1.ImgLink = "http://cdn-0.nflximg.com/en_us/boxshots/large/1181532.jpg";
			m1.Title = "The Big Lebowski";
			m1.Year = 1998;
			m1.OSoundTrack = new List<Rtone.Song>();
			m1.RSoundTrack = new List<Rtone.Song>();
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Bob Dylan",
				Title = "The Man In Me"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Yma Sumac",
				Title = "Ataypura"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Booker T. & the M.G.s",
				Title = "'Alan Alch' and 'Dominic Frontiere'"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Townes van Zandt",
				Title = "Dead Flowers"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Don Van Vliet",
				Title = "Her Eyes Are a Blue Million Miles"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Gipsy Kings",
				Title = "Hotel California"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Nina Simone",
				Title = "I Got It Bad & That Ain't Good"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Monks",
				Title = "I Hate You"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Kenny Rogers",
				Title = "Just Dropped In (To See What Condition My Condition Was In)"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Creedence Clearwater Revival",
				Title = "Lookin' Out My Back Door"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Henry Mancini",
				Title = "Lujon"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Elvis Costello",
				Title = "My Mood Swings"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Bob Dylan",
				Title = "Three Angels",
				PlayLink = "http://www.last.fm/music/Bob%2bDylan/_/Three%2bAngels"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Piero Piccioni",
				Title = "Traffic Boom",
				PlayLink = "http://www.last.fm/music/Piero%2bPiccioni/_/Traffic%2bBoom"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Townes Van Zandt",
				Title = "Tecumseh Valley",
				PlayLink = "http://www.last.fm/music/Townes%2bVan%2bZandt/_/Tecumseh%2bValley"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Don van Vliet",
				Title = "Her Eyes Are A Blue Million Miles",
				PlayLink = "http://www.last.fm/music/Don%2bvan%2bVliet/_/Her%2bEyes%2bAre%2bA%2bBlue%2bMillion%2bMiles"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Gipsy Kings",
				Title = "Hotel California (Spanish Mix)",
				PlayLink = "http://www.last.fm/music/Gipsy%2bKings/_/Hotel%2bCalifornia%2b(Spanish%2bMix)"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Nina Simone",
				Title = "Feeling Good",
				PlayLink = "http://www.last.fm/music/Nina%2bSimone/_/Feeling%2bGood"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Monks",
				Title = "Oh, How To Do Now",
				PlayLink = "http://www.last.fm/music/The%2bMonks/_/Oh%252c%2bHow%2bTo%2bDo%2bNow"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Statler Brothers",
				Title = "Oh, How To Do Now",
				PlayLink = "http://www.last.fm/music/The%2bStatler%2bBrothers/_/Flowers%2bOn%2bThe%2bWall"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Creedence Clearwater Revival",
				Title = "Travelin' Band",
				PlayLink = "http://www.last.fm/music/Creedence%2bClearwater%2bRevival/_/Travelin%2527%2bBand"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Piero Piccioni",
				Title = "Traffic Boom",
				PlayLink = "http://www.last.fm/music/Piero%2bPiccioni/_/Traffic%2bBoom"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Bob Dylan",
				Title = "The Man In Me",
				PlayLink = "http://www.last.fm/music/Bob%2bDylan/_/The%2bMan%2bIn%2bMe"
			});
			return m1;
		}

		private Rtone.Movie Adventureland()
		{
			var m1 = new Rtone.Movie();
			m1.ImgLink = "http://cdn-1.nflximg.com/en_us/boxshots/large/70099787.jpg";
			m1.Title = "Adventureland";
			m1.Year = 2008;
			m1.OSoundTrack = new List<Rtone.Song>();
			m1.RSoundTrack = new List<Rtone.Song>();
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Brian Kenney, Ian Berkowitz",
				Title = "Adventureland Theme Song"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Judas Priest",
				Title = "Breaking the Law"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Whitesnake",
				Title = "Here I Go Again"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Falco",
				Title = "Rock Me Amadeus"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Crowded House",
				Title = "Don't Dream It's Over"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Velvet Underground",
				Title = "Pale Blue Eyes"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Cure",
				Title = "Just Like Heaven"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Replacements",
				Title = "Unsatisfied"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Rolling Stones",
				Title = "Tops"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Replacements",
				Title = "Bastards of Young"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Lou Reed",
				Title = "Satellite of Love"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "'Garry Beers' (as Garry William Beers), 'Andrew Farriss' (as Andrew C. Farriss)",
				Title = "Don't Change"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "Nick Lowe",
				Title = "So It Goes"
			});
			m1.OSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Velvet Underground",
				Title = "Here She Comes Now"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Judas Priest",
				Title = "Living After Midnight",
				PlayLink = "http://www.last.fm/music/Judas%2bPriest/_/Living%2bAfter%2bMidnight"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Whitesnake",
				Title = "Is This Love",
				PlayLink = "http://www.last.fm/music/Whitesnake/_/Is%2bThis%2bLove"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Falco",
				Title = "Der Kommissar",
				PlayLink = "http://www.last.fm/music/Falco/_/Der%2bKommissar"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Crowded House",
				Title = "Something So Strong",
				PlayLink = "http://www.last.fm/music/Crowded%2bHouse/_/Something%2bSo%2bStrong"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Velvet Underground",
				Title = "Some Kinda Love",
				PlayLink = "http://www.last.fm/music/The%2bVelvet%2bUnderground/_/Some%2bKinda%2bLove"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Cure",
				Title = "Friday I'm In Love",
				PlayLink = "http://www.last.fm/music/The%2bCure/_/Friday%2bI%2527m%2bIn%2bLove"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Replacements",
				Title = "Seen Your Video",
				PlayLink = "http://www.last.fm/music/The%2bReplacements/_/Seen%2bYour%2bVideo"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Rolling Stones",
				Title = "Worried About You",
				PlayLink = "http://www.last.fm/music/The%2bRolling%2bStones/_/Worried%2bAbout%2bYou"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Replacements",
				Title = "Left of the Dial",
				PlayLink = "http://www.last.fm/music/The%2bReplacements/_/Left%2bof%2bthe%2bDial"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Lou Reed",
				Title = "Wagon Wheel",
				PlayLink = "http://www.last.fm/music/Lou%2bReed/_/Wagon%2bWheel"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "Nick Lowe",
				Title = "Marie Provost",
				PlayLink = "http://www.last.fm/music/Nick%2bLowe/_/Marie%2bProvost"
			});
			m1.RSoundTrack.Add(new Rtone.Song
			{
				Artist = "The Velvet Underground",
				Title = "I Heard Her Call My Name",
				PlayLink = "http://www.last.fm/music/The%2bVelvet%2bUnderground/_/I%2bHeard%2bHer%2bCall%2bMy%2bName"
			});

			return m1;

		}


		private Rtone.Movie PullingJohn()
		{

			var m1 = new Rtone.Movie();
			m1.ImgLink = "http://cdn-1.nflximg.com/en_us/boxshots/large/70114963.jpg";
			m1.Title = "Pulling John";
			m1.Year = 2009;
			m1.OSoundTrack = new List<Rtone.Song>();
			m1.RSoundTrack = new List<Rtone.Song>();
			m1.OSoundTrack.Add(new Rtone.Song { Artist = "Boris Alexandrov", Title = "Farewell of Slavianka", PlayLink = "" });
			m1.OSoundTrack.Add(new Rtone.Song { Artist = "Ruslana", Title = "Kish Kish Kish", PlayLink = "" });
			m1.OSoundTrack.Add(new Rtone.Song { Artist = "Piotr Bulakhov and Vasily Chuyevsky", Title = "Gori, Gori Moya Zvezda", PlayLink = "" });
			m1.RSoundTrack.Add(new Rtone.Song { Artist = "Boris Alexandrov", Title = "Song of the Plains", PlayLink = "http://www.last.fm/music/Boris%2bAlexandrov/_/Song%2bof%2bthe%2bPlains" });
			m1.RSoundTrack.Add(new Rtone.Song { Artist = "Ruslana", Title = "Wild Dances", PlayLink = "http://www.last.fm/music/Ruslana/_/Wild%2bDances" });

			return m1;
		}
	}
}

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Purchasing;

namespace HEXPLAY
{
	public partial class Game
	{
		public static ushort gameId = 1;
		public static uint gameVersion = 1;

		public static Entity2D player;

		public static bool isPlayLevel;

		public static Entity2D slotmachine;

		public static Particles particlesSnow;
		public static Particles particlesCoins;
		public static Particles particlesLight;

		public static int maxHitCount;

		public static Settings settings;

		public static List<HighScores> highScores;
		public static HighScores newHighScores;
		public static List<HeroSet> heroSet;

		public static string localNotificationsTitle = "AVALANCHE";
		public static string localNotificationsText = "Get 100 Coins for free!";

		public static string appInviteShareURL = "http://hexplay.com/games/avalanche/";

		public static string appInviteTitle = "AVALANCHE The Video Game";
		public static string appInviteMessage = "Hey! Check this out. It is a fun retro arcade video game: https://goo.gl/fGGqSY";
		public static string appInviteDeepLink = "https://play.google.com/store/apps/details?id=com.herocraft.game.free.avalanche";
		public static string appInviteCallToActionText = "Hey! Check this out. It is a fun retro arcade video game: https://goo.gl/fGGqSY";

		public static string rateUsTitle = "Like this game?";
		public static string rateUsMessage = "Please rate to support future updates!";

		public static string androidAppUrl = "market://details?id=com.herocraft.game.free.avalanche";
		public static string appleID  = "1205411271";

		public static string appleIDRateUs = "itms-apps://itunes.apple.com/id1205411271?mt=8";

#if UNITY_ANDROID
		public const string leaderBoardId = "CgkI--_YspETEAIQAQ";

		public const string achievement1000PointsID = "CgkI--_YspETEAIQAg";
		public const string achievement2000PointsID = "CgkI--_YspETEAIQAw";
		public const string achievement5000PointsID = "CgkI--_YspETEAIQBA";
		public const string achievement10000PointsID = "CgkI--_YspETEAIQBQ";
		public const string achievement50000PointsID = "CgkI--_YspETEAIQBg";

		public const string unityAdsGameId = "1239630";

		public static string fyberAppId = "84518";
		public static string fyberSecurityToken = "d9cc68efa201310be25622656fbf0d4f";
#endif

#if UNITY_IPHONE
		public const string leaderBoardId = "avalache_high_scores";

		public const string achievement1000PointsID = "avalanche_achievement_high_score_1000";
		public const string achievement2000PointsID = "avalanche_achievement_high_score_2000";
		public const string achievement5000PointsID = "avalanche_achievement_high_score_5000";
		public const string achievement10000PointsID = "avalanche_achievement_high_score_10000";
		public const string achievement50000PointsID = "avalanche_achievement_high_score_50000";

		public const string unityAdsGameId = "1307607";

		public static string fyberAppId = "91917";
		public static string fyberSecurityToken = "cee5bedf836424afc5fbf0e516fc895c";
#endif

#if UNITY_WEBGL || UNITY_STANDALONE
		public const string leaderBoardId = "";
		public const string unityAdsGameId = ""; 

		public const string achievement1000PointsID = "";
		public const string achievement2000PointsID = "";
		public const string achievement5000PointsID = "";
		public const string achievement10000PointsID = "";
		public const string achievement50000PointsID = "";
#endif

		public static Dictionary<string, ProductType> productCollection = new Dictionary<string, ProductType>()
		{
			{ "com.herocraft.game.free.avalance.hero.2", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.3", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.4", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.5", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.6", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.7", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.8", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.9", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.10", ProductType.NonConsumable },

			{ "com.herocraft.game.free.avalance.hero.11", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.12", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.13", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.14", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.15", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.16", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.17", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.18", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.19", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.20", ProductType.NonConsumable },

			{ "com.herocraft.game.free.avalance.hero.21", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.22", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.23", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.24", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.25", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.26", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.27", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.28", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.29", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.30", ProductType.NonConsumable },

			{ "com.herocraft.game.free.avalance.hero.31", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.32", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.33", ProductType.NonConsumable },
			{ "com.herocraft.game.free.avalance.hero.34", ProductType.NonConsumable },
		};

	}
}
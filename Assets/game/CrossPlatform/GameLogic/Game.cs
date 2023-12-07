using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public partial class Game
	{
		public static void CreateGame()
		{
			Render.SetQualitySettings();

			CreateGUIStyles();

			highScores = new List<HighScores>();
			HighScores.Load("/local", highScores);

			HeroSet.CreateHeroSet();

			CreateWorld();

			gamePDA.Push(new GameStateLoading());
		}

		public static void CreateWorld()
		{
			World2D.CreateSpace2D();
		}

		public static void Rest()
		{
			RestTime();
			iteration = 0;

			LevelGenerator.Rest();
			World2D.Rest();
			Render.Rest();
			Sound.Rest();
			GUI.Rest();

			random.Rest();
		}

		public static void PreSetup()
		{
			settings = new Settings();
			LoadSettings();

			settings.gameRunCount++;

			collection = new Collection();

			text = new Dictionary<TEXT, string>();
			Utils.LoadText("en");
			Utils.SetLanguage();

			//RuntimePermissions.Setup();
		}

		public static void Setup()
		{
			//Platform.Setup();
			GUI.Setup();
			Sound.Setup();
			Render.Setup();
			World2D.Setup();
			LevelGenerator.Setup();

			inputController = new InputController();

			random = new PseudoRandom();
			random.Rest();

			gamePDA = new GamePDA();

			savedGameActions = new MemoryBuffer(256);

			SaveSettings();

			RestTime();
		}
	}
}
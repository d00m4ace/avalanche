using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class HeroSet
	{
		public int heroID;
		public HEXInt heroTokens;
		public HEXInt heroTokensTotal;

		public bool heroReceived;

		public void Write(MemoryBuffer mb)
		{
			mb.Write(heroID);
			mb.Write(heroTokens);
			mb.Write(heroTokensTotal);
		}

		public bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out heroID))
				return false;

			if(!mb.Read(out heroTokens))
				return false;

			if(!mb.Read(out heroTokensTotal))
				return false;

			return true;
		}

		static readonly byte[] enigmaKey = "Fgmb4323gkcvsx1z75vHfg".ToUTF8Bytes();

		public static void Save(List<HeroSet> heroSet)
		{
			using(MemoryBuffer mb = new MemoryBuffer(1024))
			{
				mb.Write(heroSet, (b, x) => x.Write(b));
				Utils.FileWrite("", "HeroSet", Enigma.CodeMd5(mb.buffer, enigmaKey));
			}
		}

		public static void Load(List<HeroSet> heroSet)
		{
			byte[] buffer;
			if(!Utils.FileRead("", "HeroSet", out buffer))
				return;

			buffer = Enigma.DecodeMd5(buffer, enigmaKey);
			if(buffer == null)
				return;

			using(MemoryBuffer mb = new MemoryBuffer(buffer))
			{
				mb.Read(heroSet, (b) => { HeroSet hs = new HeroSet(); if(hs.Read(b)) return hs; return null; });
			}

			buffer = null;
		}

		public static void LoadAsset(List<HeroSet> heroSet)
		{
			byte[] buffer;
			if(!Utils.LoadAsset("data", "HeroSet", out buffer))
				return;

			buffer = Enigma.DecodeMd5(buffer, enigmaKey);
			if(buffer == null)
				return;

			using(MemoryBuffer mb = new MemoryBuffer(buffer))
			{
				mb.Read(heroSet, (b) => { HeroSet hs = new HeroSet(); if(hs.Read(b)) return hs; return null; });
			}

			buffer = null;
		}

		public static void AddGameStateGotNewHero(PushdownAutomata pda)
		{
			for(int i = 1; i < 35; i++)
			{
				if(Game.heroSet[i - 1].heroReceived)
				{
					Game.heroSet[i - 1].heroReceived = false;
					pda.Push(new PushdownAutomata.TransitionState(new GameStateGotNewHero(i - 1)));
				}
			}
		}

		public static void CreateHeroSet()
		{
			Game.heroSet = new List<HeroSet>();
			Load(Game.heroSet);

			List<HeroSet> heroSetAsset = new List<HeroSet>();
			LoadAsset(heroSetAsset);

			for(int i = 1; i < 35; i++)
			{
				if(i > Game.heroSet.Count)
				{
					HeroSet hs = new HeroSet();
					hs.heroID = i;
					hs.heroTokens = heroSetAsset[i - 1].heroTokens;
					hs.heroTokensTotal = heroSetAsset[i - 1].heroTokensTotal;
					Game.heroSet.Add(hs);
				}
			}

			/*
						int[] tokens = { 0,
										1, 500, 200, 250, 300, 50, 75, 100, 150, 250,
										300, 250, 200, 200, 500, 300, 250, 200, 300, 200,
										350, 400, 600, 1000, 250, 250, 250, 500, 200, 250,
										1000, 250, 500, 300, 5, 6, 7, 8, 9, 10,
										1, 2, 3, 4, 5, 6, 7, 8, 9, 10, };

						for(int i = 1; i < 35; i++)
						{
							if(i > Game.heroSet.Count)
							{
								HeroSet hs = new HeroSet();
								hs.heroID = i;
								hs.heroTokens = 0;
								hs.heroTokensTotal = tokens[i];
								if(i == 1) hs.heroTokens = tokens[i];
									Game.heroSet.Add(hs);
							}
						}
			*/

			Save(Game.heroSet);
		}
	}
}
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class MainMusic : Music
	{
		public override void GoToNextSet()
		{
			if(Game.player.vel.Length > 10) setPlaying++;
			else setPlaying--;

			if(setPlaying < 0) setPlaying = 0;
			if(setPlaying == sets.Length) setPlaying = 0;
		}

		public MainMusic()
		{
			sets = new Set[]
			{
				//new Set { rate = 1f / 8, page = 0, toneOffset = 0},
				//new Set { rate = 1f / 8, page = 0, toneOffset = 1},

				//new Set { rate = 1f / 8, page = 1, toneOffset = 0},
				//new Set { rate = 1f / 8, page = 1, toneOffset = 1},

				//new Set { rate = 1f / 8, page = 2, toneOffset = 0},
				//new Set { rate = 1f / 8, page = 2, toneOffset = 1},

				//new Set { rate = 1f / 8, page = 3, toneOffset = 0},
				//new Set { rate = 1f / 8, page = 3, toneOffset = 1},

				//new Set { rate = 1f / 8, page = 4, toneOffset = 0},
				//new Set { rate = 1f / 8, page = 4, toneOffset = 1},

				new Set { rate = 1f / 8, page = 0, toneOffset = 0, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},
				new Set { rate = 1f / 8, page = 0, toneOffset = 1, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},

				new Set { rate = 1f / 8, page = 1, toneOffset = 0, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},
				new Set { rate = 1f / 8, page = 1, toneOffset = 1, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},

				new Set { rate = 1f / 8, page = 2, toneOffset = 0, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},
				new Set { rate = 1f / 8, page = 2, toneOffset = 1, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},

				new Set { rate = 1f / 8, page = 3, toneOffset = 0, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},
				new Set { rate = 1f / 8, page = 3, toneOffset = 1, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},

				new Set { rate = 1f / 8, page = 4, toneOffset = 0, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},
				new Set { rate = 1f / 8, page = 4, toneOffset = 1, instruments = new int[] { 10, 2, 3, 4, 5, 6, 7, 8, 5, 7, 11, 12, }},
			};

			pages = new Page[]
			{
				new Page { notes = new int[][]
				{
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
				}},

				new Page { notes = new int[][]
				{
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
				}},

				new Page { notes = new int[][]
				{
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
				}},

				new Page { notes = new int[][]
				{
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
				}},

				new Page { notes = new int[][]
				{
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x8A, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x8A, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x8A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x9A, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, },
new int[] { 0xAA, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
new int[] { 0x00, 0x00, 0x00, 0xAA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9A, 0x00, 0x00, },
				}},
			};
		}
	}
}
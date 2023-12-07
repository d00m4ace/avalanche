using System;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class Music
	{
		public Fixed[] tones = new Fixed[] { 1, (Fixed)1.059463f, (Fixed)1.122462f, (Fixed)1.189207f, (Fixed)1.259921f, (Fixed)1.334840f, (Fixed)1.414214f, (Fixed)1.498307f, (Fixed)1.587401f, (Fixed)1.681793f, (Fixed)1.781797f, (Fixed)1.887749f, 2, (Fixed)2.122462f, (Fixed)2.189207f, };
		public string[] instruments = new string[] { "gba/gba01", "gba/gba02", "gba/gba03", "gba/gba04", "gba/gba05", "gba/gba06", "gba/gba10", "gba/gba08", "gba/gba09", "gba/gba16", "gba/gba11", "gba/gba12", };

		public float trackPlayingTime = 0;

		public int setPlaying = 0;
		public int trackPlaying = 0;

		public bool isPlaying = false;
		public bool isLoop = false;

		public Fixed volume = Fixed.OneHalf;

		public class Page
		{
			public int[][] notes;
		}

		public Page[] pages;

		public class Set
		{
			public float rate;
			public int page;
			public int toneOffset = 0;
			public int[] instruments = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, };
		}

		public Set[] sets;

		public void PlayTrack(int set, int track)
		{
			int[][] notes = pages[sets[set].page].notes;

			for(int i = 0; i < notes[track].Length; i++)
			{
				if(sets[set].instruments[i] == 0)
					continue;

				int t = (notes[track][i] >> 4) + sets[set].toneOffset;
				if(t < 1) t = 1; if(t > 15) t = 15;

				int v = notes[track][i] & 0xF;

				//if(notes[track][i] > 0)
					//Sound.PlaySound(instruments[sets[set].instruments[i] - 1], v * volume / 10, tones[t - 1]);
			}
		}

		public virtual void OnUpdate()
		{
#if !SERVER
			if(!Game.settings.musicOn)
				return;

			if(isPlaying)
			{
				trackPlayingTime += UnityEngine.Time.deltaTime;

				if(trackPlayingTime >= sets[setPlaying].rate)
				{
					if(trackPlaying == pages[sets[setPlaying].page].notes.Length)
					{
						trackPlaying = 0;
						GoToNextSet();

						if(setPlaying == sets.Length)
						{
							setPlaying = 0;
							isPlaying = isLoop;
						}
					}

					if(isPlaying)
					{
						PlayTrack(setPlaying, trackPlaying);

						trackPlayingTime = 0;
						trackPlaying++;
					}
				}
			}
#endif
		}

		public virtual void GoToNextSet()
		{
			setPlaying++;
		}

		public void Play()
		{
			trackPlayingTime = 0;
			trackPlaying = 0;
			setPlaying = 0;
			isPlaying = true;
		}
	}
}
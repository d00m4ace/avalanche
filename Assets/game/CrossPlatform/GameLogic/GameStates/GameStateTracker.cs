using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateTracker : GameState
	{
		GUIWindow window;

		GUILabel[] tracks;

		GUIToggleGroup noteToggle;
		GUIToggleGroup volumeToggle;

		GUIButton lastSoundButton;

		GUIAnimation soundButtonBlinkAnimation = new GUIAnimationBlinkBackColor(0.65f, Color.Red, false, true);
		GUIAnimation soundButtonIdleAnimation;

		Fixed[] tones = new Fixed[] { 1, (Fixed)1.059463f, (Fixed)1.122462f, (Fixed)1.189207f, (Fixed)1.259921f, (Fixed)1.334840f, (Fixed)1.414214f, (Fixed)1.498307f, (Fixed)1.587401f, (Fixed)1.681793f, (Fixed)1.781797f, (Fixed)1.887749f, 2, (Fixed)2.122462f, (Fixed)2.189207f, };
		public string[] instruments = new string[] { "gba/gba01", "gba/gba02", "gba/gba03", "gba/gba04", "gba/gba05", "gba/gba06", "gba/gba10", "gba/gba08", "gba/gba09", "gba/gba14", "gba/gba11", "gba/gba12", };

		float trackRate;
		float trackPlayingTime;
		int trackPlaying;
		bool isPlaying;
		bool isLoop;

		int[][] notes;

		public override void OnEnter(PushdownAutomata pda)
		{
			GUI.playSound = false;
			GUI.SetScale(2);

			window = new GUIWindow(style: Game.GUIStyle.Block);
			window.SetAlign(GUIWindow.Align.MiddleCenter);
			window.focusWindow = true;
			window.SetContentLayout(GUIWindow.ContentLayout.Vertical);
			window.backColor.a = 128;
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.SetShadowBackTexture("gui/images/bar");

			window.animation = new GUIAnimationSize(0.35f, int.MinValue, 0);

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.contentEqualWidth = w.contentEqualHeight = true;
				w.SetContentLayout(GUIWindow.ContentLayout.Horizontal);

				tracks = new GUILabel[1 + 32];

				notes = new int[32][];

				for(int t = 0; t < tracks.Length; t++)
				{
					GUIElement[] elements = new GUIElement[13];

					GUIButton button;
					elements[0] = button = new GUIButton(Game.ButtonID.Sound, new GUIText("▶"), id: t * 256);
					if(t == 0) button.backColor = Color.LightGreen;

					if(t > 0)
						notes[t - 1] = new int[12];

					for(int i = 1; i < elements.Length; i++)
					{
						int s = i; if(t > 0) s = 0;
						elements[i] = button = new GUIButton(Game.ButtonID.Sound, new GUIText(s.ToString("X2")), id: t * 256 + i);
						if(t == 0) button.backColor = Color.LightGreen;

						if(t > 0)
							notes[t - 1][i - 1] = 0x00;
					}

					w.Add(tracks[t] = new GUILabel(null, Game.GUIStyle.Block, contentLayout: GUILabel.ContentLayout.Vertical));
					tracks[t].elements = new List<GUIElement>(elements);
					tracks[t].backColor = GetTrackBackColor(t);
				}

				window.Add(w);
			}

			{
				GUIElement[] elements = new GUIElement[15];
				for(int i = 1; i <= elements.Length; i++)
					elements[i - 1] = new GUIToggle(Game.ButtonID.Note, new GUIElement[] { new GUIText((i << 4).ToString("X2")), new GUIText((i << 4).ToString("X2")) }, new Color[] { Color.LightBlue, Color.LightSkyBlue }, id: i);
				window.Add(new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.Note), noteToggle = new GUIToggleGroup(elements, contentLayout: GUILabel.ContentLayout.Horizontal) }));
			}

			{
				GUIElement[] elements = new GUIElement[10];
				for(int i = 1; i <= elements.Length; i++)
					elements[i - 1] = new GUIToggle(Game.ButtonID.Volume, new GUIElement[] { new GUIText(i.ToString("X2")), new GUIText(i.ToString("X2")) }, new Color[] { Color.LightBlue, Color.LightSkyBlue }, id: i);
				window.Add(new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.Volume), volumeToggle = new GUIToggleGroup(elements, contentLayout: GUILabel.ContentLayout.Horizontal) }));
				volumeToggle.SetToggle(9);
			}

			SetTracks();

			trackRate = 1f / 8;
			trackPlayingTime = 0;
			trackPlaying = 1;
			isPlaying = false;
			isLoop = false;

			GUI.Add(window);
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);

			GUI.playSound = true;
			GUI.SetOptimalScale();
		}

		Color GetTrackBackColor(int track)
		{
			Color[] color = new Color[] { Color.LightBlue, Color.RoyalBlue };

			return color[((track - 1) / 4) % 2];
		}

		public void PlayTrack(int track)
		{
			for(int t = 0; t < tracks.Length; t++)
				tracks[t].backColor = GetTrackBackColor(t);

			tracks[track].backColor = Color.LimeGreen;

			for(int s = 0; s < notes[track - 1].Length; s++)
			{
				int t = notes[track - 1][s] >> 4;
				int v = notes[track - 1][s] & 0xF;

				//if(notes[track - 1][s] > 0)
					//Sound.Play(instruments[s], (Fixed)v / 10, tones[t - 1]);
			}
		}

		public void SaveTracks()
		{
			string text = "";

			for(int track = 0; track < notes.Length; track++)
			{
				text += "new int[] { ";
				for(int s = 0; s < notes[track].Length; s++)
				{
					text += "0x" + notes[track][s].ToString("X2") + ", ";
				}
				text += "}, \n";
			}

			File.WriteAllText(UnityEngine.Application.persistentDataPath + "/notes", text);
		}

		public void SetTracks()
		{
			notes = new int[][]
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
			};

			for(int t = 1; t < tracks.Length; t++)
			{
				for(int i = 1; i < tracks[t].elements.Count; i++)
				{
					GUIButton button = (GUIButton)tracks[t].elements[i];

					GUIText guiText = (GUIText)button.element;

					if(notes[t - 1][i - 1] != 0)
					{
						//notes[t - 1][i - 1] = (((notes[t - 1][i - 1] >> 4) + 1 ) << 4) + (notes[t - 1][i - 1] & 0xF);

						guiText.SetText((notes[t - 1][i - 1] >> 4).ToString("X") + (notes[t - 1][i - 1] & 0xF).ToString("X"));
						button.RestSize();
						button.backColor = Color.LightGreen;
					}
					else
					{
						guiText.SetText("00");
						button.RestSize();
						button.backColor = Color.LightBlue;
					}
				}
			}
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(isPlaying)
			{
				trackPlayingTime += UnityEngine.Time.deltaTime;

				if(trackPlayingTime >= trackRate)
				{
					if(trackPlaying == tracks.Length)
					{
						tracks[tracks.Length - 1].backColor = GetTrackBackColor(tracks.Length - 1);
						trackPlayingTime = 0;
						trackPlaying = 1;
						isPlaying = isLoop;
					}

					if(isPlaying)
					{
						PlayTrack(trackPlaying);
						trackPlayingTime = 0;
						trackPlaying++;
					}
				}
			}

			if(GUI.buttonPushed != null)
			{
				switch(GUI.buttonPushed.buttonID)
				{
					case Game.ButtonID.Sound:
					{
						if(lastSoundButton != null) lastSoundButton.animation = soundButtonIdleAnimation;
						lastSoundButton = GUI.buttonPushed;
						soundButtonIdleAnimation = lastSoundButton.animation; lastSoundButton.animation = soundButtonBlinkAnimation;

						int t = lastSoundButton.id >> 8;
						int i = lastSoundButton.id & 0xF;

						if(t > 0 && i > 0)
						{
							GUIText guiText = (GUIText)lastSoundButton.element;

							if(guiText.text == "00")
							{
								guiText.SetText((noteToggle.toggle + 1).ToString("X") + (volumeToggle.toggle + 1).ToString("X"));
								lastSoundButton.RestSize();
								lastSoundButton.backColor = Color.LightGreen;

								notes[t - 1][i - 1] = ((noteToggle.toggle + 1) << 4) + (volumeToggle.toggle + 1);
							}
							else
							{
								guiText.SetText("00");
								lastSoundButton.RestSize();
								lastSoundButton.backColor = Color.LightBlue;

								notes[t - 1][i - 1] = 0x00;
							}
						}

						//if(i > 0)
							//Sound.Play(instruments[i - 1], (Fixed)(volumeToggle.toggle + 1) / 10, tones[noteToggle.toggle]);

						if(i == 0)
						{
							trackPlayingTime = 0;
							trackPlaying = t == 0 ? 1 : t;
							isPlaying = true;
							isLoop = t == 0;

							if(t == 0)
								SaveTracks();
						}
					}
					break;

					case Game.ButtonID.Note:
					case Game.ButtonID.Volume:
					if(lastSoundButton != null)
					{
						int t = lastSoundButton.id >> 8;
						int i = lastSoundButton.id & 0xF;

						if(t > 0 && i > 0)
						{
							GUIText guiText = (GUIText)lastSoundButton.element;
							guiText.SetText((noteToggle.toggle + 1).ToString("X") + (volumeToggle.toggle + 1).ToString("X"));
							lastSoundButton.RestSize();
							lastSoundButton.backColor = Color.LightGreen;

							notes[t - 1][i - 1] = ((noteToggle.toggle + 1) << 4) + (volumeToggle.toggle + 1);
						}

						//if(i > 0)
						//	Sound.Play(instruments[i - 1], (Fixed)(volumeToggle.toggle + 1) / 10, tones[noteToggle.toggle]);
					}
					break;
				}
			}
		}
	}
}
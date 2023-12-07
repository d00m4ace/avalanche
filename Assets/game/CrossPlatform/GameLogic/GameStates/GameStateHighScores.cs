using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateHighScores : GameState
	{
		GUIWindow window;

		static readonly string[] placeName = { "1 ", "2 ", "3 ", "4 ", "5 ", "6 ", "7 ", "8 ", "9 ", "10" };

		public override void OnEnter(PushdownAutomata pda)
		{
			window = new GUIWindow(style: Game.GUIStyle.Empty);
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.SetHeightLayout(GUIWindow.HeightLayout.PercentHeight, 100);
			window.SetAlign(GUIWindow.Align.BottomCenter);
			window.focusWindow = true;

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Block);
				w.SetAlign(GUIWindow.Align.MiddleCenter);
				w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.backColor.a = 128;
				w.spacing = 1;
				w.SetShadowBackTexture("gui/images/bar");
				w.paddingHeight = 6;
				w.yOffset = -24;
				w.noClip = false;
				w.animation = new GUIAnimationSize(0.35f, int.MinValue, 0);
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.HighScores, Game.GUIStyle.MediumFont) }));

				int ci = Game.highScores.Count < 10 ? Game.highScores.Count : 10;
				for(int i = 0; i < ci; i++)
				{
					GUIAnimation animation = Game.highScores[i] == Game.newHighScores ? new GUIAnimationBlinkBaseColor(0.5f, Color.Yellow, false, true) : new GUIAnimation();
					int minHeight = i < ci - 1 ? 24 : 25;
					w.Add(element = new GUILabel(new GUIElement[] {
						new GUIText(placeName[i], Game.GUIStyle.MediumFont, animation:animation, width: 8 * 2 * 2),
						new GUIImage("gui/images/coins/coin" + Game.highScores[i].heroID.ToString("000")),
						new GUIText(Game.highScores[i].GetNameWithDots(), Game.GUIStyle.MediumFont, animation:animation, width: 8 * 2 * 8),
						new GUIText(Game.highScores[i].socre.ToString("000000"), Game.GUIStyle.MediumFont, animation:animation, width: 8 * 2 * 6),
						new GUIButton(Game.ButtonID.Replay, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/camera") }), id:i)
					}, minHeight: minHeight, spacing: 3));
				}
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.BottomCenter);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.spacing = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;

#if UNITY_IPHONE
				if(Social.IsSignIn())
				{
					w.Add(element = new GUIButton(Game.ButtonID.World, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/graph"), new GUIText(Game.TEXT.World) }), Game.GUIStyle.Button));
					//w.Add(element = new GUIButton(Game.ButtonID.Achievements, new GUIImage("gui/images/icons/flag_alt", imageColor: Color.White), Game.GUIStyle.BoxButton));
				}
#endif

#if UNITY_ANDROID
				if(Social.IsSignIn())
				{
					w.Add(element = new GUIButton(Game.ButtonID.World, new GUIImage("gui/images/icons/google_play", imageColor: Color.Green), Game.GUIStyle.BoxButton));
					w.Add(element = new GUIButton(Game.ButtonID.Achievements, new GUIImage("gui/images/icons/google_play_achievements", imageColor: Color.Green), Game.GUIStyle.BoxButton));
				}
#endif

				w.Add(element = new GUIButton(Game.ButtonID.Continue, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/play"), new GUIText(Game.TEXT.Continue) }), Game.GUIStyle.Button, defaultFocus: true));
				w.Add(element = new GUIButton(Game.ButtonID.Video, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/video_share"), new GUIText(Game.TEXT.Video) }), Game.GUIStyle.Button));
			}

			GUI.Add(window);
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(window.animation.IsPlaying())
				return;

			if(GUI.buttonPushed != null)
			{
				switch(GUI.buttonPushed.buttonID)
				{
					case Game.ButtonID.Replay:
					if(HighScores.LoadReplay("/local", Game.highScores[GUI.buttonPushed.id].replayFileName, Game.savedGameActions))
					{
						Game.newHighScores = null;
						Game.isReplay = true;
						pda.Pop(this);
						pda.Push(new PushdownAutomata.TransitionState(new GameStateHighScores()));
						pda.Push(new PushdownAutomata.TransitionState(new GameStatePlayLevel()));
						pda.Push(new GameStateRestartLevel());
					}
					return;

					case Game.ButtonID.Continue:
					pda.Pop(this);
					return;

					case Game.ButtonID.World:
					//Social.ShowLeaderBoard(Game.leaderBoardId);
					return;

					case Game.ButtonID.Achievements:
					//Social.ShowAchievements();
					return;

					case Game.ButtonID.Video:
					//VideoRecording.ShowVideoReplays();
					return;
				}
			}
		}
	}
}
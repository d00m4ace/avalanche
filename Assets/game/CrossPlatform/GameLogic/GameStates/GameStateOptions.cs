using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateOptions : GameState
	{
		GUIWindow window;

		Game.ButtonID buttonPushed;
		bool isClosed = false;

		public override void OnEnter(PushdownAutomata pda)
		{
			window = new GUIWindow();
			window.SetWidthLayout(GUIWindow.WidthLayout.ContentWidth);
			window.SetHeightLayout(GUIWindow.HeightLayout.ContentHeight);
			window.SetAlign(GUIWindow.Align.MiddleCenter);
			window.focusWindow = true;
			window.SetContentLayout(GUIWindow.ContentLayout.Vertical);
			window.paddingWidth = 10;
			window.paddingHeight = 10;
			window.spacing = 8;
			window.shadowBackTextureColor = Color.Dark * Color.WhiteAlpha50;
			window.animation = new GUIAnimationXY(0.35f, int.MinValue, -GUI.GetScreenHeight());

			{
				GUIElement element;
				window.Add(element = new GUIText(Game.TEXT.Options, Game.GUIStyle.MediumFont));

				{
					GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
					w.contentEqualWidth = w.contentEqualHeight = true;
					w.SetContentLayout(GUIWindow.ContentLayout.Vertical);

					w.Add(element = new GUIButton(Game.ButtonID.HighScores, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/graph"), new GUIText(Game.TEXT.HighScores) }), Game.GUIStyle.Button));
					w.Add(element = new GUIButton(Game.ButtonID.Slotmachine, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/slotmachine"), new GUIText(Game.TEXT.Slotmachine) }), Game.GUIStyle.Button));
					w.Add(element = new GUIButton(Game.ButtonID.Settings, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/settings_clear"), new GUIText(Game.TEXT.Settings) }), Game.GUIStyle.Button));
					w.Add(element = new GUIButton(Game.ButtonID.MoreGames, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/gamepad"), new GUIText(Game.TEXT.MoreGames) }), Game.GUIStyle.Button));
					w.Add(element = new GUIButton(Game.ButtonID.Credits, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/info"), new GUIText(Game.TEXT.Credits) }), Game.GUIStyle.Button));
					w.Add(element = new GUIButton(Game.ButtonID.Back, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/back"), new GUIText(Game.TEXT.Back) }), Game.GUIStyle.Button, defaultFocus: true));

					window.Add(w);
				}
			}

			GUI.Add(window);

			Sound.StopSounds();
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(window.animation.IsPlaying())
				return;

			if(isClosed)
			{
				isClosed = false;

				switch(buttonPushed)
				{
					case Game.ButtonID.Back:
					pda.Pop(this);
					break;

					case Game.ButtonID.HighScores:
					pda.Pop(this);
					pda.Pop(pda.GetState<GameStateMainMenu>());
					pda.Push(new PushdownAutomata.TransitionState(new GameStateMainMenu()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateRestartLevel()));
					pda.Push(new GameStateHighScores());
					break;

					case Game.ButtonID.Slotmachine:
					pda.Pop(this);
					pda.Pop(pda.GetState<GameStateMainMenu>());
					pda.Push(new PushdownAutomata.TransitionState(new GameStateMainMenu()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateRestartLevel()));
					pda.Push(new GameStateSlotmachine());
					break;

					case Game.ButtonID.Settings:
					pda.Push(new GameStateSettings());
					break;

					case Game.ButtonID.Credits:
					pda.Push(new GameStateCredits());
					break;

					case Game.ButtonID.MoreGames:
#if UNITY_ANDROID
					UnityEngine.Application.OpenURL("market://search?q=herocraft ltd");
#elif UNITY_IPHONE || UNITY_PLAYER
					UnityEngine.Application.OpenURL("itms-apps://itunes.apple.com/developer/herocraft-ltd/id482544510");
#endif
					break;
				}

				return;
			}

			if(GUI.IsAndroidBackButtonPushed())
			{
				window.animation.PlayInverse();
				buttonPushed = Game.ButtonID.Back;
				isClosed = true;
				return;
			}

			if(GUI.buttonPushed != null)
			{
				if(GUI.buttonPushed.buttonID == Game.ButtonID.Back ||
				   GUI.buttonPushed.buttonID == Game.ButtonID.HighScores ||
				   GUI.buttonPushed.buttonID == Game.ButtonID.Slotmachine)
					window.animation.PlayInverse();

				buttonPushed = GUI.buttonPushed.buttonID;

				isClosed = true;
			}
		}
	}
}
using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStatePause : GameState
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
				window.Add(element = new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.Pause) }, Game.GUIStyle.MediumFont));

				{
					GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
					w.contentEqualWidth = w.contentEqualHeight = true;
					w.SetContentLayout(GUIWindow.ContentLayout.Vertical);

					w.Add(element = new GUIButton(Game.ButtonID.Settings, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/settings_clear"), new GUIText(Game.TEXT.Settings) }), Game.GUIStyle.Button));
					w.Add(element = new GUIButton(Game.ButtonID.Exit, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/home_clear"), new GUIText(Game.TEXT.Exit) }), Game.GUIStyle.Button));

					w.Add(element = new GUIButton(Game.ButtonID.Back, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/back"), new GUIText(Game.TEXT.Back) }), Game.GUIStyle.Button, defaultFocus: true));

					window.Add(w);
				}
			}

			GUI.Add(window);

			Sound.Play( Game.CollectionID.sound_gba_gba12);
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
					if(!Game.isReplay)
						pda.Push(new GameStateResume());
					break;

					case Game.ButtonID.Settings:
					pda.Push(new GameStateSettings());
					break;

					case Game.ButtonID.Exit:
					pda.Pop(this);
					pda.Pop(pda.GetState<GameStatePlayLevel>());
					if(Game.newHighScores != null)
						pda.Push(new GameStateEnterName());
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
				   GUI.buttonPushed.buttonID == Game.ButtonID.Exit)
					window.animation.PlayInverse();

				buttonPushed = GUI.buttonPushed.buttonID;

				isClosed = true;
			}
		}
	}
}
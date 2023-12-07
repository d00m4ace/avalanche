using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateExit : GameState
	{
		GUIWindow window;

		Game.ButtonID buttonPushed;
		bool isClosed = false;

		public override void OnEnter(PushdownAutomata pda)
		{
			window = new GUIWindow(style: Game.GUIStyle.Block);
			window.SetAlign(GUIWindow.Align.MiddleCenter);
			window.focusWindow = true;
			window.SetContentLayout(GUIWindow.ContentLayout.Vertical);
			window.backColor.a = 200;
			window.spacing = 8;
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.paddingHeight = window.paddingWidth = 8;
			window.minHeight = (int)(25 * 1.5f) + window.paddingHeight + window.paddingWidth;
			window.shadowBackTextureColor = Color.Dark * Color.WhiteAlpha50;
			window.shadowBackTexture = Render.GetTexture("gui/images/bar");

			window.animation = new GUIAnimationFilter(new GUIAnimation[] { new GUIAnimationSize(0.35f, int.MinValue, 0), new GUIAnimationElementsWait(window) });

			GUIElement element;

			window.Add(element = new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.ExitGame, percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter) }, Game.GUIStyle.MediumFont));

			window.Add(element = new GUIButton(Game.ButtonID.Exit, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/home_clear"), new GUIText(Game.TEXT.Exit) }), Game.GUIStyle.Button));

			window.Add(element = new GUIButton(Game.ButtonID.Back, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/back"), new GUIText(Game.TEXT.Back) }), Game.GUIStyle.Button, defaultFocus: true));

			GUI.Add(window);

			Sound.Play(Game.CollectionID.sound_gba_gba12);
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

					case Game.ButtonID.Exit:
					//Platform.Quit();
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
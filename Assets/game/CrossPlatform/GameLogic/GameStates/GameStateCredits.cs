using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateCredits : GameState
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
				window.Add(element = new GUIText(Game.TEXT.Credits, Game.GUIStyle.MediumFont));

				{
					GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
					w.contentEqualWidth = w.contentEqualHeight = true;
					w.SetContentLayout(GUIWindow.ContentLayout.Vertical);

					w.Add(new GUIText(Game.TEXT.CreditsText, width: 280, height: 140, animation: new GUIAnimationText(3), defaultFontSize: 8));

					window.Add(w);
				}

				window.Add(element = new GUIButton(Game.ButtonID.Back, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/back"), new GUIText(Game.TEXT.Back) }), Game.GUIStyle.Button, defaultFocus: true));
				window.Add(element = new GUIButton(Game.ButtonID.Support, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/mail_dark"), new GUIText(Game.TEXT.Support) }), Game.GUIStyle.Button));
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

			if(GUI.isAnyKeyDown)
				GUI.EndAnyAnimations();

			if(isClosed)
			{
				switch(buttonPushed)
				{
					case Game.ButtonID.Back:
					pda.Pop(this);
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
				if(GUI.buttonPushed.buttonID == Game.ButtonID.Support)
				{
#if STANSASSETS && (UNITY_IPHONE || UNITY_ANDROID)
					UM_ShareUtility.SendMail("AVALANCHE The Video Game", (startup.GetSystemInfo() + "\n\nYour text goes here:\n\n ").Replace("\n", "<br>"), "support@herocraft.com");
#endif
					return;
				}

				window.animation.PlayInverse();
				buttonPushed = GUI.buttonPushed.buttonID;
				isClosed = true;
			}
		}
	}
}
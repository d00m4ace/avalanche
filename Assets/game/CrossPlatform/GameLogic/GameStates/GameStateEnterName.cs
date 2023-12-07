using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateEnterName : GameState
	{
		GUIWindow window;
		GUIKeyboard guiKeyboard;

		public override void OnEnter(PushdownAutomata pda)
		{
			window = new GUIWindow(style: Game.GUIStyle.Block);
			window.SetAlign(GUIWindow.Align.MiddleCenter);
			window.focusWindow = true;
			window.SetContentLayout(GUIWindow.ContentLayout.Vertical);
			window.backColor.a = 128;
			window.spacing = 8;
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.SetShadowBackTexture("gui/images/bar");
			window.paddingHeight = 6;

			window.animation = new GUIAnimationSize(0.35f, int.MinValue, 0);

			{
				GUIElement element;
				window.Add(element = new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.EnterYourName, Game.GUIStyle.MediumFont, animation: new GUIAnimationText(1), percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter) }));

				guiKeyboard = new GUIKeyboard();
				guiKeyboard.textMaxLength = 8;
				guiKeyboard.SetText(Game.settings.playerName);
				window.Add(guiKeyboard);
			}

			GUI.Add(window);
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(GUI.buttonPushed != null)
			{
				switch(GUI.buttonPushed.buttonID)
				{
					case Game.ButtonID.Keyboard:

					if(Game.newHighScores != null)
					{
						Game.settings.playerName = Game.newHighScores.name = guiKeyboard.GetText();
						HighScores.RemoveAllBelowN(Game.highScores, 10);
						HighScores.Save("/local", Game.highScores);
						HighScores.SaveReplay("/local", Game.newHighScores.replayFileName, Game.savedGameActions);
						HighScores.DeleteDropout("/local", Game.highScores);
						Game.SaveSettings();
					}

					pda.Pop(this);
					return;
				}
			}
		}
	}
}
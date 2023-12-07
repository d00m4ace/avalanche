using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateResume : GameState
	{
		GUIWindow window;
		GUICounterInteger timeCounter;

		public override void OnEnter(PushdownAutomata pda)
		{
			window = new GUIWindow(style: Game.GUIStyle.Block);
			window.SetAlign(GUIWindow.Align.MiddleCenter);
			window.focusWindow = true;
			window.SetContentLayout(GUIWindow.ContentLayout.Vertical);
			window.backColor.a = 128;
			window.spacing = 25;
			window.paddingHeight = window.paddingWidth = 25;

			{
				GUIElement element;
				window.Add(element = new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.Ready, Game.GUIStyle.BigFont) }));
				window.Add(element = new GUILabel(new GUIElement[] { timeCounter = new GUICounterInteger(4, 1, 1, 0, Game.GUIStyle.BigFont) }));
				timeCounter.StartCounting();
			}

			GUI.Add(window);
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);

			Game.RestTime();

			Sound.Play(Game.CollectionID.sound_gba_gba12);

			startup.hasUnpaused = false;

			//VideoRecording.ResumeRecording();
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(timeCounter.isCounting)
				return;

			pda.Pop(this);
		}
	}
}
using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStatePlayLevel : GameState
	{
		GUIWindow window;

		GUIText timeGUIText;
		GUIText scoreGUIText;
		GUIText coinsGUIText;

		GUIText speedGUIText;

		GUIProgressBar distanceProgressBar;

		GUIText fpsGUIText;

		public override void OnEnter(PushdownAutomata pda)
		{
			window = new GUIWindow(style: Game.GUIStyle.Empty);
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.SetHeightLayout(GUIWindow.HeightLayout.PercentHeight, 100);
			window.focusWindow = true;

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
				w.SetAlign(GUIWindow.Align.TopLeft);
				w.SetVerticalAlign(GUIWindow.VerticalAlign.Left);
				w.paddingHeight = 0;
				w.paddingWidth = 3;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = distanceProgressBar = new GUIProgressBar(new GUIText("", style: Game.GUIStyle.MediumFont), percentWidth: 96, height: 24));

				w.Add(element = new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.GameScreenScore), scoreGUIText = new GUIText() }, Game.GUIStyle.MediumFont));
				w.Add(element = new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/clock"), timeGUIText = new GUIText() }, Game.GUIStyle.MediumFont));
				w.Add(element = new GUILabel(new GUIElement[] { new GUIImage("gui/images/game/coin"), coinsGUIText = new GUIText() }, Game.GUIStyle.MediumFont));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.BottomRight);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIButton(Game.ButtonID.Pause, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/pause"), new GUIText(Game.TEXT.Pause) }), Game.GUIStyle.Button, defaultFocus: true));
			}

			if(Game.isReplay)
			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);

				w.SetAlign(GUIWindow.Align.BottomLeft);
				w.paddingWidth = 6;
				w.paddingHeight = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIImage("gui/images/icons/camera", Game.GUIStyle.MediumFont, animation: new GUIAnimationBlinkBaseColor(0.5f, Color.Red, false, true)));
				element.color = Color.Zero;
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
				w.SetAlign(GUIWindow.Align.BottomLeft);
				w.SetVerticalAlign(GUIWindow.VerticalAlign.Left);
				w.noClip = true;
				window.Add(w);

				//w.Add(speedGUIText = new GUIText(""));
				//w.Add(fpsGUIText = new GUIText(""));
			}

			GUI.Add(window);

			Game.isPlayLevel = true;

			//VideoRecording.StartRecording();

			Game.RestTime();
		}

		public override void OnExit(PushdownAutomata pda)
		{
			//VideoRecording.StopRecording();

			Sound.StopSounds();

			GUI.Remove(window);
			Game.Rest();

			Game.isPlayLevel = false;
		}

		bool isGameOver;

		float deltaTime = 0.0f;
		int lastTime;

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(false)
			{
				deltaTime += (UnityEngine.Time.deltaTime - deltaTime) * 0.1f;
				float msec = deltaTime * 1000.0f;
				float fps = 1.0f / deltaTime;
				fpsGUIText.SetText(string.Format("{0:0.0} ms ({1:0.} fps) {2}x{3} {4} hits:{5}", msec, fps, UnityEngine.Screen.width, UnityEngine.Screen.height, GUI.scale, Game.maxHitCount));
				//if(Game.maxHitCount > 10) UnityEditor.EditorApplication.isPaused = true;
			}

			if(startup.hasUnpaused)
			{
				//VideoRecording.PauseRecording();

				if(!isGameOver)
					pda.Push(new GameStatePause());

				startup.hasUnpaused = false;

				return;
			}

			{
				AgentSkier playerAgent = Game.player.actor.GetAgent<AgentSkier>();
				coinsGUIText.SetText(playerAgent.coins.ToString());
				scoreGUIText.SetText(playerAgent.score.ToString());

				//speedGUIText.SetText(playerAgent.speed.ToString());

				distanceProgressBar.SetProgress(playerAgent.distance / (Fixed)Game.settings.distance, playerAgent.distance.ToString() + Game.TEXT.MeterShort.GetText() + "/" + Game.settings.distance.ToString() + Game.TEXT.MeterShort.GetText());

				if(lastTime != (int)playerAgent.time)
				{
					timeGUIText.SetText(Utils.TimeToMMSS(playerAgent.time));
					lastTime = (int)playerAgent.time;
				}

				if(playerAgent.isDead && playerAgent.timeDead > 3 && pda.GetState<GameStateGameOver>() == null)
				{
					//VideoRecording.StopRecording();
					Game.saveJoystickActions = false;
					GUI.Remove(window);
					pda.Push(new GameStateGameOver());
					isGameOver = true;
					Sound.StopSounds();
					return;
				}
			}

			Game.GameUpdate();

			Vector2 cam = Game.player.pos + Game.player.vel;
			Render.MoveCameraToPos(cam, GUI.IsPortrait() ? -3 : 0, 10, 0.65f);

			Game.particlesSnow.SetPosToCamera();

			if(!isGameOver && Game.settings.musicOn)
				Sound.PlayMusic(Game.CollectionID.sound_avalanche2, 1, 1, true);

			if(GUI.IsAndroidBackButtonPushed() || (GUI.buttonPushed != null && GUI.buttonPushed.buttonID == Game.ButtonID.Pause))
			{
				//VideoRecording.PauseRecording();

				if(!isGameOver)
					pda.Push(new GameStatePause());

				return;
			}
		}

		public override void OnGameUpdate(PushdownAutomata pda)
		{
			if(Game.iteration == 10 && !Game.isReplay)
			{
				GUIBubble bubble = new GUIBubble(new GUIImage("gui/images/icons/point_hand", scale: 6, animation: new GUIAnimationXYMove(1, 200, int.MinValue, isLoop: true)), 8, GUI.GetScreenWidth() / 2 + 100, GUI.GetScreenHeight() * 3 / 4);
				GUI.AddBack(bubble);
			}

			if(Game.iteration > 0 && Game.iteration % (15 * 10) == 0 && Game.random.Random(100) > 50)
				Sound.Play(Game.CollectionID.sound_random, (Fixed)1 / 3, 1);

			if(isGameOver)
				return;

			LevelGenerator.Update();
		}
	}
}
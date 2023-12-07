using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateMainMenu : GameState
	{
		GUIWindow window;

		GUIText fpsGUIText;

		public override void OnEnter(PushdownAutomata pda)
		{
			window = new GUIWindow(style: Game.GUIStyle.Empty);
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.SetHeightLayout(GUIWindow.HeightLayout.PercentHeight, 100);
			window.SetAlign(GUIWindow.Align.BottomCenter);
			window.focusWindow = true;

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				//w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.TopRight);
				w.paddingHeight = 8;
				w.paddingWidth = 8;
				w.spacing = 8;
				w.noClip = true;
				window.Add(w);

				GUIElement element;

				w.Add(element = new GUIButton(Game.ButtonID.Facebook, new GUIImage("gui/images/icons/facebook_dark"), Game.GUIStyle.BoxButton));
				((GUIButton)element).backColor = Color.RoyalBlue;
				w.Add(element = new GUIButton(Game.ButtonID.Twitter, new GUIImage("gui/images/icons/twitter_dark"), Game.GUIStyle.BoxButton));
				((GUIButton)element).backColor = Color.LightSkyBlue;

			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				//w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.TopLeft);
				w.paddingHeight = 8;
				w.paddingWidth = 8;
				w.spacing = 8;
				w.noClip = true;
				window.Add(w);

				GUIElement element;

				w.Add(element = new GUIButton(Game.ButtonID.InviteFriends, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/hero_cosmic"), new GUIText(Game.TEXT.InviteFriends) }), Game.GUIStyle.BoxButton));
			}

			{
				
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.MiddleCenter);
				w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
				w.spacing = 8;
				w.noClip = true;
				w.animation = new GUIAnimationXY(0.65f, int.MinValue, -GUI.GetScreenHeight());
				window.Add(w);

				GUIElement element;

				w.Add(element = new GUIImage("gui/images/icons/rings", Game.GUIStyle.Default, null,238,118));
				w.Add(element = new GUIText(Game.TEXT.AVALANCHE, Game.GUIStyle.BigFont));
				w.Add(element = new GUIText(Game.TEXT.TheVideoGame, Game.GUIStyle.MediumFont));
				w.Add(element = new GUIText(Game.TEXT.Copyright, defaultFontSize: 8));
				w.Add(element = new GUIText(Game.TEXT.PressStart, Game.GUIStyle.MediumFont, animation: new GUIAnimationBlinkBaseColor(0.5f, Color.Yellow, false, true), percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter));
				element.color = Color.Zero;
				w.Add(element = new GUIButton(Game.ButtonID.Play, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/play"), new GUIText(Game.TEXT.Start) }), Game.GUIStyle.Button, defaultFocus: true));


			}	

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.BottomCenter);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIButton(Game.ButtonID.Shadows, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/wrench"), new GUIText(Game.TEXT.Shadows) }), Game.GUIStyle.Button));
				w.Add(new FlexibleSpace());
				w.Add(element = new GUIButton(Game.ButtonID.Options, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/itablet"), new GUIText(Game.TEXT.Options) }), Game.GUIStyle.Button));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.BottomCenter);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIText("v" + UnityEngine.Application.version, defaultFontSize: 8));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.TopLeft);
				w.noClip = true;
				window.Add(w);

				w.Add(fpsGUIText = new GUIText("", percentWidth: 100));
			}

			GUI.Add(window);

			Game.RestTime();

//			if(LocalNotifications.isClickOnLocalNotification)
//			{
//				LocalNotifications.isClickOnLocalNotification = false;
//				Game.settings.coins += 100;
//				Game.SaveSettings();
//			}
//			Social.FirstSignIn();
		}

		public override void OnExit(PushdownAutomata pda)
		{
			Sound.StopSounds();

			GUI.Remove(window);

			Game.Rest();
		}

		float deltaTime = 0.0f;

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

			AgentSkier playerAgent = Game.player.actor.GetAgent<AgentSkier>();
			playerAgent.isBot = true;

			if(playerAgent.isDead)
			{
				LevelGenerator.ShuffleSkierList();
				Entity2D skier = LevelGenerator.GetNextLiveRandomSkier();
				if(skier != null)
					Game.player = skier;
			}

			Game.GameUpdate();

			Vector2 cam = Game.player.pos + Game.player.vel;
			Render.MoveCameraToPos(cam, -6, 10, 0.65f);

			Game.particlesSnow.SetPosToCamera();

			if(Game.settings.musicOn)
				Sound.PlayMusic(Game.CollectionID.sound_avalanche2, 1, 1, true);

			if(GUI.IsAndroidBackButtonPushed())
			{				
				pda.Push(new GameStateExit());
				return;
			}

			if(GUI.buttonPushed != null)
			{
				switch(GUI.buttonPushed.buttonID)
				{
					case Game.ButtonID.Play:
					pda.Pop(this);
					pda.Push(new PushdownAutomata.TransitionState(new GameStateMainMenu()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateRestartLevel()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateSlotmachine()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateHighScores()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStatePlayLevel()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateRestartLevel()));
					pda.Push(new GameStateSelectHero());
					return;

					case Game.ButtonID.Options:
					pda.Push(new GameStateOptions());
					return;

					case Game.ButtonID.Shadows:
					Game.settings.shadowsOn = !Game.settings.shadowsOn;
					Render.SetQualitySettings();
					Game.SaveSettings();
					return;

					case Game.ButtonID.InviteFriends:
					//AppInvite.SendInvitation();
					//UM_ShareUtility.ShareMedia(Game.TEXT.AVALANCHE.GetText(), Game.TEXT.AppInviteMessage.GetText() + " " + Game.appInviteShareURL);
					return;

					case Game.ButtonID.Facebook:
					//UnityEngine.Application.OpenURL("https://www.facebook.com/hexplay.games");
					return;

					case Game.ButtonID.Twitter:
					//UnityEngine.Application.OpenURL("https://twitter.com/hexplay");
					return;
				}
			}
		}

		public override void OnGameUpdate(PushdownAutomata pda)
		{
			if(Game.iteration > 0 && Game.iteration % (15 * 10) == 0 && Game.random.Random(100) > 50)
				Sound.Play(Game.CollectionID.sound_random, (Fixed)1 / 3, 1);

			LevelGenerator.Update();
		}
	}
}
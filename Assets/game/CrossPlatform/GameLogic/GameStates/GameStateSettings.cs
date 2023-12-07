using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateSettings : GameState
	{
		GUIWindow window;

		Game.ButtonID buttonPushed;
		bool isClosed = false;

		GUIToggle shadowsGUIToggle;
		GUIToggle musicGUIToggle;
		GUIToggle soundGUIToggle;

		GUIToggle faceCamGUIToggle;
		GUIToggle videoRecGUIToggle;

		GUIToggle socialSignInGUIToggle;

		GUISelector languageGUISelector;

		GUIWindow logWindow;
		GUIText logText;

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
				window.Add(new GUIText(Game.TEXT.Settings, Game.GUIStyle.MediumFont));

				{
					GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
					w.contentEqualWidth = true;
					w.SetContentLayout(GUIWindow.ContentLayout.Vertical);

					w.Add(new GUILabel(new GUIElement[] {
						new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/musical_double"), new GUIText(Game.TEXT.Music), musicGUIToggle = new GUIToggle(Game.ButtonID.Music, new GUIElement[] { new GUIText(Game.TEXT.On, textMaxLength: 4), new GUIText(Game.TEXT.Off, textMaxLength: 4) }, new Color[] { Color.LightGreen, Color.Red }) }, Game.GUIStyle.BoxButton),
						new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/sound"), new GUIText(Game.TEXT.Sound), soundGUIToggle = new GUIToggle(Game.ButtonID.Sound, new GUIElement[] { new GUIText(Game.TEXT.On, textMaxLength: 4), new GUIText(Game.TEXT.Off, textMaxLength: 4) }, new Color[] { Color.LightGreen, Color.Red }) }, Game.GUIStyle.BoxButton),
					}));

//					if(VideoRecording.IsReadyForRecording())
//					{
//						w.Add(new GUILabel(new GUIElement[] {
//							new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/hero_cosmic"), new GUIText(Game.TEXT.FaceCam, textMaxLength: 8), faceCamGUIToggle = new GUIToggle(Game.ButtonID.FaceCam, new GUIElement[] { new GUIText(Game.TEXT.On, textMaxLength: 4), new GUIText(Game.TEXT.Off, textMaxLength: 4) }, new Color[] { Color.LightGreen, Color.Red }) }, Game.GUIStyle.BoxButton),
//							new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/camera"), new GUIText(Game.TEXT.VideoRec, textMaxLength: 8), videoRecGUIToggle = new GUIToggle(Game.ButtonID.VideoRec, new GUIElement[] { new GUIText(Game.TEXT.On, textMaxLength: 4), new GUIText(Game.TEXT.Off, textMaxLength: 4) }, new Color[] { Color.LightGreen, Color.Red }) }, Game.GUIStyle.BoxButton),
//						}));
//					}

					w.Add(new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/bubble_dark"), new GUIText(Game.TEXT.Language),
						languageGUISelector = new GUISelector(new GUIElement[] {
							new GUIText(Game.TEXT.English, defaultFontSize: 8),
							new GUIText(Game.TEXT.Russian, defaultFontSize: 8),
							new GUIText(Game.TEXT.Germany, defaultFontSize: 8),
							new GUIText(Game.TEXT.French, defaultFontSize: 8),
							new GUIText(Game.TEXT.Italian, defaultFontSize: 8),
							new GUIText(Game.TEXT.Spanish, defaultFontSize: 8),
							new GUIText(Game.TEXT.ChineseSimplified, defaultFontSize: 16),
							new GUIText(Game.TEXT.ChineseTraditional, defaultFontSize: 16),
							new GUIText(Game.TEXT.Korean, defaultFontSize: 16),
							new GUIText(Game.TEXT.Japanese, defaultFontSize: 16),
							new GUIText(Game.TEXT.Thai, defaultFontSize: 16),
							new GUIText(Game.TEXT.Vietnamese, defaultFontSize: 8),
							new GUIText(Game.TEXT.Arabic, defaultFontSize: 16),
							new GUIText(Game.TEXT.Indonesian, defaultFontSize: 8),
							new GUIText(Game.TEXT.Polish, defaultFontSize: 8),
							new GUIText(Game.TEXT.Norwegian, defaultFontSize: 8),
							new GUIText(Game.TEXT.Portuguese, defaultFontSize: 8),
							new GUIText(Game.TEXT.Finnish, defaultFontSize: 8),
							new GUIText(Game.TEXT.Dutch, defaultFontSize: 8),
							new GUIText(Game.TEXT.Czech, defaultFontSize: 8),
							new GUIText(Game.TEXT.Catalan, defaultFontSize: 8),
							new GUIText(Game.TEXT.Greek, defaultFontSize: 8),
							new GUIText(Game.TEXT.Icelandic, defaultFontSize: 8),
							new GUIText(Game.TEXT.Turkish, defaultFontSize: 8),
							new GUIText(Game.TEXT.Swedish, defaultFontSize: 8),
						}) }, Game.GUIStyle.BoxButton));

					w.Add(new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/wrench"), new GUIText(Game.TEXT.Shadows), shadowsGUIToggle = new GUIToggle(Game.ButtonID.Shadows, new GUIElement[] { new GUIText(Game.TEXT.On, textMaxLength: 4), new GUIText(Game.TEXT.Off, textMaxLength: 4) }, new Color[] { Color.LightGreen, Color.Red }) }, Game.GUIStyle.BoxButton));

#if UNITY_ANDROID
					w.Add(new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/google_play"), new GUIText(Game.TEXT.GooglePlayServices), socialSignInGUIToggle = new GUIToggle(Game.ButtonID.SocialSignIn, new GUIElement[] { new GUIText(Game.TEXT.SignOut, textMaxLength: 8), new GUIText(Game.TEXT.SignIn, textMaxLength: 8) }, new Color[] { Color.LightGreen, Color.LightBlue }) }, Game.GUIStyle.BoxButton));
#endif

#if UNITY_IOS || UNITY_STANDALONE_OSX
					w.Add(new GUILabel(new GUIElement[] {
						new GUIImage("gui/images/icons/trolley"), new GUIText(Game.TEXT.RestorePurchases), 
						new GUIButton(Game.ButtonID.RestorePurchases, new GUILabel(new GUIElement[] { new GUIText("▶") }))
					}, Game.GUIStyle.BoxButton));
#endif

					if(Game.settings.language > 0) languageGUISelector.SetSelector(Game.settings.language - 1);

					shadowsGUIToggle.SetToggle(Game.settings.shadowsOn ? 0 : 1);

					musicGUIToggle.SetToggle(Game.settings.musicOn ? 0 : 1);
					soundGUIToggle.SetToggle(Game.settings.soundOn ? 0 : 1);

//					if(VideoRecording.IsReadyForRecording())
//					{
//						faceCamGUIToggle.SetToggle(Game.settings.faceCamOn ? 0 : 1);
//						videoRecGUIToggle.SetToggle(Game.settings.videoRecOn ? 0 : 1);
//					}

#if UNITY_ANDROID
					socialSignInGUIToggle.SetToggle(Social.IsSignIn() ? 0 : 1);
#endif

					window.Add(w);
				}

				window.Add(element = new GUIButton(Game.ButtonID.Back, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/back"), new GUIText(Game.TEXT.Back) }), Game.GUIStyle.Button, defaultFocus: true));
			}

			GUI.Add(window);

			{
				logWindow = new GUIWindow(style: Game.GUIStyle.Empty);
				logWindow.SetAlign(GUIWindow.Align.TopLeft);
				logWindow.noClip = true;

				logWindow.Add(logText = new GUIText("", width: UnityEngine.Screen.width, height: UnityEngine.Screen.height));

				GUI.Add(logWindow);
			}
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);
			GUI.Remove(logWindow);
		}

		int logPressCount;

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(false)
			{
				logText.SetText(
					"attitude: " + UnityEngine.Input.gyro.attitude + "\n" +
					"gravity: " + UnityEngine.Input.gyro.gravity + "\n" +
					"rotationRate: " + UnityEngine.Input.gyro.rotationRate + "\n" +
					"rotationRateUnbiased: " + UnityEngine.Input.gyro.rotationRateUnbiased + "\n" +
					"updateInterval: " + UnityEngine.Input.gyro.updateInterval + "\n" +
					"userAcceleration: " + UnityEngine.Input.gyro.userAcceleration + "\n" + "\n" +

					"Horizontal: " + UnityEngine.Input.GetAxis("Horizontal") +
					" Vertical: " + UnityEngine.Input.GetAxis("Vertical") + "\n" + "\n" +

					"Mouse X: " + UnityEngine.Input.GetAxis("Mouse X") +
					" Mouse Y: " + UnityEngine.Input.GetAxis("Mouse Y")
					);
			}

			if(window.animation.IsPlaying())
				return;

			if(isClosed)
			{
				switch(buttonPushed)
				{
					case Game.ButtonID.Back:
					pda.Pop(this);

					Game.settings.shadowsOn = shadowsGUIToggle.toggle == 0 ? true : false;

					Game.settings.musicOn = musicGUIToggle.toggle == 0 ? true : false;
					Game.settings.soundOn = soundGUIToggle.toggle == 0 ? true : false;

//					if(VideoRecording.IsReadyForRecording())
//					{
//						Game.settings.faceCamOn = faceCamGUIToggle.toggle == 0 ? true : false;

//						if(Game.settings.faceCamOn)
//							Game.settings.faceCamOn = RuntimePermissions.IsPermissionsOnRecordingGranted();

//						Game.settings.videoRecOn = videoRecGUIToggle.toggle == 0 ? true : false;
//					}

#if UNITY_ANDROID
					Game.settings.socialSignIn = socialSignInGUIToggle.toggle == 0 ? true : false;
#endif

					Render.SetQualitySettings();

					Sound.StopSounds();

					Game.settings.language = languageGUISelector.selector + 1;
					Utils.SetLanguage();
					GUI.RestSize();

					Game.SaveSettings();
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

			if(GUI.buttonPushed != null && GUI.buttonPushed.buttonID == Game.ButtonID.Back)
			{
				window.animation.PlayInverse();
				buttonPushed = GUI.buttonPushed.buttonID;
				isClosed = true;
			}
			else if(GUI.buttonPushed != null && GUI.buttonPushed.buttonID == Game.ButtonID.RestorePurchases)
			{
//				IAP.RestorePurchases();
			}
			else if(GUI.buttonPushed != null && GUI.buttonPushed.buttonID == Game.ButtonID.SocialSignIn)
			{
				bool socialSignIn = socialSignInGUIToggle.toggle == 0 ? true : false;

//				if(socialSignIn && !Social.IsSignIn())
//					Social.SignIn();
//				else if(!socialSignIn && Social.IsSignIn())
//					Social.SignOut();
			}
			else if(GUI.buttonPushed != null && GUI.buttonPushed.buttonID == Game.ButtonID.FaceCam)
			{
				bool faceCamOn = faceCamGUIToggle.toggle == 0 ? true : false;

				if(faceCamOn)
				{
#if UNITY_IPHONE
					if(!Everyplay.FaceCamIsRecordingPermissionGranted())
						Everyplay.FaceCamRequestRecordingPermission();
#endif

#if UNITY_ANDROID
					//if(!RuntimePermissions.IsPermissionsOnRecordingGranted())
					//RuntimePermissions.OpenDialogPermissionsOnRecording();
#endif
				}
			}

			if(GUI.isCursorFireUp)
			{
				if(GUI.cursorPos.x > 0 && GUI.cursorPos.x < 20 && GUI.cursorPos.y > 0 && GUI.cursorPos.y < 20)
				{
					logPressCount++;
					Console.WriteLine(GUI.cursorPos);

					if(logPressCount == 3)
					{
						Sound.Play(Game.CollectionID.sound_click);
//						UM_ShareUtility.ShareMedia("Log", startup.GetLog());
					}
				}
			}
		}
	}
}
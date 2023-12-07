using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateNotEnoughCoins : GameState
	{
		GUIWindow window;

		GUIButton adsRewardedVideoButton;
		GUIImage adsRewardedVideoImage;

		Game.ButtonID buttonPushed;
		bool isClosed = false;

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
			window.shadowBackTextureColor = Color.Dark * Color.WhiteAlpha50;
			window.paddingHeight = 6;

			window.animation = new GUIAnimationSize(0.15f, int.MinValue, 0);

			{
				GUIElement element;
				window.Add(element = new GUIText(Game.TEXT.NotEnoughCoins, Game.GUIStyle.MediumFont, percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter));

				window.Add(element = adsRewardedVideoButton = new GUIButton(Game.ButtonID.AdsRewardedVideo, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/video_ad"), new GUIText("+100"), adsRewardedVideoImage = new GUIImage("gui/images/game/coin") }), Game.GUIStyle.Button, defaultFocus: true));

				window.Add(element = new GUIButton(Game.ButtonID.Back, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/back"), new GUIText(Game.TEXT.Back) }), Game.GUIStyle.Button));
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

			if(isClosed)
			{
				pda.Pop(this);
				return;
			}


//			adsRewardedVideoButton.isDisabled = !Ads.IsRewardedVideoAdReady();

//			if(Ads.IsRewardedVideoAdFinished())
			{
				Game.settings.coins += ((HEXInt)79) + 21;

				Game.SaveSettings();

				isClosed = true;
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
				if(GUI.buttonPushed.buttonID == Game.ButtonID.AdsRewardedVideo)
				{					
//					if(Ads.IsRewardedVideoAdReady()) pda.Push(new GameStateRewardedVideoAd());
					return;
				}

				window.animation.PlayInverse();
				buttonPushed = GUI.buttonPushed.buttonID;
				isClosed = true;
			}
		}
	}
}
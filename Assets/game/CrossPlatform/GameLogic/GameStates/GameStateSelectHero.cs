using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateSelectHero : GameState
	{
		GUIWindow window;

		public int heroSelected = 1;

		GUIText heroName;
		GUIText heroRemoveADs;

		GUIImage heroForbiddenImage;
		GUILabel heroTokensLabel;
		GUIImage heroTokenImage;
		GUIProgressBar heroTokensProgressBar;

		GUIButton playButton;
		GUIText heroCoinsText;

		GUIButton adsRewardedVideoButton;
		GUIImage adsRewardedVideoImage;

		GUIText coinsText;
		HEXInt coins;

		GUILabel playLabel;
		GUILabel buyLabel;

		public override void OnEnter(PushdownAutomata pda)
		{
			//IAP.onPurchaseComplete += onPurchaseComplete;
			//IAP.onPurchaseFailed += onPurchaseFailed;

			coins = Game.settings.coins = 1000000;

			heroSelected = Game.settings.heroSelected;

			window = new GUIWindow(style: Game.GUIStyle.Empty);
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.SetHeightLayout(GUIWindow.HeightLayout.PercentHeight, 100);
			window.SetAlign(GUIWindow.Align.BottomCenter);
			window.focusWindow = true;

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.TopCenter);
				w.noClip = true;
				w.paddingHeight = 16;
				w.paddingWidth = 8;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIText(Game.TEXT.SelectYourHero, Game.GUIStyle.MediumFont, percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.MiddleLeft);
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIButton(Game.ButtonID.Left, new GUIText("◀"), Game.GUIStyle.MediumFont));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.MiddleRight);
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIButton(Game.ButtonID.Right, new GUIText("▶"), Game.GUIStyle.MediumFont));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
				w.SetAlign(GUIWindow.Align.MiddleCenter);
				w.noClip = true;
				w.spacing = 6;
				window.Add(w);

				GUIElement element;

				w.Add(element = new GUIElement(height: 8 + Utils.GetLanguageDefaultFontSize() * 2));

				w.Add(element = new GUILabel(new GUIElement[] { heroForbiddenImage = new GUIImage("gui/images/icons/forbidden", scale: 6) }, Game.GUIStyle.Default, height: 8 * 9));
				heroForbiddenImage.color = Color.Red;

				w.Add(element = heroName = new GUIText("", Game.GUIStyle.MediumFont));

				w.Add(element = heroTokensLabel = new GUILabel(new GUIElement[]
				{
					heroTokenImage = new GUIImage("gui/images/coins/coin001"),
					heroTokensProgressBar = new GUIProgressBar(new GUIText("", Game.GUIStyle.MediumFont), percentWidth: 55, height: 24),
				}));

				w.Add(element = heroRemoveADs = new GUIText(Game.TEXT.RemoveADs, percentWidth: 90, heightTextLines: 2, align: GUIText.Align.MiddleCenter));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.BottomRight);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = adsRewardedVideoButton = new GUIButton(Game.ButtonID.AdsRewardedVideo, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/video_ad"), new GUIText("+25", defaultFontSize: 8), adsRewardedVideoImage = new GUIImage("gui/images/coins/coin001") }), Game.GUIStyle.Button));
				adsRewardedVideoImage.scale = 0.5f;
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.TopLeft);
				w.paddingHeight = 2;
				w.paddingWidth = 2;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUILabel(new GUIElement[] { new GUIImage("gui/images/game/coin"), coinsText = new GUIText(coins.ToString()) }, Game.GUIStyle.Default, minWidth: 80));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.BottomLeft);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIButton(Game.ButtonID.Back, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/back"), new GUIText(Game.TEXT.Back) }), Game.GUIStyle.Button));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetAlign(GUIWindow.Align.BottomCenter);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				playLabel = new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/play"), new GUIText(Game.TEXT.Play) }, Game.GUIStyle.Button);

//				if(IAP.IsInitialized())
//					buyLabel = new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/trolley"), heroCoinsText = new GUIText("xxx00.00", defaultFontSize: 8) }, Game.GUIStyle.Button);
//				else //FOR COINS!
					buyLabel = new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/trolley"), new GUIImage("gui/images/game/coin"), heroCoinsText = new GUIText("2000", defaultFontSize: 8) }, Game.GUIStyle.Button);

				w.Add(element = playButton = new GUIButton(Game.ButtonID.Play, playLabel, Game.GUIStyle.Button, defaultFocus: true));
			}

			GUI.Add(window);

			for(int i = 0; i < Game.collection.elementsPool[Game.ObjectType.Skier].Count; i++)
			{
				Entity2D entity = (Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][i];
				entity.Rest();
				entity.SetPos(Vector2.V((i + 1) * 2, (i + 1) * 2));
				entity.SetZ(Fixed.OneHalf);
				entity.Enable(true);
				entity.Update();

				if(i == heroSelected - 1)
				{
					GameObjectAnimation goa = entity.model.GetGameObjectAnimation();
					goa.goAnimation = new GOAnimationZRotation(90);
				}
			}

			SelectHero();

			Render.portraitCameraOffsetAspectFix = false;
			Render.SetCameraZoom(3);
			Render.SetCameraPos(Vector2.V(heroSelected * 2, heroSelected * 2), 1, 10);

			if(Game.settings.faceCamOn)
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

		public void onPurchaseComplete(string productId)
		{
			int id;

			if(!int.TryParse(productId.Substring(productId.LastIndexOf('.') + 1), out id))
				return;

			//if(!IAP.ValidateReceiptProductID(productId))
			//	return;

			Game.settings.hasIAP = true;

			Game.settings.heroSelected = id;
			Game.heroSet[id - 1].heroTokens = Game.heroSet[id - 1].heroTokensTotal;

			HeroSet.Save(Game.heroSet);
			Game.SaveSettings();

			Game.gamePDA.Pop(this);
			Game.gamePDA.Push(new PushdownAutomata.TransitionState(new GameStateSelectHero()));
			Game.gamePDA.Push(new GameStateGotNewHero(id - 1));

			Console.WriteLine("onPurchaseComplete:" + productId);
		}

		public void onPurchaseFailed(string productId)
		{
			Console.WriteLine("onPurchaseFailed:" + productId);
		}

		void SelectHero()
		{
			heroName.SetText((Game.TEXT.HeroName0 + heroSelected));

			string heroIcon = "gui/images/coins/coin" + heroSelected.ToString("000");
			heroTokenImage.SetImage(heroIcon);
			adsRewardedVideoImage.SetImage(heroIcon);

			string productId = "com.herocraft.game.free.avalance.hero." + heroSelected;
			Console.WriteLine(productId);

/*
			if(IAP.IsInitialized())
			{
				if(Game.heroSet[heroSelected - 1].heroTokens != Game.heroSet[heroSelected - 1].heroTokensTotal)
				{
					if(IAP.ValidateReceiptProductID(productId))
					{
						Game.settings.hasIAP = true;
						Game.heroSet[heroSelected - 1].heroTokens = Game.heroSet[heroSelected - 1].heroTokensTotal;
						HeroSet.Save(Game.heroSet);
					}
				}
			}
*/
			heroTokensProgressBar.SetProgress((Fixed)Game.heroSet[heroSelected - 1].heroTokens / Game.heroSet[heroSelected - 1].heroTokensTotal, Game.heroSet[heroSelected - 1].heroTokens.ToString() + "/" + Game.heroSet[heroSelected - 1].heroTokensTotal);

			heroForbiddenImage.isDisabled = heroTokensLabel.isDisabled = heroRemoveADs.isDisabled = Game.heroSet[heroSelected - 1].heroTokens == Game.heroSet[heroSelected - 1].heroTokensTotal;

			adsRewardedVideoButton.isDisabled = true;//Game.heroSet[heroSelected - 1].heroTokens == Game.heroSet[heroSelected - 1].heroTokensTotal || !Ads.IsRewardedVideoAdReady();

			heroCoinsText.SetText((Game.heroSet[heroSelected - 1].heroTokensTotal * 8).ToString());

			if(!heroTokensLabel.isDisabled)
			{
				playButton.buttonID = Game.ButtonID.Buy;
				playButton.element = buyLabel;

				//if(IAP.IsInitialized())
					//((GUIText)buyLabel.elements[1]).SetText(IAP.PriceStringProductID(productId));

				playButton.RestSize();
			}
			else
			{
				playButton.buttonID = Game.ButtonID.Play;
				playButton.element = playLabel;
				playButton.RestSize();
			}

			heroSelectedMouseMove = heroSelected;

			window.RestSize();
		}

		public override void OnExit(PushdownAutomata pda)
		{
			//IAP.onPurchaseComplete -= onPurchaseComplete;
			//IAP.onPurchaseFailed -= onPurchaseFailed;

			GUI.Remove(window);

			for(int i = 0; i < Game.collection.elementsPool[Game.ObjectType.Skier].Count; i++)
				World2D.Delete((Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][i]);

			World2D.DeleteDeletedEntities();

			Render.portraitCameraOffsetAspectFix = true;
		}

		Fixed heroSelectedMouseMove;

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(coins != Game.settings.coins)
			{
				coins = Game.settings.coins;
				coinsText.SetText(coins.ToString());
			}

			adsRewardedVideoButton.isDisabled = true;//Game.heroSet[heroSelected - 1].heroTokens == Game.heroSet[heroSelected - 1].heroTokensTotal || !Ads.IsRewardedVideoAdReady();

			//if(Ads.IsRewardedVideoAdFinished())
			if(false)
			{
				Game.heroSet[heroSelected - 1].heroTokens += 25;

				if(Game.heroSet[heroSelected - 1].heroTokens >= Game.heroSet[heroSelected - 1].heroTokensTotal)
				{
					Game.settings.heroSelected = heroSelected;
					Game.heroSet[heroSelected - 1].heroTokens = Game.heroSet[heroSelected - 1].heroTokensTotal;

					pda.Pop(this);
					pda.Push(new PushdownAutomata.TransitionState(new GameStateSelectHero()));
					pda.Push(new GameStateGotNewHero(heroSelected - 1));
				}
				else
				{
					SelectHero();
				}

				HeroSet.Save(Game.heroSet);
				Game.SaveSettings();

				return;
			}

			Action<int> selectNewHero = (newHeroSelected) =>
			{
				if(newHeroSelected < 1)
					newHeroSelected = 1;
				else if(newHeroSelected > Game.collection.elementsPool[Game.ObjectType.Skier].Count)
					newHeroSelected = Game.collection.elementsPool[Game.ObjectType.Skier].Count;

				heroSelectedMouseMove = newHeroSelected;

				if(newHeroSelected != heroSelected)
				{
					Entity2D hero = (Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][heroSelected - 1];
					hero.model.RestGameObjectAnimation();
					hero.model.PlayAnimation(Game.CollectionID.animation_idle);
					hero.Update();

					heroSelected = newHeroSelected;
					hero = (Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][heroSelected - 1];

					GameObjectAnimation goa = hero.model.GetGameObjectAnimation();
					goa.goAnimation = new GOAnimationZRotation(90);

					hero.Update();

					Sound.Play(Game.CollectionID.sound_click);

					SelectHero();
				}
			};

			Game.ButtonID buttonID = Game.ButtonID.None;

			if(GUI.IsAndroidBackButtonPushed())
				buttonID = Game.ButtonID.Back;

			if(GUI.buttonPushed != null)
				buttonID = GUI.buttonPushed.buttonID;

			if(buttonID != Game.ButtonID.None)
			{
				int newHeroSelected = heroSelected;

				switch(buttonID)
				{
					case Game.ButtonID.Play:
					Game.isReplay = false;
					Game.settings.heroSelected = heroSelected;
					pda.Pop(this);
					return;

					case Game.ButtonID.Buy:
					{
//						if(IAP.IsInitialized())
//						{
//							string productId = "com.herocraft.game.free.avalance.hero." + heroSelected;
//							Console.WriteLine(productId);
//							IAP.BuyProductID(productId);
//						}
//						else
						{
							HEXInt cost = Game.heroSet[heroSelected - 1].heroTokensTotal * 8;

							if(Game.settings.coins > cost)
							{
								Game.settings.heroSelected = heroSelected;
								Game.settings.coins -= cost;
								Game.heroSet[heroSelected - 1].heroTokens = Game.heroSet[heroSelected - 1].heroTokensTotal;

								HeroSet.Save(Game.heroSet);
								Game.SaveSettings();

								pda.Pop(this);
								pda.Push(new PushdownAutomata.TransitionState(new GameStateSelectHero()));
								pda.Push(new GameStateGotNewHero(heroSelected - 1));
							}
							else
							{
								pda.Push(new GameStateNotEnoughCoins());
							}
						}
					}
					return;

					case Game.ButtonID.AdsRewardedVideo:
//					if(Ads.IsRewardedVideoAdReady()) pda.Push(new GameStateRewardedVideoAd());
					return;

					case Game.ButtonID.Left:
					newHeroSelected--;
					break;

					case Game.ButtonID.Right:
					newHeroSelected++;
					break;

					case Game.ButtonID.Back:
					pda.PopAll();
					pda.Push(new PushdownAutomata.TransitionState(new GameStateMainMenu()));
					pda.Push(new GameStateRestartLevel());
					break;
				}

				selectNewHero(newHeroSelected);
			}

			{
				Entity2D hero = (Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][heroSelected - 1];

				if(!hero.model.IsPlayingAnimation(Game.CollectionID.animation_move))
					hero.model.PlayAnimation(Game.CollectionID.animation_move);
			}

			if(GUI.isCursorDown && !GUI.isCursorInButtonArea)
			{
				heroSelectedMouseMove += -(Fixed)(GUI.deltaCursorPositionX / (UnityEngine.Screen.width / 5));

				if((int)Math.Round(heroSelectedMouseMove) != heroSelected)
					selectNewHero((int)Math.Round(heroSelectedMouseMove));
			}

			Render.MoveCameraToPos(Vector2.V(heroSelectedMouseMove * 2, heroSelectedMouseMove * 2), 1, 10, 0.36f);

			Game.particlesSnow.SetPosToCamera();
		}
	}
}
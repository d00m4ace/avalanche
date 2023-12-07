using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateSlotmachine : GameState
	{
		GUIWindow window;
		GUIWindow tokensWindow;

		GUIText coinsText;
		HEXInt coins;

		public override void OnEnter(PushdownAutomata pda)
		{
			coins = Game.settings.coins;

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
				w.paddingHeight = 8 * 3;
				w.paddingWidth = 8;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIText(Game.TEXT.SpinToWinANewHeroes, Game.GUIStyle.MediumFont, percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter));
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
				w.SetAlign(GUIWindow.Align.BottomCenter);
				w.noClip = true;
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.spacing = 6;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIButton(Game.ButtonID.Spin, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/slotmachine"), new GUIImage("gui/images/game/coin"), new GUIText("10") }), Game.GUIStyle.Button));
				w.Add(element = new GUIButton(Game.ButtonID.Continue, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/play"), new GUIText(Game.TEXT.Continue) }), Game.GUIStyle.Button, defaultFocus: true));
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

			GUI.Add(window);

			Game.slotmachine.Rest();
			Game.slotmachine.SetAngle(-120 * Math.PI / 180);
			Game.slotmachine.SetPos(Vector2.V(0, 0));
			Game.slotmachine.Enable(true);
			Game.slotmachine.Update();
			Game.slotmachine.model.PlayAnimation(Game.CollectionID.animation_idle);

			Render.portraitCameraOffsetAspectFix = false;
			Render.SetCameraZoom(2.5f);
			Render.SetCameraPos(Vector2.V(0, 0), 2.5f, 10);
		}

		PseudoRandom random = new PseudoRandom();
		List<HEXInt> tokens = new List<HEXInt>();

		void GenerateTokens()
		{
			random.Rest();
			tokens.Clear();

			HEXInt coinsCount = random.Random(100);

			if(coinsCount > 100 - 35) coinsCount = 1;
			else if(coinsCount > 100 - 35 - 25) coinsCount = 2;
			else if(coinsCount > 100 - 35 - 25 - 15) coinsCount = 3;
			else if(coinsCount > 100 - 35 - 25 - 15 - 10) coinsCount = 4;
			else if(coinsCount > 100 - 35 - 25 - 15 - 10 - 7) coinsCount = 5;
			else if(coinsCount > 100 - 35 - 25 - 15 - 10 - 7 - 5) coinsCount = 6;
			else coinsCount = 7;

			for(int i = 0; i < coinsCount; i++)
			{
				HEXInt token = random.Random(Game.heroSet.Count);

				if(Game.heroSet[token].heroTokens == Game.heroSet[token].heroTokensTotal - 1)
					Game.heroSet[token].heroReceived = true;

				if(Game.heroSet[token].heroTokens >= Game.heroSet[token].heroTokensTotal)
				{
					token = -1; Game.settings.coins++;
				}
				else
					Game.heroSet[token].heroTokens++;

				tokens.Add(token + 1);
			}
		}

		void CreateTokensWindow()
		{
			tokensWindow = new GUIWindow(style: Game.GUIStyle.Block);
			tokensWindow.SetAlign(GUIWindow.Align.MiddleCenter);
			tokensWindow.focusWindow = false;
			tokensWindow.SetContentLayout(GUIWindow.ContentLayout.Horizontal);
			tokensWindow.backColor.a = 128;
			tokensWindow.spacing = 8;
			tokensWindow.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			tokensWindow.paddingHeight = tokensWindow.paddingWidth = 8;
			tokensWindow.minHeight = (int)(25 * 1.5f) + tokensWindow.paddingHeight + tokensWindow.paddingWidth;

			tokensWindow.animation = new GUIAnimationFilter(new GUIAnimation[] { new GUIAnimationSize(0.35f, int.MinValue, 0), new GUIAnimationElementsWait(tokensWindow) });

			GUIElement element;

			for(int i = 0; i < tokens.Count; i++)
			{
				if(tokens[i] != 0)
					tokensWindow.Add(element = new GUIImage("gui/images/coins/coin" + tokens[i].ToString("000"), scale: 1.5f));
				else
					tokensWindow.Add(element = new GUIImage("gui/images/game/coin", scale: 3));

				element.animation = new GUIAnimationXY(0.35f, GUI.GetScreenWidth());

				element.animation.endSound = Game.CollectionID.sound_coin;
			}

			GUI.Add(tokensWindow);
		}

		void FreeTokensWindow()
		{
			if(tokensWindow != null)
				GUI.Remove(tokensWindow);

			tokensWindow = null;
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);

			FreeTokensWindow();

			Game.slotmachine.Enable(false);
			Game.slotmachine.Update();

			HeroSet.AddGameStateGotNewHero(pda);

			Render.portraitCameraOffsetAspectFix = true;
		}

		bool isSpin;
		bool isSpinning;

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(coins != Game.settings.coins)
			{
				coins = Game.settings.coins;
				coinsText.SetText(coins.ToString());
			}

			if(GUI.buttonPushed != null && !isSpin)
			{
				switch(GUI.buttonPushed.buttonID)
				{
					case Game.ButtonID.Spin:
					if(tokensWindow == null || !tokensWindow.IsPlayingAnimation())
					{
						if(Game.settings.coins >= 10)
						{
							Game.slotmachine.model.PlayAnimation(Game.CollectionID.animation_spin);

							isSpin = true;

							Game.settings.coins -= 10;

							FreeTokensWindow();

							Sound.StopOne(Game.CollectionID.sound_slotmachine);
							Sound.Play(Game.CollectionID.sound_slotmachine_start);
						}
						else
						{
							pda.Push(new GameStateNotEnoughCoins());
						}
					}
					break;

					case Game.ButtonID.Continue:
					if(!isSpinning && (tokensWindow == null || !tokensWindow.IsPlayingAnimation()))
					{
						Sound.StopOne(Game.CollectionID.sound_slotmachine);
						pda.Pop(this);
					}
					break;
				}
			}

			if(!Game.slotmachine.model.IsPlayingAnimation() && isSpin)
			{
				if(!isSpinning)
				{
					Game.slotmachine.model.PlayAnimation(Game.CollectionID.animation_spin_1 + Game.random.Random(3), 2);

					GameObjectAnimation goa = Game.slotmachine.model.GetGameObjectAnimation();
					goa.goAnimation = new GOAnimationFilter(
						new GOAnimationXScale(1.0f, 1.0f, 0.9f, 1.2f),
						new GOAnimationYScale(1.0f, 1.0f, 0.9f, 1.2f),
						new GOAnimationZposition(0, 2.0f, -0.2f, 0.2f)
						);

					isSpinning = true;

					Sound.StopOne(Game.CollectionID.sound_slotmachine);
					Sound.PlayOne(Game.CollectionID.sound_slotmachine, 1, 1, false);
				}
				else
				{
					Game.slotmachine.model.RestGameObjectAnimation();
					Game.slotmachine.Update();

					isSpin = isSpinning = false;

					GenerateTokens();
					CreateTokensWindow();

					HeroSet.Save(Game.heroSet);
					Game.SaveSettings();
				}
			}

			if(GUI.buttonPushed != null && GUI.buttonPushed.buttonID == Game.ButtonID.Spin && isSpinning)
				Game.slotmachine.model.StopAnimation();

			if(GUI.isAnyKeyDown)
			{
				GUI.EndAnyAnimations();
				Game.slotmachine.model.StopAnimation();
			}
		}
	}
}
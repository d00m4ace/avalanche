using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateGotNewHero : GameState
	{
		GUIWindow window;

		int heroID;

		GUIText heroName;

		bool isSetted = false;

		public GameStateGotNewHero(int heroID)
		{
			this.heroID = heroID;
		}

		public override void OnEnter(PushdownAutomata pda)
		{
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);

			World2D.Delete((Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][heroID]);
			World2D.DeleteDeletedEntities();

			Game.particlesCoins.Rest();
			Game.particlesLight.Rest();

			Sound.StopOne(Game.CollectionID.sound_newhero);

			Render.portraitCameraOffsetAspectFix = true;
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(!isSetted)
			{
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
					w.paddingHeight = w.paddingWidth = 6;
					window.Add(w);

					GUIElement element;
					w.Add(element = new GUIText(Game.TEXT.GotANewHero, Game.GUIStyle.MediumFont, percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter));
				}

				{
					GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
					w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
					w.SetAlign(GUIWindow.Align.MiddleCenter);
					w.noClip = true;
					window.Add(w);

					GUIElement element;
					w.Add(element = new GUIElement(height: 8 * 16));
					w.Add(element = heroName = new GUIText("", Game.GUIStyle.MediumFont));
				}

				{
					GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
					w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
					w.SetAlign(GUIWindow.Align.BottomCenter);
					w.paddingHeight = 6;
					w.noClip = true;
					window.Add(w);

					GUIElement element;
					w.Add(element = new GUIButton(Game.ButtonID.Continue, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/play"), new GUIText(Game.TEXT.Continue) }), Game.GUIStyle.Button, defaultFocus: true));
				}

				GUI.Add(window);

				Entity2D entity = (Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][heroID];
				entity.Rest();
				entity.SetPos(Vector2.V(0, 0));
				entity.Enable(true);
				entity.Update();
				GameObjectAnimation goa = entity.model.GetGameObjectAnimation();
				goa.goAnimation = new GOAnimationZRotation(60);

				heroName.SetText(Game.TEXT.HeroName0 + heroID + 1);

				Game.particlesCoins.Rest();
				Game.particlesCoins.SetTexture("gui/images/coins/coin" + (heroID + 1).ToString("000"));
				Game.particlesCoins.SetPos(Vector2.V(0, -1), 1);
				Game.particlesCoins.Play();

				Game.particlesLight.Rest();
				Game.particlesLight.SetPos(Vector2.V(-Fixed.OneHalf, 2), -Fixed.OneHalf);
				Game.particlesLight.Play();

				Sound.Play(Game.CollectionID.sound_newhero);

				Render.portraitCameraOffsetAspectFix = false;
				Render.SetCameraZoom(3);
				Render.SetCameraPos(Vector2.V(0, 0), 1, 10);

				isSetted = true;
				return;
			}

			if(GUI.buttonPushed != null)
			{
				switch(GUI.buttonPushed.buttonID)
				{
					case Game.ButtonID.Continue:
					pda.Pop(this);
					return;
				}
			}
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GameStateLoading : GameState
	{
		int task = 0;

		GUIWindow window;

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(Game.IsAnyCoroutines())
			{
				Console.WriteLine("Loading...");
				return;
			}

			if(task == 0)
				LoadPersistentResources();
			else if(task == 1)
				LoadResources();
			else if(task == 2)
				LoadDependedResources();
			else
			{
				if(Sound.IsPlayingOne(Game.CollectionID.sound_hexlogo1)) return;

				pda.Pop(this);
				pda.Push(new PushdownAutomata.TransitionState(new GameStateMainMenu()));
				pda.Push(new GameStateRestartLevel());

				//pda.Push(new GameStateTracker());
			}

			task++;
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);
			GUI.SetOptimalScale();
		}

		void LoadPersistentResources()
		{
			Render.LoadGUISkin("Game");
			Render.PreloadTextures("hexplay", "circle2", "circle25", "circle50");
		}

		void LoadResources()
		{
			window = new GUIWindow(style: Game.GUIStyle.Block);
			window.SetAlign(GUIWindow.Align.MiddleCenter);
			window.SetHeightLayout(GUIWindow.HeightLayout.PercentHeight);
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth);
			window.backColor = Color.Dark;

			int min = Screen.width < Screen.height ? Screen.width : Screen.height;
			if(min >= 1024) window.Add(new GUIImage("textures/hexplay-herocraft-1024", animation: new GUIAnimationBaseColor(3.0f, Color.C(255, 255, 255, 0))));
			else window.Add(new GUIImage("textures/hexplay-herocraft-512", animation: new GUIAnimationBaseColor(3.0f, Color.C(255, 255, 255, 0))));

			GUI.Add(window);
			GUI.SetScale(1, true);

			Sound.PreloadSound(
				Game.CollectionID.sound_gba_gba01,
				Game.CollectionID.sound_gba_gba02,
				Game.CollectionID.sound_gba_gba03,
				Game.CollectionID.sound_gba_gba04,
				Game.CollectionID.sound_gba_gba05,
				Game.CollectionID.sound_gba_gba06,
				Game.CollectionID.sound_gba_gba07,
				Game.CollectionID.sound_gba_gba08,
				Game.CollectionID.sound_gba_gba09,
				Game.CollectionID.sound_gba_gba10,
				Game.CollectionID.sound_gba_gba11,
				Game.CollectionID.sound_gba_gba12,
				Game.CollectionID.sound_gba_gba13,
				Game.CollectionID.sound_gba_gba14,
				Game.CollectionID.sound_gba_gba15,
				Game.CollectionID.sound_gba_gba16);

			Sound.PreloadSound(
				Game.CollectionID.sound_click,
				Game.CollectionID.sound_select);

			Sound.PlaySound(Game.CollectionID.sound_hexlogo1, Fixed.OneHalf, 1);

			Render.PreloadParticles(
				Game.CollectionID.particles_snow,
				Game.CollectionID.particles_hit,
				Game.CollectionID.particles_avalanche,
				Game.CollectionID.particles_snow_trail,
				Game.CollectionID.particles_coins,
				Game.CollectionID.particles_light,
				Game.CollectionID.particles_pickup,
				Game.CollectionID.particles_explosion);
		}

		void LoadDependedResources()
		{
			Game.SetupCollection();
		}
	}
}
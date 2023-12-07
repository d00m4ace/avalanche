using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateRestartLevel : GameState
	{
		bool isRestarted = false;

		public override void OnEnter(PushdownAutomata pda) { }

		public override void OnExit(PushdownAutomata pda) { }

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(Game.IsAnyCoroutines())
				return;

			if(!isRestarted)
			{
				Game.Rest();

				int heroesCount = Game.collection.elementsPool[Game.ObjectType.Skier].Count;

				if(Game.isReplay)
				{
					Game.savedGameActions.SetPos(0);

					Game.savedGameActions.Read(out Game.random.seed[0]);
					Game.savedGameActions.Read(out Game.random.seed[1]);

					Game.savedGameActions.Read(out LevelGenerator.slopeRandom.seed[0]);
					Game.savedGameActions.Read(out LevelGenerator.slopeRandom.seed[1]);

					Game.savedGameActions.Read(out Game.settings.heroSelected);
					Game.savedGameActions.Read(out heroesCount);

					Game.saveJoystickActions = false;
				}
				else
				{
					Game.savedGameActions.Rest();

					Game.savedGameActions.Write(Game.random.seed[0]);
					Game.savedGameActions.Write(Game.random.seed[1]);

					Game.savedGameActions.Write(LevelGenerator.slopeRandom.seed[0]);
					Game.savedGameActions.Write(LevelGenerator.slopeRandom.seed[1]);

					Game.savedGameActions.Write(Game.settings.heroSelected);
					Game.savedGameActions.Write(heroesCount);

					Game.saveJoystickActions = true;

					Game.newHighScores = null;
				}

				LevelGenerator.GenerateRandomSkierList(heroesCount);

				Space2D space = World2D.GetSpace2D(1);

				Entity2D entity = Game.player = (Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][Game.settings.heroSelected - 1];
				entity.Rest();
				entity.SetPos(Vector2.V(0, 5));
				entity.Enable(true);
				entity.Update();

				space.Add(entity);

				Game.particlesSnow.Rest();
				Game.particlesSnow.Play();

				Render.SetCameraAngles(30f, -45f / 3, 0f);
				Render.SetCameraPos(Game.player.pos, 0, 10);

				Game.maxHitCount = 0;
				Game.isPlayLevel = false;

				isRestarted = true;

				startup.hasUnpaused = false;

				return;
			}

			if(LevelGenerator.slopeCount < 6)
				LevelGenerator.Update(false);
			else
				pda.Pop(this);
		}
	}
}
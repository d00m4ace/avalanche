using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public partial class Game
	{
		public static Collection collection;

		public static InputController inputController;

		public static Stopwatch updateStopwatch;
		public static long updateLastElapsedTicks;

		public static void RestTime()
		{
			if(updateStopwatch == null)
			{
				updateStopwatch = new Stopwatch();
				updateStopwatch.Start();
			}

			updateStopwatch.Reset();
			updateStopwatch.Start();

			updateLastElapsedTicks = updateStopwatch.ElapsedTicks;
		}

		public static ulong worldIterations = 6;
		public static ulong iteration = 0;

		public static PseudoRandom random;

		public static GamePDA gamePDA;

		public static Dictionary<TEXT, string> text;

		public static bool IsAnyCoroutines()
		{
#if !SERVER
			return startup.coroutinesCount != 0;
#else
			return false;
#endif
		}

		public static void PreloadGameObjects(ObjectType collection, params CollectionID[] objects)
		{
			for(int i = 0; i < objects.Length; i++)
			{
				Game.collection.Get<Entity2D>(collection, objects[i]).elementUsed = false;
			}
		}

		public static void PreloadGameObjects(ObjectType collection, CollectionID startID, CollectionID endID)
		{
			for(int i = (int)startID; i <= (int)endID; i++)
			{
				Game.collection.Get<Entity2D>(collection, (CollectionID)i).elementUsed = false;
			}
		}

		public static Entity2D CreateBoxGameObject(CollectionID modelID, ObjectType objectType, Entity2D.Type type, Fixed? width = null, Fixed? height = null, Vector2 offset = default(Vector2), Action<Model> onLoad = null)
		{
			Model model = Render.CreateModel(modelID, onLoad);

			Entity2D entity = World2D.CreateBox(type, Vector2.V(0, 0), width.HasValue ? width.Value : 1, height.HasValue ? height.Value : 1, offset);
			model.Link(entity);
			entity.objectType = objectType;
			return entity;
		}

		public static Entity2D CreateCircleGameObject(CollectionID modelID, ObjectType objectType, Entity2D.Type type, Fixed? radius, Vector2 offset = default(Vector2), Action<Model> onLoad = null)
		{
			Model model = Render.CreateModel(modelID, onLoad);

			Entity2D entity = World2D.CreateCircle(type, Vector2.V(0, 0), radius.HasValue ? radius.Value : 1, offset);
			model.Link(entity);
			entity.objectType = objectType;
			return entity;
		}

		public static MemoryBuffer savedGameActions;
		public static bool saveJoystickActions;
		public static bool isReplay;

		public static void GameUpdate()
		{
			long time = updateStopwatch.ElapsedTicks;

			if(time - updateLastElapsedTicks > Stopwatch.Frequency * 1)
			{
				RestTime();
				return;
			}

			while(updateLastElapsedTicks < time)
			{
				if(World2D.iteration % worldIterations == 0)
				{
#if !SERVER
					InputManager.Update(inputController);
#endif
					gamePDA.OnGameUpdate();
					Simulate();
				}

				if(updateLastElapsedTicks < time)
				{
					World2D.Simulate();
					long dt = Stopwatch.Frequency / World2D.simulationsPerSecond;
					updateLastElapsedTicks += dt;
				}
			}
		}

		public static void Simulate()
		{
			Update();
			iteration++;
		}

		public static void Update()
		{
			UpdateDynamicActorsInSpacies();
		}

		public static void UpdateDynamicActorsInSpacies()
		{
			for(int i = 1; i < World2D.spacies.NextID; i++)
			{
				if(World2D.spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				int jc = World2D.spacies[i].dynamicEntities.Count;
				for(int j = 0; j < jc; j++)
				{
					if(World2D.spacies[i].dynamicEntities[j].actor != null)
						World2D.spacies[i].dynamicEntities[j].actor.OnUpdateGameLogic();
				}
			}
		}
	}
}
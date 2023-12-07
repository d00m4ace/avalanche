#if false
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using GameLogic;
using FixedPoint;
using Math = FixedPoint.Math;

namespace Universe2D
{
	public struct World2DTest
	{
		public static GameLogic.Game gameLogic;

		public static Entity2D player;

		public static Actor playerActor;

		public static Ray2D ray = new Ray2D();

		public static void Setup()
		{
			gameLogic = new GameLogic.Game();

			World2D world2d = gameLogic.world2d;

			{
				Space2D space = world2d.CreateSpace2D();

				Fixed offset = -(Fixed)5;

				space.Add(player = world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(offset, 0), 1).AddPhysicBody(1));

				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(0, 0), 1, 1));

				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset, 0), 1, 1));

				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(2 + offset, 2), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(3 + offset, 3), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(1 + offset, 1), 1).AddPhysicBody(1));

				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(-1 + offset, -1), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(-2 + offset, -2), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(-3 + offset, -3), 1).AddPhysicBody(1));

				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset, 5), 10, 1));
				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset, -5), 10, 1));
				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset * 2, 0), 1, 12));

				space.Add(world2d.CreateBox(Entity2D.Type.Ground, Vector2.V(offset, 0), 10, 12));

				Actor actor = new Actor();
				actor.Link(player);
				actor.Add(new AgentJoystickInput());
			}

			{
				Space2D space = world2d.CreateSpace2D();

				Fixed offset = (Fixed)5;

				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset, 0), 1, 1));

				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(2 + offset, 2), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(3 + offset, 3), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(1 + offset, 1), 1).AddPhysicBody(1));

				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(-1 + offset, -1), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(-2 + offset, -2), 1).AddPhysicBody(1));
				space.Add(world2d.CreateCircle(Entity2D.Type.Dynamic, Vector2.V(-3 + offset, -3), 1).AddPhysicBody(1));

				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset, 5), 10, 1));
				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset, -5), 10, 1));
				space.Add(world2d.CreateBox(Entity2D.Type.Static, Vector2.V(offset * 2, 0), 1, 12));

				space.Add(world2d.CreateBox(Entity2D.Type.Ground, Vector2.V(offset, 0), 10, 12));
			}
		}

		public static void IntersectRay()
		{
			ray.Set(player.pos, player.dir, Fixed.MaxValue);
			World2DTest.gameLogic.world2d.IntersectRay(Entity2D.Type.Dynamic, ray, x => x == player, (e, r) => { return false; });
			World2DTest.gameLogic.world2d.IntersectRay(Entity2D.Type.Static, ray, x => x == player, (e, r) => { return false; });
		}
	}
}
#endif
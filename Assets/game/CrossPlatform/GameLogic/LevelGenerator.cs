using System;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class LevelGenerator
	{
		public static Fixed slopeVisibleY, slopeHeight = (Fixed)128 / 10;
		public static Vector2 slopeLastGeneratorCenter;
		public static PseudoRandom slopeRandom;
		public static int slopeCount;

		public static List<Entity2D> slopeEntities;

		public static List<Entity2D> randomSkierList;

		public static void Setup()
		{
			slopeRandom = new PseudoRandom();
			slopeEntities = new List<Entity2D>();
		}

		public static void Rest()
		{
			slopeVisibleY = 0;
			slopeLastGeneratorCenter = Vector2.V(0, 2 * slopeHeight);

			slopeRandom.Rest();

			slopeCount = 0;
			slopeLeftRockX = 0;
			slopeRightRockX = 0;
			slopeLeftRockDir = 1;
			slopeLeftRockDirCount = 1;
			slopeRightRockDir = 1;
			slopeRightRockDirCount = 1;
			slopeTreesCount = 0;

			int ic = slopeEntities.Count;
			for(int i = 0; i < ic; i++)
				World2D.Delete(slopeEntities[i]);
			slopeEntities.Clear();

			routineGenerateSlope = null;

			MakeLevelColors();
		}

		public static void MakeLevelColors()
		{
			List<Color> colors = new List<Color>() { Color.White, Color.Silver, Color.LightSkyBlue, Color.Snow, Color.Dimgray, Color.DarkCyan, Color.Lavender, };

			{
				colors.Shuffle(Game.random);

				Entity2D entity = Game.collection.Get<Entity2D>(Game.ObjectType.Rock, Game.CollectionID.rock_01);
				entity.model.SetSharedMaterialColor("color_", colors[0]);
				entity.elementUsed = false;

				entity = Game.collection.Get<Entity2D>(Game.ObjectType.Rock, Game.CollectionID.rock_02);
				entity.model.SetSharedMaterialColor("color_", colors[0]);
				entity.elementUsed = false;
			}

			{
				Entity2D entity = Game.collection.Get<Entity2D>(Game.ObjectType.Ground, Game.CollectionID.ground_terrain);
				colors.Shuffle(Game.random);
				entity.model.SetSharedMaterialColor("color_", colors[0]);
				entity.elementUsed = false;
				colors.RemoveAt(0);
			}

			{
				Entity2D entity = Game.collection.Get<Entity2D>(Game.ObjectType.Tree, Game.CollectionID.tree_01);
				colors.Shuffle(Game.random);
				entity.model.SetSharedMaterialColor("Matrix_", colors[0]);
				entity.elementUsed = false;
				colors.RemoveAt(0);

				entity = Game.collection.Get<Entity2D>(Game.ObjectType.Tree, Game.CollectionID.tree_02);
				colors.Shuffle(Game.random);
				entity.model.SetSharedMaterialColor("Matrix_", colors[0]);
				entity.elementUsed = false;
				colors.RemoveAt(0);

				entity = Game.collection.Get<Entity2D>(Game.ObjectType.Tree, Game.CollectionID.tree_03);
				colors.Shuffle(Game.random);
				entity.model.SetSharedMaterialColor("Matrix_", colors[0]);
				entity.elementUsed = false;
				colors.RemoveAt(0);
			}
		}

		public static void GenerateRandomSkierList(int heroesCount)
		{
			randomSkierList = new List<Entity2D>();

			for(int i = 0; i < Game.collection.elementsPool[Game.ObjectType.Skier].Count; i++)
				randomSkierList.Add((Entity2D)Game.collection.elementsPool[Game.ObjectType.Skier][i]);

			if(randomSkierList.Count > heroesCount)
				randomSkierList.RemoveRange(heroesCount, randomSkierList.Count - heroesCount);

			ShuffleSkierList();
		}

		public static void ShuffleSkierList()
		{
			Utils.Shuffle(randomSkierList, slopeRandom);
		}

		public static Entity2D GetNextFreeRandomSkier()
		{
			for(int i = 0; i < randomSkierList.Count; i++)
			{
				if(randomSkierList[i].flags.Has(Entity2D.Flags.Disabled))
					return randomSkierList[i];
			}

			return null;
		}

		public static Entity2D GetNextLiveRandomSkier()
		{
			for(int i = 0; i < randomSkierList.Count; i++)
			{
				if(!randomSkierList[i].flags.Has(Entity2D.Flags.Disabled))
				{
					AgentSkier playerAgent = randomSkierList[i].actor.GetAgent<AgentSkier>();

					if(playerAgent.isDead)
						continue;

					return randomSkierList[i];
				}
			}

			return null;
		}

		public static void FreeSlope(Fixed freeY, bool freeAll = false)
		{
			Space2D space = World2D.GetSpace2D(1);

			List<Entity2D> entitiesToFree = new List<Entity2D>();

			Action<List<Entity2D>> checkFromStart = (entities) =>
			{
				int jc = entities.Count;
				for(int j = 0; j < jc; j++)
				{
					if(entities[j].pos.y < freeY)
						break;

					entitiesToFree.Add(entities[j]);
				}
			};

			checkFromStart(space.GetEntities(Entity2D.Type.Dynamic));
			checkFromStart(space.GetEntities(Entity2D.Type.Ground));
			checkFromStart(space.GetEntities(Entity2D.Type.Static));

			int ic = entitiesToFree.Count;
			for(int i = 0; i < ic; i++)
			{
				if(!freeAll)
				{
					if(entitiesToFree[i].objectType == Game.ObjectType.Avalanche)
						continue;
				}

				World2D.Delete(entitiesToFree[i]);
			}
		}

		public static IEnumerator GenerateSlope()
		{
			Space2D space = World2D.GetSpace2D(1);

			Fixed width = (Fixed)128 / 10;
			Fixed height = (Fixed)128 / 10;

			if(slopeCount == 0)
			{
				GenerateAvalanche(space, slopeLastGeneratorCenter);
				yield return 0;
			}

			GenerateGround(space, slopeLastGeneratorCenter);
			yield return 0;

			GenerateRock(slopeLastGeneratorCenter);
			yield return 0;

			if(slopeCount > 2)
			{
				Vector2 c = slopeLastGeneratorCenter, ext = Vector2.V(width * 7 / 2 - 1, height / 2 - 1);
				AABB2D aabb = AABB2D.Rest;
				aabb.SetExtents(c, ext);

				if(slopeCount % 2 == 0)
				{
					GenerateCoins(aabb);
					yield return 0;
				}

				GenerateTrees(aabb);
				yield return 0;
			}

			if(slopeCount > 6)
			{
				Vector2 c = slopeLastGeneratorCenter, ext = Vector2.V(width * 7 / 2 - 1, 1);
				c.y += slopeHeight / 2 - 2;

				AABB2D aabb = AABB2D.Rest;
				aabb.SetExtents(c, ext);

				GenerateBot(aabb);
				yield return 0;
			}

			slopeEntities.Sort((a, b) => (int)(b.aabb.t.raw - a.aabb.t.raw));

			int ic = slopeEntities.Count;
			for(int i = 0; i < ic; i++)
				space.Add(slopeEntities[i]);

			slopeEntities.Clear();

			slopeLastGeneratorCenter.y -= slopeHeight;
			slopeCount++;
		}

		public static IEnumerator routineGenerateSlope = null;

		public static void Update(bool doFree = true)
		{
			if(routineGenerateSlope != null)
			{
				if(!routineGenerateSlope.MoveNext())
					routineGenerateSlope = null;

				return;
			}

			if(Game.player.pos.y - slopeLastGeneratorCenter.y < slopeHeight * 4)
			{
				if(doFree)
					FreeSlope(Game.player.pos.y + 7 * slopeHeight / 2);

				routineGenerateSlope = GenerateSlope();
			}
		}

		public static void GenerateGround(Space2D space, Vector2 center)
		{
			Fixed width = (Fixed)128 / 10;
			Fixed height = (Fixed)128 / 10;

			Action<Vector2, Vector2, Fixed> genGround = (pos, dir, z) =>
			{
				Entity2D entity = Game.collection.Get<Entity2D>(Game.ObjectType.Ground, Game.CollectionID.ground_terrain);
				entity.Rest();
				entity.SetPos(pos);
				entity.SetZ(z);
				entity.SetDir(dir);
				entity.Enable(true);
				entity.Update();
				space.Add(entity);
			};

			for(int y = 0; y < 1; y++)
			{
				genGround(center, -Vector2.YAxis, 0);
				center.y -= (Fixed)128 / 10;
			}
		}

		public static void GenerateAvalanche(Space2D space, Vector2 center)
		{
			Fixed width = (Fixed)128 / 10;

			for(int x = -4 * 2; x < 4 * 2; x++)
			{
				Entity2D entity = Game.collection.Get<Entity2D>(Game.ObjectType.Avalanche, Game.CollectionID.misc_avalanche);
				entity.Rest();

				Vector2 pos = center + Vector2.V(x * width / 2, 16);

				entity.SetPos(pos);
				entity.SetDir(-Vector2.YAxis);
				entity.Enable(true);

				entity.Update();
				space.Add(entity);
			}
		}

		public static int slopeLeftRockDir;
		public static int slopeLeftRockDirCount;
		public static int slopeLeftRockX;

		public static int slopeRightRockX;
		public static int slopeRightRockDir;
		public static int slopeRightRockDirCount;

		public static void GenerateRock(Vector2 center)
		{
			Fixed width = (Fixed)128 / 10;
			Fixed height = (Fixed)128 / 10;

			center.y += height / 2 - (Fixed)32 / 10;

			Action<Entity2D, Vector2, Vector2, Fixed> genRock = (entity, pos, dir, z) =>
			{
				entity.Rest();
				entity.SetPos(pos);
				entity.SetZ(z);
				entity.SetDir(dir);
				entity.Enable(true);
				entity.Update();
				slopeEntities.Add(entity);
			};

			for(int y = 0; y < 2; y++)
			{
				Vector2 pos = center + Vector2.V(-3 * width - width / 2 + (Fixed)slopeLeftRockX * 8 / 10, 0);
				Vector2 dir = Vector2.Zero;
				dir = Vector2.Polar(0);
				genRock((y == 0) ? Game.collection.Get<Entity2D>(Game.ObjectType.Rock, Game.CollectionID.rock_01) : Game.collection.Get<Entity2D>(Game.ObjectType.Rock, Game.CollectionID.rock_02), pos, dir, 0);

				slopeLeftRockX += slopeLeftRockDir;
				if(slopeLeftRockDirCount-- == 0)
				{
					slopeLeftRockDirCount = 1 + slopeRandom.Random(5);
					slopeLeftRockDir = Math.Sin(pos.y / 10) > 0 ? 1 : -1; //slopeRandom.Random(100) > 50 ? 1 : -1;
				}
				if(slopeLeftRockX < 0) slopeLeftRockX = 0;
				else if(slopeLeftRockX > 32) slopeLeftRockX = 32;

				pos = center + Vector2.V(3 * width + width / 2 - (Fixed)slopeRightRockX * 8 / 10, 0);
				dir = Vector2.Polar(Math.PI);
				genRock((y == 0) ? Game.collection.Get<Entity2D>(Game.ObjectType.Rock, Game.CollectionID.rock_01) : Game.collection.Get<Entity2D>(Game.ObjectType.Rock, Game.CollectionID.rock_02), pos, dir, 0);

				slopeRightRockX += slopeRightRockDir;
				if(slopeRightRockDirCount-- == 0)
				{
					slopeRightRockDirCount = 1 + slopeRandom.Random(5);
					slopeRightRockDir = Math.Sin(pos.y / 10) > 0 ? 1 : -1; //slopeRandom.Random(100) > 50 ? 1 : -1;
				}
				if(slopeRightRockX < 0) slopeRightRockX = 0;
				else if(slopeRightRockX > 32) slopeRightRockX = 32;

				center.y -= (Fixed)64 / 10;
			}
		}

		public static void GenerateCoins(AABB2D aabb)
		{
			Fixed width = aabb.Width, height = aabb.Height;
			Fixed widthHalf = width / 2, heightHalf = height / 2;
			Vector2 center = aabb.Center, border = Vector2.V(1, 1);

			AABB2D box = AABB2D.Rest;

			Func<Vector2, Vector2, bool, bool> genCoins = (pos, dir, generate) =>
			{
				for(int i = 0; i < 10; i++)
				{
					Vector2 p = pos + Vector2.V(Math.Sin(i * Math.PI / 4), -i).Rotate(dir);

					if(!generate)
					{
						box.SetExtents(p, border);

						int jc = slopeEntities.Count;
						for(int j = 0; j < jc; j++)
							if(box.Intersect(ref slopeEntities[j].aabb))
								return false;
					}

					if(generate)
					{
						Entity2D entity = Game.collection.Get<Entity2D>(Game.ObjectType.Item, Game.CollectionID.item_coin);
						entity.Rest();
						entity.SetPos(p);
						entity.Enable(true);
						entity.Update();

						GameObjectAnimation goa = entity.model.GetGameObjectAnimation();
						goa.goAnimation = new GOAnimationFilter(
							new GOAnimationZRotation(180),
							new GOAnimationZposition((float)((Fixed)7 / 20 + 7 * Math.Sin(2 * i * Math.PI / 10) / 20), 1, 0, 0.7f)
							);

						slopeEntities.Add(entity);
					}
				}
				return true;
			};

			for(int i = 0; i < 1; i++)
			{
				int loopCounter = 0;
				while(true)
				{
					Vector2 pos = center + Vector2.V(widthHalf - slopeRandom.RandomFixed((int)(width * 10)) * width, heightHalf - slopeRandom.RandomFixed((int)(height * 10)) * height / 4);
					Vector2 dir = Vector2.Zero;
					dir = Vector2.Polar(Math.PI / 4 - Math.PI / 2 * slopeRandom.RandomFixed(180));

					if(genCoins(pos, dir, false))
					{
						genCoins(pos, dir, true);
						break;
					}
					if(loopCounter++ > 50) break;
				}
			}
		}

		public static int slopeTreesCount = 0;

		public static void GenerateTrees(AABB2D aabb)
		{
			Fixed width = aabb.Width, height = aabb.Height;
			Fixed widthHalf = width / 2, heightHalf = height / 2;
			Vector2 center = aabb.Center, border = Vector2.V(4, 4);

			int treesCount = slopeTreesCount / 3 + slopeRandom.Random(1 + (2 * slopeTreesCount) / 3);

			for(int i = 0; i < treesCount; i++)
			{
				int loopCounter = 0;
				while(true)
				{
					Vector2 pos = center + Vector2.V(widthHalf - slopeRandom.RandomFixed((int)(width * 10)) * width, heightHalf - slopeRandom.RandomFixed((int)(height * 10)) * height);
					AABB2D box = AABB2D.Rest;
					box.SetExtents(pos, border);

					bool hasIntersect = false;

					int jc = slopeEntities.Count;
					for(int j = 0; j < jc; j++)
						if(box.Intersect(ref slopeEntities[j].aabb))
						{
							hasIntersect = true;
							break;
						}

					if(!hasIntersect)
					{
						Game.CollectionID[] trees = { Game.CollectionID.tree_01, Game.CollectionID.tree_02, Game.CollectionID.tree_03 };
						Entity2D entity = Game.collection.Get<Entity2D>(Game.ObjectType.Tree, trees[slopeRandom.Random(3)]);
						entity.Rest();
						entity.SetPos(pos);
						entity.SetAngle(Math.PIDouble * slopeRandom.RandomFixed(4));
						entity.Enable(true);
						entity.Update();
						slopeEntities.Add(entity);
						break;
					}
					if(loopCounter++ > 50) break;
				}
			}

			if(slopeCount % 2 == 0)
				slopeTreesCount++;

			if(slopeTreesCount > 12)
				slopeTreesCount = 12;
		}

		public static void GenerateBot(AABB2D aabb)
		{
			Fixed width = aabb.Width, height = aabb.Height;
			Fixed widthHalf = width / 2, heightHalf = height / 2;
			Vector2 center = aabb.Center, border = Vector2.V(2, 2);

			if(GetNextFreeRandomSkier() == null)
				return;

			for(int i = 0; i < 1; i++)
			{
				int loopCounter = 0;
				while(true)
				{
					Vector2 pos = center + Vector2.V(widthHalf - slopeRandom.RandomFixed((int)(width * 10)) * width, heightHalf - slopeRandom.RandomFixed((int)(height * 10)) * height);
					AABB2D box = AABB2D.Rest;
					box.SetExtents(pos, border);

					bool hasIntersect = false;

					int jc = slopeEntities.Count;
					for(int j = 0; j < jc; j++)
						if(box.Intersect(ref slopeEntities[j].aabb))
						{
							hasIntersect = true;
							break;
						}

					if(!hasIntersect)
					{
						Entity2D entity = GetNextFreeRandomSkier();
						entity.Rest();

						entity.SetPos(pos);
						entity.SetAngle(Math.PI / 4 - Math.PI / 2 * slopeRandom.RandomFixed(180));

						entity.Enable(true);
						entity.Update();

						AgentSkier agent = entity.actor.GetAgent<AgentSkier>();
						agent.isBot = true;

						slopeEntities.Add(entity);
						break;
					}
					if(loopCounter++ > 50) break;
				}
			}
		}
	}
}

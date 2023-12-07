using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class World2D
	{
		public static Entity2D[] ignoreEntityNone = { };
		public static Entity2D[] ignoreEntityOne = { null };

		public static Game.ObjectType[] ignoreObjectTypeNone = { };
		public static Game.ObjectType[] ignoreObjectTypeOne = { Game.ObjectType.None };
		public static Game.ObjectType[] ignoreObjectTypeTwo = { Game.ObjectType.None, Game.ObjectType.None };
		public static Game.ObjectType[] ignoreObjectTypeThree = { Game.ObjectType.None, Game.ObjectType.None, Game.ObjectType.None };

		public static List<Entity2D> intersects;

		public static Utils.Array<Space2D> spacies;
		public static Utils.Array<Entity2D> entities;
		public static Collision2D collision;

		public static List<Entity2D> deleted;

		public static Dictionary<int, Entity2DProxy> entityProxies;
		public static Dictionary<int, Space2DProxy> spaceProxies;

		public const int simulationsPerSecond = 60;

#if !SERVER
		public const float secondsPerSimulations = 1.0f / simulationsPerSecond;
#else
		public const double secondsPerSimulations = 1.0 / simulationsPerSecond;
#endif

		public static Fixed dt = (Fixed)1 / simulationsPerSecond;
		public static ulong iteration = 0;

		public static void Setup()
		{
			collision = new Collision2D();

			entities = new Utils.Array<Entity2D>();
			spacies = new Utils.Array<Space2D>();

			deleted = new List<Entity2D>();

			intersects = new List<Entity2D>();

			entityProxies = new Dictionary<int, Entity2DProxy>();
			spaceProxies = new Dictionary<int, Space2DProxy>();
		}

		public static void Delete(Space2D space)
		{
			if(space.flags.Has(Space2D.Flags.Deleted))
				return;

			space.DeleteAllEntities();

			DeleteDeletedEntities();

			space.flags |= Space2D.Flags.Deleted | Space2D.Flags.Disabled;

#if !SERVER
			if(space.proxy != null)
			{
				FreeEntity2DProxy(space.proxy.id);
			}
#endif
		}

		public static void Delete(Entity2D entity)
		{
			if(entity.flags.Has(Entity2D.Flags.Delete))
				return;

			entity.flags |= Entity2D.Flags.Delete;

			deleted.Add(entity);
		}

		public static void DeleteDeletedEntities()
		{
			int ic = deleted.Count;
			for(int i = 0; i < ic; i++)
			{
				Space2D space = GetSpace2D(deleted[i].spaceID);

				if(deleted[i].actor != null)
					deleted[i].actor.OnDelete();

				if(space != null)
					space.Remove(deleted[i]);

				deleted[i].Enable(false);
				deleted[i].Update();

				deleted[i].flags &= ~Entity2D.Flags.Delete;

				deleted[i].elementUsed = false;

#if !SERVER
				if(deleted[i].proxy != null)
				{
					FreeEntity2DProxy(deleted[i].proxy.id);
				}
#endif
			}

			deleted.Clear();
		}

		public static void DeleteAllEntities()
		{
			for(int i = 1; i < spacies.NextID; i++)
				spacies[i].DeleteAllEntities();

			DeleteDeletedEntities();
		}

		public static void Rest()
		{
			iteration = 0;
			DeleteAllEntities();
		}

		public static Space2D GetSpace2D(int id) { return spacies[id]; }
		public static Space2D CreateSpace2D()
		{
			Space2D space = null;

			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Deleted))
				{
					space = spacies[i];
					break;
				}
			}

			if(space == null)
			{
				space = spacies.AddNext(new Space2D(spacies.CreateNextID()));
			}

			space.Rest();

			return space;
		}

		public static Entity2D GetEntity2D(int id) { return entities[id]; }
		public static Entity2D CreateEntity2D(Entity2D.Type type, Vector2 pos, Fixed angle)
		{
			return entities.AddNext(new Entity2D(entities.CreateNextID(), type, pos, angle));
		}

		public static Entity2DProxy GetEntity2DProxy(int id)
		{
			if(entityProxies.ContainsKey(id))
				return entityProxies[id];
			return null;
		}
		public static Entity2DProxy CreateEntity2DProxy(int id)
		{
			entityProxies[id] = new Entity2DProxy(id);
			return entityProxies[id];
		}
		public static void FreeEntity2DProxy(int id)
		{
			if(entityProxies.ContainsKey(id))
			{
				entityProxies[id].UnLink();
				entityProxies.Remove(id);
			}
		}

		public static Space2DProxy GetSpace2DProxy(int id)
		{
			if(spaceProxies.ContainsKey(id))
				return spaceProxies[id];
			return null;
		}
		public static Space2DProxy CreateSpace2DProxy(int id)
		{
			spaceProxies[id] = new Space2DProxy(id);
			return spaceProxies[id];
		}
		public static void FreeSpace2DProxy(int id)
		{
			if(spaceProxies.ContainsKey(id))
			{
				spaceProxies[id].UnLink();
				spaceProxies.Remove(id);
			}
		}

		public static Entity2D CreateCircle(Entity2D.Type type, Vector2 pos, Fixed radius, Vector2 offset = default(Vector2))
		{
			Entity2D entity = CreateEntity2D(type, pos, 0);
			entity.AddCircleShape(offset, radius);
			entity.Rebuild();
			return entity;
		}

		public static Entity2D CreateBox(Entity2D.Type type, Vector2 pos, Fixed width, Fixed height, Vector2 offset = default(Vector2))
		{
			Entity2D entity = CreateEntity2D(type, pos, 0);
			entity.AddBoxShape(offset, width, height, 0);
			entity.Rebuild();
			return entity;
		}

		public static Fixed gridWidth;
		public static Fixed gridHeight;

		public static int gridXSize;
		public static int gridYSize;

		public static bool isGrid;

		public static void CreateGrid(int xSize, int ySize, Fixed width, Fixed height)
		{
			isGrid = true;

			gridWidth = width;
			gridHeight = height;

			gridXSize = xSize;
			gridYSize = ySize;

			Fixed spaceWidth = gridWidth / gridXSize;
			Fixed spaceHeight = gridHeight / gridYSize;

			for(int y = 0; y < gridYSize; y++)
				for(int x = 0; x < gridXSize; x++)
				{
					Space2D space = CreateSpace2D();
					space.aabb.SetExtents(Vector2.V(spaceWidth * x + spaceWidth / 2, spaceHeight * y + spaceHeight / 2), Vector2.V(spaceWidth / 2, spaceHeight / 2));
					space.fixedAABB = true;
				}
		}

		public static Space2D SpaceContainsPoint(Vector2 point)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				if(!spacies[i].aabb.ContainsPoint(point))
					continue;

				return spacies[i];
			}

			return null;
		}

		public static void Simulate()
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				spacies[i].Simulate();
			}

			collision.contactPool.FreeAll();
			CheckDynamicEntitiesContacts(Entity2D.Type.Dynamic);
			CheckDynamicEntitiesContacts(Entity2D.Type.Static);
			collision.ResolveContacts();
			collision.ResolveCollisions();

			TransferDynamicEntitiesToSpacies();

			DeleteDeletedEntities();

			Update();
			iteration++;

			//Console.WriteLine("Iteration Game:{0} World:{1}", Game.iteration, World2D.iteration);
		}

		public static void Update()
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				spacies[i].Update();
			}
		}

		public static void CheckDynamicEntitiesContacts(Entity2D.Type type)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				int jc = spacies[i].dynamicEntities.Count;
				for(int j = 0; j < jc; j++)
				{
					if(spacies[i].dynamicEntities[j].flags.Has(Entity2D.Flags.Disabled | Entity2D.Flags.Unstoppable))
						continue;

					collision.CheckContacts(spacies[i].GetEntities(type), spacies[i].dynamicEntities[j]);

					for(int k = 1; k < spacies.NextID; k++)
					{
						if(i == k || spacies[k].flags.Has(Space2D.Flags.Disabled))
							continue;

						if(spacies[k].aabb.Intersect(ref spacies[i].dynamicEntities[j].aabb))
							collision.CheckContacts(spacies[k].GetEntities(type), spacies[i].dynamicEntities[j]);
					}
				}
			}
		}

		public static void TransferDynamicEntitiesToSpacies()
		{
			{
				int spaciesCount = 0;

				for(int i = 1; i < spacies.NextID; i++)
				{
					if(spacies[i].flags.Has(Space2D.Flags.Disabled))
						continue;

					spaciesCount++;
				}

				if(spaciesCount < 2)
					return;
			}

			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				int jc = spacies[i].dynamicEntities.Count;
				for(int j = 0; j < jc; j++)
				{
					Entity2D entity = spacies[i].dynamicEntities[j];

					if(entity.flags.Has(Entity2D.Flags.Disabled | Entity2D.Flags.NoTransfer))
						continue;

					Entity2D e = spacies[i].ContainsPoint(Entity2D.Type.Ground, entity.pos, ignoreEntityNone, ignoreObjectTypeNone);

					if(e != null)
						continue;

					for(int k = 1; k < spacies.NextID; k++)
					{
						if(i == k || spacies[k].flags.Has(Space2D.Flags.Disabled))
							continue;

						if(spacies[k].aabb.Intersect(ref entity.aabb))
						{
							e = spacies[k].ContainsPoint(Entity2D.Type.Ground, entity.pos, ignoreEntityNone, ignoreObjectTypeNone);

							if(e != null)
							{
								spacies[i].Remove(entity);
								spacies[k].Add(entity);
								jc--; j--;
								break;
							}
						}
					}
				}
			}
		}

		public static Entity2D ContainsPoint(Entity2D.Type type, Vector2 point, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				if(!spacies[i].aabb.ContainsPoint(point))
					continue;

				Entity2D entity = spacies[i].ContainsPoint(type, point, ignoreEntity, ignoreGameObjectType);

				if(entity != null)
					return entity;
			}

			return null;
		}

		public static bool IntersectRay(Entity2D.Type type, Ray2D ray, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				if(spacies[i].IntersectRay(type, ray, ignoreEntity, ignoreGameObjectType))
					return true;
			}

			return false;
		}

		public static bool IntersectAABB(Entity2D.Type type, AABB2D aabb, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				if(spacies[i].IntersectAABB(type, aabb, ignoreEntity, ignoreGameObjectType))
					return true;
			}

			return false;
		}

		public static void FindAllIntersectRay(Entity2D.Type type, Ray2D ray, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType, List<Entity2D> intersects)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				FindAllIntersectRay(type, ray, ignoreEntity, ignoreGameObjectType, intersects);
			}
		}

		public static void FindAllIntersectAABB(Entity2D.Type type, AABB2D aabb, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType, List<Entity2D> intersects)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				spacies[i].FindAllIntersectAABB(type, aabb, ignoreEntity, ignoreGameObjectType, intersects);
			}
		}

		public static void FindAllContainsPoint(Entity2D.Type type, Vector2 point, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType, List<Entity2D> intersects)
		{
			for(int i = 1; i < spacies.NextID; i++)
			{
				if(spacies[i].flags.Has(Space2D.Flags.Disabled))
					continue;

				if(!spacies[i].aabb.ContainsPoint(point))
					continue;

				Entity2D entity = spacies[i].ContainsPoint(type, point, ignoreEntity, ignoreGameObjectType);
			}
		}
	}
}
using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class Space2D
	{
		public readonly int id;

		public Space2DProxy proxy;

		public long restElapsedTicks;

		public List<Entity2D> groundEntities;
		public List<Entity2D> staticEntities;
		public List<Entity2D> dynamicEntities;

		List<Entity2D>[] entities;

		public AABB2D aabb;
		public bool fixedAABB;

		public int flags;

		public abstract class Flags
		{
			public const int None = 0,
			GroundDirty = 1,
			StaticDirty = 1 << 1,
			DynamicDirty = 1 << 2,
			Disabled = 1 << 3,
			NotVisible = 1 << 4,
			Deleted = 1 << 5;
		}

		public List<Entity2D> GetEntities(Entity2D.Type type)
		{
			return entities[(int)type - 1];
		}

		public Space2D(int id)
		{
			this.id = id;
			groundEntities = new List<Entity2D>();
			staticEntities = new List<Entity2D>();
			dynamicEntities = new List<Entity2D>();
			entities = new List<Entity2D>[] { groundEntities, staticEntities, dynamicEntities };

#if SERVER
			World2D.CreateSpace2DProxy(id).Link(this);
#endif
		}

		public void Rest()
		{
			flags = Flags.None;

			restElapsedTicks = Game.updateStopwatch.ElapsedTicks;

			if(proxy != null)
			{
				proxy.Rest();
			}
		}

		public void Add(Entity2D entity)
		{
			GetEntities(entity.type).Add(entity);
			entity.spaceID = id;
			flags |= (1 << ((int)(entity.type - 1)));
		}

		public void Remove(Entity2D entity)
		{
			GetEntities(entity.type).Remove(entity);
			entity.spaceID = 0;
			flags |= (1 << ((int)(entity.type - 1)));
		}

		public bool IntersectRay(Entity2D.Type type, Ray2D ray, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType)
		{
			List<Entity2D> entites = GetEntities(type);

			int ic = entites.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entites[i].flags.Has(Entity2D.Flags.Disabled | Entity2D.Flags.IgnoreIntersectRay))
					continue;

				bool ignore = false;

				for(int j = 0; j < ignoreEntity.Length; j++)
					if(entites[i] == ignoreEntity[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				for(int j = 0; j < ignoreGameObjectType.Length; j++)
					if(entites[i].objectType == ignoreGameObjectType[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				if(ray.aabb.t < entites[i].aabb.b)
					continue;

				if(ray.aabb.b > entites[i].aabb.t)
					return false;

				entites[i].IntersectRay(ray);

				if(ray.entity == entites[i])
					return true;
			}

			return false;
		}

		public void FindAllIntersectRay(Entity2D.Type type, Ray2D ray, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType, List<Entity2D> intersects)
		{
			List<Entity2D> entites = GetEntities(type);

			int ic = entites.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entites[i].flags.Has(Entity2D.Flags.Disabled | Entity2D.Flags.IgnoreIntersectRay))
					continue;

				bool ignore = false;

				for(int j = 0; j < ignoreEntity.Length; j++)
					if(entites[i] == ignoreEntity[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				for(int j = 0; j < ignoreGameObjectType.Length; j++)
					if(entites[i].objectType == ignoreGameObjectType[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				if(ray.aabb.t < entites[i].aabb.b)
					continue;

				if(ray.aabb.b > entites[i].aabb.t)
					break;

				entites[i].IntersectRay(ray);

				if(ray.entity == entites[i])
					intersects.Add(entites[i]);
			}
		}

		public bool IntersectAABB(Entity2D.Type type, AABB2D aabb, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType)
		{
			List<Entity2D> entites = GetEntities(type);

			int ic = entites.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entites[i].flags.Has(Entity2D.Flags.Disabled))
					continue;

				bool ignore = false;

				for(int j = 0; j < ignoreEntity.Length; j++)
					if(entites[i] == ignoreEntity[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				for(int j = 0; j < ignoreGameObjectType.Length; j++)
					if(entites[i].objectType == ignoreGameObjectType[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				if(aabb.t < entites[i].aabb.b)
					continue;

				if(aabb.b > entites[i].aabb.t)
					return false;

				if(entites[i].aabb.Intersect(ref aabb))
				{
					return true;
				}
			}

			return false;
		}

		public void FindAllIntersectAABB(Entity2D.Type type, AABB2D aabb, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType, List<Entity2D> intersects)
		{
			List<Entity2D> entites = GetEntities(type);

			int ic = entites.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entites[i].flags.Has(Entity2D.Flags.Disabled))
					continue;

				bool ignore = false;

				for(int j = 0; j < ignoreEntity.Length; j++)
					if(entites[i] == ignoreEntity[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				for(int j = 0; j < ignoreGameObjectType.Length; j++)
					if(entites[i].objectType == ignoreGameObjectType[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				if(aabb.t < entites[i].aabb.b)
					continue;

				if(aabb.b > entites[i].aabb.t)
					break;

				if(entites[i].aabb.Intersect(ref aabb))
					intersects.Add(entites[i]);
			}
		}


		public Entity2D ContainsPoint(Entity2D.Type type, Vector2 point, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType)
		{
			List<Entity2D> entites = GetEntities(type);

			int ic = entites.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entites[i].flags.Has(Entity2D.Flags.Disabled))
					continue;

				bool ignore = false;

				for(int j = 0; j < ignoreEntity.Length; j++)
					if(entites[i] == ignoreEntity[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				for(int j = 0; j < ignoreGameObjectType.Length; j++)
					if(entites[i].objectType == ignoreGameObjectType[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				if(point.y < entites[i].aabb.b)
					continue;

				if(point.y > entites[i].aabb.t)
					return null;

				if(entites[i].ContainsPoint(point))
					return entites[i];
			}

			return null;
		}

		public void FindAllContainsPoint(Entity2D.Type type, Vector2 point, Entity2D[] ignoreEntity, Game.ObjectType[] ignoreGameObjectType, List<Entity2D> intersects)
		{
			List<Entity2D> entites = GetEntities(type);

			int ic = entites.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entites[i].flags.Has(Entity2D.Flags.Disabled))
					continue;

				bool ignore = false;

				for(int j = 0; j < ignoreEntity.Length; j++)
					if(entites[i] == ignoreEntity[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				for(int j = 0; j < ignoreGameObjectType.Length; j++)
					if(entites[i].objectType == ignoreGameObjectType[j])
					{
						ignore = true;
						break;
					}

				if(ignore)
					continue;

				if(point.y < entites[i].aabb.b)
					continue;

				if(point.y > entites[i].aabb.t)
					break;

				if(entites[i].ContainsPoint(point))
					intersects.Add(entites[i]);
			}
		}

		public void SortEntities(Entity2D.Type type)
		{
			GetEntities(type).Sort((a, b) => (int)(b.aabb.t.raw - a.aabb.t.raw));
		}

		public void Simulate()
		{
			int ic = dynamicEntities.Count;
			for(int i = 0; i < ic; i++)
			{
				if(dynamicEntities[i].flags.Has(Entity2D.Flags.Disabled))
					continue;

				Entity2D groundEnity = ContainsPoint(Entity2D.Type.Ground, dynamicEntities[i].pos, World2D.ignoreEntityNone, World2D.ignoreObjectTypeNone);
				dynamicEntities[i].lastGroundEntityID = groundEnity != null ? groundEnity.id : 0;

				dynamicEntities[i].Simulate();
			}

			Update();
		}

		public void Update()
		{
			if(flags.Has(Flags.GroundDirty | Flags.StaticDirty))
				UpdateAABB();

			if(flags.Has(Flags.GroundDirty))
				SortEntities(Entity2D.Type.Ground);

			if(flags.Has(Flags.StaticDirty))
				SortEntities(Entity2D.Type.Static);

			if(flags.Has(Flags.DynamicDirty))
				SortEntities(Entity2D.Type.Dynamic);

#if SERVER
			if(flags.Has(Flags.GroundDirty))
				proxy.groundDirtyElapsedTicks = Game.updateStopwatch.ElapsedTicks;

			if(flags.Has(Flags.StaticDirty))
				proxy.staticDirtyElapsedTicks = Game.updateStopwatch.ElapsedTicks;

			if(flags.Has(Flags.DynamicDirty))
				proxy.dynamicDirtyElapsedTicks = Game.updateStopwatch.ElapsedTicks;
#endif

			flags &= ~(Flags.GroundDirty | Flags.StaticDirty | Flags.DynamicDirty);
		}

		public void UpdateAABB()
		{
			if(!fixedAABB)
			{
				aabb = AABB2D.Rest;

				int ic = groundEntities.Count;
				for(int i = 0; i < ic; i++)
					aabb.Add(ref groundEntities[i].aabb);

				ic = staticEntities.Count;
				for(int i = 0; i < ic; i++)
					aabb.Add(ref staticEntities[i].aabb);
			}
		}

		public void DeleteAllEntities()
		{
			int ic = groundEntities.Count;
			for(int i = 0; i < ic; i++)
				World2D.Delete(groundEntities[i]);

			ic = staticEntities.Count;
			for(int i = 0; i < ic; i++)
				World2D.Delete(staticEntities[i]);

			ic = dynamicEntities.Count;
			for(int i = 0; i < ic; i++)
				World2D.Delete(dynamicEntities[i]);
		}
	}
}
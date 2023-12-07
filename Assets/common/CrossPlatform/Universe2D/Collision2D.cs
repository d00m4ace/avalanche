using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class Collision2D
	{
		public ObjectPool<Entity2DContact> contactPool;

		public Collision2D()
		{
			contactPool = new ObjectPool<Entity2DContact>(() => new Entity2DContact());
		}

		public bool CheckContacts(List<Entity2D> entitesA, List<Entity2D> entitesB)
		{
			bool entityHasContacts = false;

			int ic = entitesB.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entitesB[i].flags.Has(Entity2D.Flags.Disabled))
					continue;

				entityHasContacts = CheckContacts(entitesA, entitesB[i]) ? true : entityHasContacts;
			}

			return entityHasContacts;
		}

		public bool CheckContacts(List<Entity2D> entites, Entity2D entity)
		{
			bool entityHasContacts = false;

			int ic = entites.Count;
			for(int i = 0; i < ic; i++)
			{
				if(entites[i].flags.Has(Entity2D.Flags.Disabled))
					continue;

				if(entites[i] == entity)
					continue;

				if(entity.aabb.b > entites[i].aabb.t)
					return entityHasContacts;

				if(entity.aabb.t < entites[i].aabb.b)
					continue;

				if(!entity.HasZOverlap(entites[i]))
					continue;

				bool alredyHasContact = false;

				int jc = contactPool.usedCount;
				for(int j = 0; j < jc; j++)
					if(contactPool.Used(j).a == entity && contactPool.Used(j).b == entites[i])
					{
						entityHasContacts = alredyHasContact = true; break;
					}

				if(alredyHasContact)
					continue;

				entityHasContacts = CheckContacts(entites[i], entity) ? true : entityHasContacts;
			}

			return entityHasContacts;
		}

		public bool CheckContacts(Entity2D a, Entity2D b)
		{
			if(a.flags.Has(Entity2D.Flags.Projectile) && b.flags.Has(Entity2D.Flags.Projectile))
				return false;

			if(a.type == Entity2D.Type.Dynamic && b.flags.Has(Entity2D.Flags.IgnoreDynamic))
				return false;

			if(b.type == Entity2D.Type.Dynamic && a.flags.Has(Entity2D.Flags.IgnoreDynamic))
				return false;

			if(a.type == Entity2D.Type.Static && b.flags.Has(Entity2D.Flags.IgnoreStatic))
				return false;

			if(a.aabb.Intersect(ref b.aabb))
			{
				Entity2DContact c = contactPool.Get();
				c.FindContacts(a, b);

				if(c.contactsCount == 0)
				{
					contactPool.Free(c);
					return false;
				}

				return true;
			}

			return false;
		}

		public void ResolveContacts()
		{
			int ic = contactPool.usedCount;
			for(int i = 0; i < ic; i++)
			{
				Contact2D c = contactPool.Used(i).Median();

				if(!contactPool.Used(i).a.flags.Has(Entity2D.Flags.DoNotResolveContact) && !contactPool.Used(i).b.flags.Has(Entity2D.Flags.DoNotResolveContact))
					contactPool.Used(i).a.ResolveContact(ref c, contactPool.Used(i).b);

				contactPool.Used(i).a.OnContactEntity(ref c, contactPool.Used(i).b);
				c.axis.n = -c.axis.n;
				contactPool.Used(i).b.OnContactEntity(ref c, contactPool.Used(i).a);
			}
		}

		public void ResolveCollisions()
		{
			int ic = contactPool.usedCount;
			for(int i = 0; i < ic; i++)
			{
				Contact2D c = contactPool.Used(i).Median();

				if(contactPool.Used(i).a.flags.Has(Entity2D.Flags.DoNotResolveCollision) || contactPool.Used(i).b.flags.Has(Entity2D.Flags.DoNotResolveCollision))
					continue;

				if(!contactPool.Used(i).a.flags.Has(Entity2D.Flags.Unstoppable))
					contactPool.Used(i).a.ResolveCollision(ref c);
				c.axis.n = -c.axis.n;
				if(!contactPool.Used(i).b.flags.Has(Entity2D.Flags.Unstoppable))
					contactPool.Used(i).b.ResolveCollision(ref c);
			}
		}
	}
}
using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class Entity2DProxy
	{
		public readonly int id;

		public Entity2D entity;

		public long restElapsedTicks;

		public const int MAX_TRANSFORMS = 5;
		public Entity2DTransform[] transform;

		public Entity2DProxy(int id)
		{
			this.id = id;

			transform = new Entity2DTransform[MAX_TRANSFORMS];

			transform[0].pos = Vector2.One;
		}

		public void ShiftTransforms()
		{
			for(int i = MAX_TRANSFORMS - 1; i > 0; i--)
				transform[i] = transform[i - 1];
		}

		public void Link(Entity2D entity)
		{
			this.entity = entity;
			entity.proxy = this;
		}

		public void UnLink()
		{
			if(entity != null)
			{
				entity.proxy = null;
				entity = null;
			}
		}

		public void Rest()
		{
			restElapsedTicks = Game.updateStopwatch.ElapsedTicks;
		}
	}

	public struct Entity2DTransform
	{
		public int proxyId;
		public Game.ObjectType objectType;
		public Game.CollectionID elementID;
		public Vector2 pos, vel;
		public Fixed angle, rot;
		public Vector2 dir;
		public Fixed z;

		public Game.CollectionID animation;

		public ulong iteration;

		public void Write(MemoryBuffer mb)
		{
			mb.Write(proxyId);
			mb.Write(objectType);
			mb.Write(elementID);
			mb.Write(pos);
			mb.Write(vel);
			mb.Write(angle);
			mb.Write(rot);
			mb.Write(dir);
			mb.Write(z);
			mb.Write(animation);
		}

		public bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out proxyId))
				return false;

			if(!mb.Read(out objectType))
				return false;

			if(!mb.Read(out elementID))
				return false;

			if(!mb.Read(out pos))
				return false;

			if(!mb.Read(out vel))
				return false;

			if(!mb.Read(out angle))
				return false;

			if(!mb.Read(out rot))
				return false;

			if(!mb.Read(out dir))
				return false;

			if(!mb.Read(out z))
				return false;

			if(!mb.Read(out animation))
				return false;

			iteration = World2D.iteration;

			return true;
		}
	}
}
using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class Space2DProxy
	{
		public readonly int id;

		public Space2D space;

		public long restElapsedTicks;

		public long groundDirtyElapsedTicks;
		public long staticDirtyElapsedTicks;
		public long dynamicDirtyElapsedTicks;

		public Space2DProxy(int id)
		{
			this.id = id;
		}

		public void Link(Space2D space)
		{
			this.space = space;
			space.proxy = this;
		}

		public void UnLink()
		{
			if(space != null)
			{
				space.proxy = null;
				space = null;
			}
		}

		public void Rest()
		{
			groundDirtyElapsedTicks = staticDirtyElapsedTicks = dynamicDirtyElapsedTicks = restElapsedTicks = Game.updateStopwatch.ElapsedTicks;
		}
	}

	public struct Space2DTransform
	{
		public int proxyId;

		public long restElapsedTicks;

		public AABB2D aabb;

		public long groundDirtyElapsedTicks;
		public long staticDirtyElapsedTicks;
		public long dynamicDirtyElapsedTicks;

		public void Write(MemoryBuffer mb)
		{
			mb.Write(proxyId);
			mb.Write(restElapsedTicks);
			mb.Write(aabb);
			mb.Write(groundDirtyElapsedTicks);
			mb.Write(staticDirtyElapsedTicks);
			mb.Write(dynamicDirtyElapsedTicks);
		}

		public bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out proxyId))
				return false;

			if(!mb.Read(out restElapsedTicks))
				return false;

			if(!mb.Read(out aabb))
				return false;

			if(!mb.Read(out groundDirtyElapsedTicks))
				return false;

			if(!mb.Read(out staticDirtyElapsedTicks))
				return false;

			if(!mb.Read(out dynamicDirtyElapsedTicks))
				return false;

			return true;
		}
	}
}
using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class ObjectPool<T> where T : class
	{
		Stack<T> pool;
		List<T> used;

		public int freeCount { get { return pool.Count; } }
		public int usedCount { get { return used.Count; } }

		public T Used(int i) { return used[i]; }

		public Func<T> NewT;
		int growth;

		public ObjectPool(Func<T> newT, int growth = 10)
		{
			pool = new Stack<T>();
			used = new List<T>();
			this.NewT = newT;
			this.growth = growth;
			Growth();
		}

		void Growth()
		{
			for(int i = 0; i < growth; i++)
				pool.Push(NewT());
		}

		public T Get()
		{
			if(pool.Count == 0)
				Growth();

			T t;
			used.Add(t = pool.Pop());
			return t;
		}

		public void Free(T t)
		{
			used.Remove(t);
			pool.Push(t);
		}

		public void FreeAll()
		{
			while(used.Count > 0)
				Free(used[0]);
		}
	}
}
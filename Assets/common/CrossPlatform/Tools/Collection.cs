using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HEXPLAY
{
	public class Collection
	{
		public class Element
		{
			public Game.CollectionID elementID;
			public bool elementUsed;
		}

		public Dictionary<Game.ObjectType, List<Element>> elementsPool;
		public Dictionary<Game.ObjectType, Func<Game.CollectionID, Element>> elementsSetup;

		public Collection()
		{
			elementsPool = new Dictionary<Game.ObjectType, List<Element>>();
			elementsSetup = new Dictionary<Game.ObjectType, Func<Game.CollectionID, Element>>();
		}

		public void Setup(Game.ObjectType collection, Func<Game.CollectionID, Element> collectionElementSetup)
		{
			elementsPool[collection] = new List<Element>();
			elementsSetup[collection] = collectionElementSetup;
		}

		public T Get<T>(Game.ObjectType collection, Game.CollectionID elementID) where T : Element
		{
			T t = null;

			List<Element> elements = elementsPool[collection];
			int count = elements.Count;
			for(int i = 0; i < count; i++)
			{
				if(!elements[i].elementUsed && elements[i].elementID == elementID)
				{
					t = (T)elements[i];
					break;
				}
			}

			if(t == null)
			{
				t = (T)elementsSetup[collection](elementID);
				t.elementID = elementID;
				elementsPool[collection].Add(t);
			}

			t.elementUsed = true;

			return t;
		}

		public void FreeAll(Game.ObjectType collection)
		{
			List<Element> elements = elementsPool[collection];
			int count = elements.Count;
			for(int i = 0; i < count; i++)
			{
				elements[i].elementUsed = false;
			}
		}
	}
}
using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public abstract class Agent
	{
		public virtual void OnRest(Actor actor) { }
		public virtual void OnDelete(Actor actor) { }

		public virtual void OnUpdateGameLogic(Actor actor) { }
		public virtual void OnUpdateWorld(Actor actor) { }

		public virtual void OnContactEntity(Actor actor, Contact2D contact, Entity2D entity) { }
	}
}
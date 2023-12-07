using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class Actor
	{
		public List<Agent> agents;

		public Entity2D entity;

		public int flags;

		public abstract class Flags
		{
			public const int None = 0,
			Disabled = 1 << 0;
		}

		public Actor()
		{
			agents = new List<Agent>();
		}

		public virtual void Add(Agent agent) { agents.Add(agent); }
		public virtual void Remove(Agent agent) { agents.Remove(agent); }

		public void Link(Entity2D entity)
		{
			this.entity = entity;
			entity.actor = this;
		}

		public T GetAgent<T>() where T : Agent
		{
			for(int i = 0; i < agents.Count; i++)
				if(agents[i] is T)
					return (T)agents[i];
			return null;
		}

		public virtual void OnRest()
		{
			for(int i = 0; i < agents.Count; i++)
				agents[i].OnRest(this);
		}

		public virtual void OnDelete()
		{
			for(int i = 0; i < agents.Count; i++)
				agents[i].OnDelete(this);
		}

		public virtual void OnUpdateGameLogic()
		{
			for(int i = 0; i < agents.Count; i++)
				agents[i].OnUpdateGameLogic(this);
		}

		public virtual void OnUpdateWorld()
		{
			for(int i = 0; i < agents.Count; i++)
				agents[i].OnUpdateWorld(this);
		}

		public virtual void OnContactEntity(Contact2D contact, Entity2D entity)
		{
			for(int i = 0; i < agents.Count; i++)
				agents[i].OnContactEntity(this, contact, entity);
		}
	}
}
using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class PushdownAutomata
	{
		public abstract class State
		{
			public bool passThrough = false;

			public virtual void OnEnter(PushdownAutomata pda) { }
			public virtual void OnExit(PushdownAutomata pda) { }
			public virtual void OnUpdate(PushdownAutomata pda) { }
		}

		public class TransitionState : State
		{
			State state;

			public TransitionState(State state) { this.state = state; }

			public override void OnUpdate(PushdownAutomata pda) { pda.Pop(this); pda.Push(state); }
		}

		public List<State> states;
		public bool isStatesChanged;

		public PushdownAutomata()
		{
			states = new List<State>();
		}

		public void OnUpdate()
		{
			isStatesChanged = false;

			for(int i = states.Count - 1; i >= 0; i--)
			{
				states[i].OnUpdate(this);

				if(isStatesChanged)
					break;

				if(!states[i].passThrough)
					break;
			}
		}

		public void Push(State state)
		{
			isStatesChanged = true;
			states.Add(state);
			state.OnEnter(this);
		}

		public void Pop(State state)
		{
			isStatesChanged = true;
			states.Remove(state);
			state.OnExit(this);
		}

		public void PopAll()
		{
			isStatesChanged = true;

			while(states.Count > 0)
				Pop(Peek());
		}

		public State Peek() { return states.Count != 0 ? states[states.Count - 1] : null; }

		public T GetState<T>() where T : State
		{
			for(int i = 0; i < states.Count; i++)
				if(states[i] is T)
					return (T)states[i];
			return null;
		}
	}
}
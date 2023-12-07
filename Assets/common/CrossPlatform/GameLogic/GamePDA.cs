using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public abstract class GameState : PushdownAutomata.State
	{
		public virtual void OnGameUpdate(PushdownAutomata pda) { }
	}

	public class GamePDA : PushdownAutomata
	{
		public GamePDA()
		{
		}

		public void OnGameUpdate()
		{
			for(int i = states.Count - 1; i >= 0; i--)
			{
				((GameState)states[i]).OnGameUpdate(this);

				if(isStatesChanged)
					break;

				if(!states[i].passThrough)
					break;
			}
		}
	}
}
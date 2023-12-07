using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateRewardedVideoAd : GameState
	{
		public override void OnEnter(PushdownAutomata pda)
		{
			//Ads.ShowRewardedVideoAd();
		}

		public override void OnExit(PushdownAutomata pda)
		{
			//Ads.LoadRewardedVideoAd();
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			//if(Ads.IsRewardedVideoAdClosed())
				pda.Pop(this);
		}
	}
}
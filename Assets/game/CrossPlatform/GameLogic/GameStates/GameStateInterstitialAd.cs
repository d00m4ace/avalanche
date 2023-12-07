using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateInterstitialAd : GameState
	{
		bool isShowed = false;
		float timeStart = 0;

		public override void OnEnter(PushdownAutomata pda)
		{
			timeStart = UnityEngine.Time.time;
		}

		public override void OnExit(PushdownAutomata pda)
		{
			//Ads.LoadInterstitialAd();
		}

		bool isClosed = false;
		float timeClosed = 0;

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(!isShowed)
			{
				if(UnityEngine.Time.time - timeStart > 2)
				{
					//Ads.ShowInterstitialAd();
					isShowed = true;
				}

				return;
			}

			if(!isClosed)// && Ads.IsInterstitialAdClosed())
			{
				isClosed = true;
				timeClosed = UnityEngine.Time.time;
			}
			else if(isClosed && UnityEngine.Time.time - timeClosed > 3)
			{
				pda.Pop(this);
			}
		}
	}
}
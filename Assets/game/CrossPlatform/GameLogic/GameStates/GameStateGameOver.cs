using System;
using System.Text;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameStateGameOver : GameState
	{
		GUIWindow window;

		GUICounterInteger scoreGUICounter;
		GUICounterInteger distanceGUICounter;
		GUICounterInteger timeGUICounter;
		GUICounterInteger coinsGUICounter;

		int counter = 0;
		GUIElement[] countersLabel;
		GUICounterInteger[] counters;

		HEXInt coins, score, distance;
		Fixed time;
		int coinsScore, distanceScore, timeScore;

		int highScoresPlace;
		GUIElement textHighScore;

		public GameStateGameOver()
		{
			passThrough = true;
		}

		public override void OnEnter(PushdownAutomata pda)
		{
			{
				AgentSkier playerAgent = Game.player.actor.GetAgent<AgentSkier>();

				score = playerAgent.score;
				coins = playerAgent.coins;
				distance = playerAgent.distance;
				time = playerAgent.time;

				distanceScore = distance * 2;
				timeScore = ((int)time) * 10;
				coinsScore = coins * 50;

				HEXInt totalScore = score + distanceScore + timeScore + coinsScore;

				//VideoRecording.SetReplayMetadata("distance", distance.ToString());
				//VideoRecording.SetReplayMetadata("score", totalScore.ToString());

				highScoresPlace = 1000000;

				if(!Game.isReplay)
				{
					if(Game.settings.distance < distance)
						Game.settings.distance = distance;

					Game.settings.coins += coins;

					HeroSet.Save(Game.heroSet);
					Game.SaveSettings();
				}

				highScoresPlace = HighScores.FindPlace(Game.highScores, totalScore);

				if(Game.newHighScores == null && !Game.isReplay)
				{
					if(highScoresPlace < 10 && Game.newHighScores == null)
					{
						Game.newHighScores = new HighScores();
						Game.newHighScores.socre = totalScore;
						Game.newHighScores.heroID = Game.settings.heroSelected;
						Game.newHighScores.name = Game.settings.playerName;
						Game.newHighScores.replayFileName = Utils.GetDateTimeStamp() + ".replay";
						HighScores.Add(Game.highScores, Game.newHighScores);

/*
						if(Social.IsSignIn())
						{
							Social.SubmitScore(Game.leaderBoardId, totalScore);

							bool unlockAchievement = false;

							if(!Game.settings.achievement1000Points && totalScore >= 1000)
							{
								Social.UnlockAchievement(Game.achievement1000PointsID);
								Game.settings.achievement1000Points = true;
								unlockAchievement = true;
							}

							if(!Game.settings.achievement2000Points && totalScore >= 2000)
							{
								Social.UnlockAchievement(Game.achievement2000PointsID);
								Game.settings.achievement2000Points = true;
								unlockAchievement = true;
							}

							if(!Game.settings.achievement5000Points && totalScore >= 5000)
							{
								Social.UnlockAchievement(Game.achievement5000PointsID);
								Game.settings.achievement5000Points = true;
								unlockAchievement = true;
							}

							if(!Game.settings.achievement10000Points && totalScore >= 10000)
							{
								Social.UnlockAchievement(Game.achievement10000PointsID);
								Game.settings.achievement10000Points = true;
								unlockAchievement = true;
							}

							if(!Game.settings.achievement50000Points && totalScore >= 50000)
							{
								Social.UnlockAchievement(Game.achievement50000PointsID);
								Game.settings.achievement50000Points = true;
								unlockAchievement = true;
							}

							if(unlockAchievement)
								Game.SaveSettings();
						}
*/						

						Sound.PlayOne(Game.CollectionID.sound_highscore, 1, 1, false);
					}
				}
			}

			window = new GUIWindow(style: Game.GUIStyle.Empty);
			window.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
			window.SetHeightLayout(GUIWindow.HeightLayout.PercentHeight, 100);
			window.SetAlign(GUIWindow.Align.BottomCenter);
			window.focusWindow = true;

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Block);
				w.SetAlign(GUIWindow.Align.MiddleCenter);
				w.SetContentLayout(GUIWindow.ContentLayout.Vertical);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.backColor.a = 128;
				w.spacing = 6;
				w.SetShadowBackTexture("gui/images/bar");
				w.paddingHeight = 10;
				w.paddingWidth = 10;
				w.noClip = true;
				w.animation = new GUIAnimationSize(0.35f, int.MinValue, 0);
				window.Add(w);


				GUIElement element;
				w.Add(element = new GUIText(Game.TEXT.GameOver, Game.GUIStyle.BigFont, animation: new GUIAnimationText(1), percentWidth: 90, heightTextLines: 3, align: GUIText.Align.MiddleCenter));

				w.Add(textHighScore = new GUIText(Game.TEXT.NewHighScorePlace.GetText() + (highScoresPlace + 1).ToString(), Game.GUIStyle.MediumFont, percentWidth: 90, heightTextLines: 2, align: GUIText.Align.MiddleCenter));
				textHighScore.animation = new GUIAnimationFilter(new GUIAnimation[] { new GUIAnimationXY(0.65f, GUI.GetScreenWidth()), new GUIAnimationBlinkBaseColor(0.5f, Color.Yellow, false, true) });
				textHighScore.isDisabled = highScoresPlace >= 10 || Game.isReplay;

				countersLabel = new GUIElement[4];
				counters = new GUICounterInteger[4];

				w.Add(element = new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.GameScreenScore), scoreGUICounter = new GUICounterInteger(0, score, 300, 100) }, Game.GUIStyle.MediumFont, minWidth: 300));
				countersLabel[0] = element; element.animation = new GUIAnimationXY(0.65f, GUI.GetScreenWidth()); element.isDisabled = true;
				w.Add(element = new GUILabel(new GUIElement[] { new GUIText(distance.ToString()), new GUIText(Game.TEXT.MeterShort), new GUIText(" x2 "), distanceGUICounter = new GUICounterInteger(0, distanceScore, 300, 100) }, Game.GUIStyle.MediumFont, minWidth: 300));
				countersLabel[1] = element; element.animation = new GUIAnimationXY(0.65f, GUI.GetScreenWidth()); element.isDisabled = true;
				w.Add(element = new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/clock"), new GUIText(Utils.TimeToMMSS(time)), new GUIText(" x10 "), timeGUICounter = new GUICounterInteger(0, timeScore, 300, 100) }, Game.GUIStyle.MediumFont, minWidth: 300));
				countersLabel[2] = element; element.animation = new GUIAnimationXY(0.65f, GUI.GetScreenWidth()); element.isDisabled = true;
				w.Add(element = new GUILabel(new GUIElement[] { new GUIImage("gui/images/game/coin"), new GUIText(coins.ToString()), new GUIText(" x50 "), coinsGUICounter = new GUICounterInteger(0, coinsScore, 300, 100) }, Game.GUIStyle.MediumFont, minWidth: 300));
				countersLabel[3] = element; element.animation = new GUIAnimationXY(0.65f, GUI.GetScreenWidth()); element.isDisabled = true;

				w.Add(element = new GUIElement());

				counters[0] = scoreGUICounter;
				counters[1] = distanceGUICounter;
				counters[2] = timeGUICounter;
				counters[3] = coinsGUICounter;

				distanceGUICounter.playSound = false;
				timeGUICounter.playSound = false;
				coinsGUICounter.playSound = false;
			}

			{
				GUIWindow w = new GUIWindow(style: Game.GUIStyle.Empty);
				w.SetWidthLayout(GUIWindow.WidthLayout.PercentWidth, 100);
				w.SetAlign(GUIWindow.Align.BottomCenter);
				w.paddingHeight = 6;
				w.paddingWidth = 6;
				w.spacing = 6;
				w.noClip = true;
				window.Add(w);

				GUIElement element;
				w.Add(element = new GUIButton(Game.ButtonID.Continue, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/play"), new GUIText(Game.TEXT.Continue) }), Game.GUIStyle.Button));
				w.Add(element = new GUIButton(Game.ButtonID.Play, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/repeat"), new GUIText(Game.TEXT.Play) }), Game.GUIStyle.Button, defaultFocus: true));
//				if(Game.settings.videoRecOn && VideoRecording.IsReadyForRecording())
//					w.Add(element = new GUIButton(Game.ButtonID.Share, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/video_share"), new GUIText(Game.TEXT.Share) }), Game.GUIStyle.Button));
//				else
//					w.Add(element = new GUIButton(Game.ButtonID.Replay, new GUILabel(new GUIElement[] { new GUIImage("gui/images/icons/camera"), new GUIText(Game.TEXT.Replay) }), Game.GUIStyle.Button));
			}

			GUI.Add(window);
		}

		public override void OnExit(PushdownAutomata pda)
		{
			GUI.Remove(window);

			pda.Pop(pda.GetState<GameStatePlayLevel>());

			Sound.StopOne(Game.CollectionID.sound_highscore);

			Game.isReplay = false;
		}

		public override void OnUpdate(PushdownAutomata pda)
		{
			if(window.animation.IsPlaying())
				return;

			if(GUI.isAnyKeyDown)
				GUI.EndAnyAnimations();

			if(GUI.buttonPushed != null)
			{
				switch(GUI.buttonPushed.buttonID)
				{
					case Game.ButtonID.Play:
					if(Game.newHighScores != null)
					{
						HighScores.RemoveAllBelowN(Game.highScores, 10);
						HighScores.Save("/local", Game.highScores);
						HighScores.SaveReplay("/local", Game.newHighScores.replayFileName, Game.savedGameActions);
						Game.SaveSettings();
						Game.newHighScores = null;
					}
					pda.Pop(this);
					pda.Push(new PushdownAutomata.TransitionState(new GameStatePlayLevel()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateRestartLevel()));
					//if(!Game.settings.hasIAP && Ads.IsInterstitialAdReady()) pda.Push(new GameStateInterstitialAd());
					return;

					case Game.ButtonID.Continue:
					pda.Pop(this);
					HeroSet.AddGameStateGotNewHero(pda);
					if(Game.newHighScores != null) pda.Push(new PushdownAutomata.TransitionState(new GameStateEnterName()));
					//if(!Game.settings.hasIAP && Ads.IsInterstitialAdReady()) pda.Push(new GameStateInterstitialAd());
					return;

					case Game.ButtonID.Replay:
					pda.Pop(this);
					Game.isReplay = true;
					pda.Push(new PushdownAutomata.TransitionState(new GameStatePlayLevel()));
					pda.Push(new PushdownAutomata.TransitionState(new GameStateRestartLevel()));
					//if(!Game.settings.hasIAP && Ads.IsInterstitialAdReady()) pda.Push(new GameStateInterstitialAd());
					return;

					case Game.ButtonID.Share:
					//VideoRecording.ShowSharingModal();
					return;
				}
			}

			if(countersLabel[counter].isDisabled)
			{
				countersLabel[counter].isDisabled = false;
				return;
			}

			if(counters[counter].IsNotStarted())
			{
				counters[counter].StartCounting();

				if(counter > 0)
				{
					scoreGUICounter.Set(scoreGUICounter.end, scoreGUICounter.end + counters[counter].end);
					scoreGUICounter.StartCounting();
				}

				return;
			}

			if(counters[counter].isCounting)
				return;

			if(counter < 3)
			{
				counter++;
				return;
			}
		}
	}
}
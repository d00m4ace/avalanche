using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public interface GUICounter
	{
		bool isCounting { get; set; }

		bool IsNotStarted();

		void StartCounting();

		void StopCounting();
	}

	public class GUICounterInteger : GUIText, GUICounter
	{
		public bool isCounting { get; set; }

		public int start, end, speed, boost;
		public float current, currentSpeed;

		float lastTimePlaySound = 0;
		public Game.CollectionID counterSound = Game.CollectionID.sound_gba_gba01;
		public bool playSound = true;

		public GUICounterInteger(int start, int end, int speed, int boost, Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int percentWidth = 0, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(start.ToString(), style, animation, width, height, percentWidth, minWidth, minHeight, maxWidth, maxHeight)
		{
			current = this.start = start;
			this.end = end;
			currentSpeed = this.speed = speed;
			this.boost = boost;

			isCounting = false;
		}

		public void Set(int start, int end)
		{
			current = this.start = start;
			currentSpeed = speed;
			this.end = end;
			lastTimePlaySound = 0;
		}

		public bool IsNotStarted()
		{
			return !isCounting && current == start && start != end;
		}

		public void StartCounting()
		{
			isCounting = true;
		}

		public void StopCounting()
		{
			isCounting = false;
		}

		public override void OnGUI()
		{
			if(isCounting)
			{
				int last = (int)current;

				if(start < end)
				{
					current += currentSpeed * Time.deltaTime;
					currentSpeed += boost * Time.deltaTime;

					if(current > end || GUI.isEndAnyAnimations)
					{
						current = end;
						isCounting = false;
					}
				}
				else
				{
					current -= currentSpeed * Time.deltaTime;
					currentSpeed += boost * Time.deltaTime;

					if(current < end || GUI.isEndAnyAnimations)
					{
						current = end;
						isCounting = false;
					}
				}

				if(last != (int)current && Time.time - lastTimePlaySound > 0.10f && playSound)
				{
					Sound.Play(counterSound, Fixed.OneHalf, 1);
					lastTimePlaySound = Time.time;
				}

				SetText(((int)current).ToString());
			}

			base.OnGUI();
		}
	}

	public class GUICounterFixed : GUIText, GUICounter
	{
		public bool isCounting { get; set; }

		public Fixed start, end, speed, boost;
		public Fixed current, currentSpeed;

		public string format;

		float lastTimePlaySound = 0;
		public Game.CollectionID counterSound = Game.CollectionID.sound_gba_gba01;
		public bool playSound = true;

		public GUICounterFixed(Fixed start, Fixed end, Fixed speed, Fixed boost, string format = "0.00", Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int percentWidth = 0, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(start.ToString(format), style, animation, width, height, percentWidth, minWidth, minHeight, maxWidth, maxHeight)
		{
			current = this.start = start;
			this.end = end;
			currentSpeed = this.speed = speed;
			this.boost = boost;
			this.format = format;

			isCounting = false;
		}

		public void Set(int start, int end)
		{
			current = this.start = start;
			currentSpeed = speed;
			this.end = end;
		}

		public bool IsNotStarted()
		{
			return !isCounting && current == start && start != end;
		}

		public void StartCounting()
		{
			isCounting = true;
		}

		public void StopCounting()
		{
			isCounting = false;
		}

		public override void OnGUI()
		{
			if(isCounting)
			{
				if(start < end)
				{
					current += currentSpeed * (Fixed)Time.deltaTime;
					currentSpeed += boost * (Fixed)Time.deltaTime;

					if(current > end || GUI.isEndAnyAnimations)
					{
						current = end;
						isCounting = false;
					}
				}
				else
				{
					current -= currentSpeed * (Fixed)Time.deltaTime;
					currentSpeed += boost * (Fixed)Time.deltaTime;

					if(current < end || GUI.isEndAnyAnimations)
					{
						current = end;
						isCounting = false;
					}
				}

				if(Time.time - lastTimePlaySound > 0.10f && playSound)
				{
					Sound.Play(counterSound, Fixed.OneHalf, 1);
					lastTimePlaySound = Time.time;
				}

				SetText(current.ToString(format));
			}

			base.OnGUI();
		}
	}

	public class GUICounterTime : GUIText, GUICounter
	{
		public bool isCounting { get; set; }

		public Fixed start, end, speed, boost;
		public Fixed current, currentSpeed;

		float lastTimePlaySound = 0;
		public Game.CollectionID counterSound = Game.CollectionID.sound_gba_gba01;
		public bool playSound = true;

		public GUICounterTime(Fixed start, Fixed end, Fixed speed, Fixed boost, Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int percentWidth = 0, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(Utils.TimeToMMSS(start), style, animation, width, height, percentWidth, minWidth, minHeight, maxWidth, maxHeight)
		{
			current = this.start = start;
			this.end = end;
			currentSpeed = this.speed = speed;
			this.boost = boost;

			isCounting = false;
		}

		public void Set(int start, int end)
		{
			current = this.start = start;
			currentSpeed = speed;
			this.end = end;
		}

		public bool IsNotStarted()
		{
			return !isCounting && current == start && start != end;
		}

		public void StartCounting()
		{
			isCounting = true;
		}

		public void StopCounting()
		{
			isCounting = false;
		}

		public override void OnGUI()
		{
			if(isCounting)
			{
				if(start < end)
				{
					current += currentSpeed * (Fixed)Time.deltaTime;
					currentSpeed += boost * (Fixed)Time.deltaTime;

					if(current > end || GUI.isEndAnyAnimations)
					{
						current = end;
						isCounting = false;
					}
				}
				else
				{
					current -= currentSpeed * (Fixed)Time.deltaTime;
					currentSpeed += boost * (Fixed)Time.deltaTime;

					if(current < end || GUI.isEndAnyAnimations)
					{
						current = end;
						isCounting = false;
					}
				}

				if(Time.time - lastTimePlaySound > 0.10f && playSound)
				{
					Sound.Play(counterSound, Fixed.OneHalf, 1);
					lastTimePlaySound = Time.time;
				}

				SetText(Utils.TimeToMMSS(current));
			}

			base.OnGUI();
		}
	}
}
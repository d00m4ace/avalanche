using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIAnimation
	{
		protected bool isPlaying = false;
		public bool playInverse = false;
		public bool isLoop = false;

		public Game.CollectionID startSound;
		public Game.CollectionID endSound;

		public virtual bool IsPlaying() { return isPlaying; }

		public virtual void Stop() { isPlaying = false; }
		public virtual void Resume() { isPlaying = true; }

		public virtual void Restart() { }
		public virtual void PlayInverse() { playInverse = !playInverse; Restart(); }

		public virtual Color AnimateBaseColor(Color color) { return color; }
		public virtual Color AnimateBackColor(Color color) { return color; }

		public virtual int AnimateX(int x) { return x; }
		public virtual int AnimateY(int y) { return y; }

		public virtual int AnimateW(int w) { return w; }
		public virtual int AnimateH(int h) { return h; }

		public virtual string AnimateText(string text) { return text; }

		public virtual void OnGUIBegin() { }
		public virtual void OnGUIEnd() { }

		public virtual List<GUIElement> AnimateElements(List<GUIElement> elements) { return elements; }
	}

	public class GUIAnimationSequence : GUIAnimation
	{
		GUIAnimation[] animations;

		GUIAnimation current;

		public GUIAnimationSequence(GUIAnimation[] animations)
		{
			this.animations = animations;
			current = animations[0];
		}

		public override bool IsPlaying()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				if(animations[i].IsPlaying()) return true;

			return false;
		}

		public override void Stop()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].Stop();
		}

		public override void Resume()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].Resume();
		}

		public override void Restart()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].Restart();
		}

		public override void PlayInverse()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].PlayInverse();
		}

		public override Color AnimateBaseColor(Color color) { return current.AnimateBaseColor(color); }
		public override Color AnimateBackColor(Color color) { return current.AnimateBackColor(color); }

		public override int AnimateX(int x) { return current.AnimateX(x); }
		public override int AnimateY(int y) { return current.AnimateY(y); }

		public override int AnimateW(int w) { return current.AnimateW(w); }
		public override int AnimateH(int h) { return current.AnimateH(h); }

		public override string AnimateText(string text) { return current.AnimateText(text); }

		public override void OnGUIBegin() { current.OnGUIBegin(); }
		public override void OnGUIEnd()
		{
			current.OnGUIEnd();

			if(!current.IsPlaying())
			{
				if(!playInverse)
				{
					for(int i = 0; i < animations.Length - 1; i++)
					{
						if(current == animations[i])
						{
							current = animations[i + 1];
							break;
						}
					}
				}
				else
				{
					for(int i = animations.Length - 1; i > 1; i--)
					{
						if(current == animations[i])
						{
							current = animations[i - 1];
							break;
						}
					}
				}
			}
		}

		public override List<GUIElement> AnimateElements(List<GUIElement> elements) { return current.AnimateElements(elements); }
	}

	public class GUIAnimationFilter : GUIAnimation
	{
		GUIAnimation[] animations;

		public GUIAnimationFilter(GUIAnimation[] animations)
		{
			this.animations = animations;
		}

		public override bool IsPlaying()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				if(animations[i].IsPlaying()) return true;

			return false;
		}

		public override void Stop()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].Stop();
		}

		public override void Resume()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].Resume();
		}

		public override void Restart()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].Restart();
		}

		public override void PlayInverse()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].PlayInverse();
		}

		public override Color AnimateBaseColor(Color color)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				color = animations[i].AnimateBaseColor(color);
			return color;
		}

		public override Color AnimateBackColor(Color color)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				color = animations[i].AnimateBackColor(color);
			return color;
		}

		public override int AnimateX(int x)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				x = animations[i].AnimateX(x);
			return x;
		}
		public override int AnimateY(int y)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				y = animations[i].AnimateY(y);
			return y;
		}

		public override int AnimateW(int w)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				w = animations[i].AnimateW(w);
			return w;
		}

		public override int AnimateH(int h)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				h = animations[i].AnimateH(h);
			return h;
		}

		public override string AnimateText(string text)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				text = animations[i].AnimateText(text);
			return text;
		}

		public override void OnGUIBegin()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].OnGUIBegin();
		}

		public override void OnGUIEnd()
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				animations[i].OnGUIEnd();
		}

		public override List<GUIElement> AnimateElements(List<GUIElement> elements)
		{
			int ic = animations.Length;
			for(int i = 0; i < ic; i++)
				elements = animations[i].AnimateElements(elements);
			return elements;
		}
	}

	public class GUIAnimationTimer : GUIAnimation
	{
		public float timeLength, time, t, invTimeLength;

		public bool isStarted;
		public bool isEnded;

		public GUIAnimationTimer(float time, bool playInverse = false, bool isLoop = false)
		{
			timeLength = time; invTimeLength = 1 / timeLength;
			this.playInverse = playInverse; this.isLoop = isLoop;
			this.time = playInverse ? 0 : timeLength;
			t = (timeLength - this.time) * invTimeLength;
			isPlaying = true;
			isStarted = isEnded = false;
		}

		public override void Restart()
		{
			time = playInverse ? 0 : timeLength;
			t = (timeLength - time) * invTimeLength;
			isPlaying = true;
			isStarted = isEnded = false;
		}

		public override void OnGUIBegin()
		{
			if(!isPlaying)
				return;

			if(!isStarted)
			{
				isStarted = true;
				if(startSound != Game.CollectionID.none)
					Sound.Play(startSound);
			}

			if(!playInverse)
			{
				time -= Time.deltaTime;

				if(time < 0 || GUI.isEndAnyAnimations)
				{
					time = 0;
					isPlaying = false;

					if(isLoop)
						PlayInverse();
				}

				t = (timeLength - time) * invTimeLength;
			}
			else
			{
				time += Time.deltaTime;

				if(time > timeLength || GUI.isEndAnyAnimations)
				{
					time = timeLength;
					isPlaying = false;

					if(isLoop)
						PlayInverse();
				}

				t = (timeLength - time) * invTimeLength;
			}

			if(!isPlaying && !isEnded)
			{
				isEnded = true;
				if(endSound != Game.CollectionID.none)
					Sound.Play(endSound);
			}
		}
	}

	public class GUIAnimationElementsWait : GUIAnimation
	{
		GUIElement element;

		public GUIAnimationElementsWait(GUIElement element)
		{
			this.element = element;
		}

		public override List<GUIElement> AnimateElements(List<GUIElement> elements)
		{
			List<GUIElement> animateElements = new List<GUIElement>();

			int i, ic = elements.Count;
			for(i = 0; i < ic; i++)
				if(elements[i].IsPlayingAnimation())
					break;

			int c = i + 1;

			if(c > elements.Count)
				c = elements.Count;

			for(i = 0; i < c; i++)
				animateElements.Add(elements[i]);

			return animateElements;
		}
	}

	public class GUIAnimationSize : GUIAnimationTimer
	{
		int startW, startH;

		public GUIAnimationSize(float time, int startW = int.MinValue, int startH = int.MinValue, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
			this.startW = startW;
			this.startH = startH;
		}

		public override int AnimateW(int w) { return startW != int.MinValue ? (int)((w - startW) * t + startW) : w; }
		public override int AnimateH(int h) { return startH != int.MinValue ? (int)((h - startH) * t + startH) : h; }
	}

	public class GUIAnimationXY : GUIAnimationTimer
	{
		int startX, startY;

		public GUIAnimationXY(float time, int startX = int.MinValue, int startY = int.MinValue, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
			this.startX = startX;
			this.startY = startY;
		}

		public override int AnimateX(int x) { return startX != int.MinValue ? (int)((x - startX) * t + startX) : x; }
		public override int AnimateY(int y) { return startY != int.MinValue ? (int)((y - startY) * t + startY) : y; }
	}

	public class GUIAnimationXYMove : GUIAnimationTimer
	{
		int moveX, moveY;

		public GUIAnimationXYMove(float time, int moveX = int.MinValue, int moveY = int.MinValue, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
			this.moveX = moveX;
			this.moveY = moveY;
		}

		public override int AnimateX(int x) { return moveX != int.MinValue ? (int)(((x - moveX) - x) * t + x) : x; }
		public override int AnimateY(int y) { return moveY != int.MinValue ? (int)(((y - moveY) - y) * t + y) : y; }
	}

	public class GUIAnimationBaseColor : GUIAnimationTimer
	{
		Color color;

		public GUIAnimationBaseColor(float time, Color color, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
			this.color = color;
		}

		public override Color AnimateBaseColor(Color color) { return Color.Lerp(this.color, color, t); }
	}

	public class GUIAnimationBackColor : GUIAnimationTimer
	{
		Color color;

		public GUIAnimationBackColor(float time, Color color, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
			this.color = color;
		}

		public override Color AnimateBackColor(Color color) { return Color.Lerp(this.color, color, t); }
	}

	public class GUIAnimationBlinkBaseColor : GUIAnimationTimer
	{
		Color color;

		public GUIAnimationBlinkBaseColor(float time, Color color, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
			this.color = color;
		}

		public override Color AnimateBaseColor(Color color) { return t < 0.5f ? color : this.color; }
	}

	public class GUIAnimationBlinkBackColor : GUIAnimationTimer
	{
		Color color;

		public GUIAnimationBlinkBackColor(float time, Color color, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
			this.color = color;
		}

		public override Color AnimateBackColor(Color color) { return t < 0.5f ? color : this.color; }
	}

	public class GUIAnimationText : GUIAnimationTimer
	{
		static StringBuilder sb = new StringBuilder();

		float lastTimePlaySound = 0;
		public Game.CollectionID printSound = Game.CollectionID.sound_gba_gba03;
		public Fixed printSoundPitch = 4;
		public float playSoundRate = 0.1f;

		public GUIAnimationText(float time, bool playInverse = false, bool isLoop = false) : base(time, playInverse, isLoop)
		{
		}

		public override string AnimateText(string text)
		{
			int lastLength = sb.Length;

			sb.Length = 0;
			sb.Append(text);
			sb.Length = (int)(text.Length * t);

			if(lastLength != sb.Length && isPlaying && Time.time - lastTimePlaySound > playSoundRate)
			{
				Sound.Play(printSound, Fixed.OneHalf, printSoundPitch);
				lastTimePlaySound = Time.time;
			}

			return sb.ToString();
		}
	}
}

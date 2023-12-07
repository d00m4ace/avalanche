using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIElement
	{
		public Game.GUIStyle style;

		public bool isDisabled = false;

		public GUIAnimation animation = new GUIAnimation();

		public Color color;

		public int x, y;
		public int width, height;

		public int minWidth, minHeight;
		public int maxWidth, maxHeight;

		public GUIElement(int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue, int width = -1, int height = -1)
		{
			this.minWidth = minWidth;
			this.minHeight = minHeight;
			this.maxWidth = maxWidth;
			this.maxHeight = maxHeight;

			color = Color.White;

			if(width != -1)
				this.minWidth = this.maxWidth = width;
			if(height != -1)
				this.minHeight = this.maxHeight = height;

			width = height = 0;
		}

		public virtual bool IsPlayingAnimation()
		{
			if(isDisabled)
				return false;

			return animation.IsPlaying();
		}

		public virtual void StopgAnimation()
		{
			animation.Stop();
		}

		public virtual void ResumeAnimation()
		{
			animation.Resume();
		}

		public virtual void RestartAnimation()
		{
			animation.Restart();
		}

		public virtual void PlayInverseAnimation()
		{
			animation.PlayInverse();
		}

		public virtual void SetStyle()
		{
		}

		public virtual void SetPos(int x, int y)
		{
			this.x = x; this.y = y;
		}

		public virtual void OnGUI()
		{
		}

		public virtual void RestSize()
		{
			width = height = 0;
		}

		public virtual void CalcWidth()
		{
		}

		public virtual void CalcHeight()
		{
		}

		public int GetWidth()
		{
			if(width < minWidth)
				return minWidth;

			if(width > maxWidth)
				return maxWidth;

			return width;
		}

		public int GetHeight()
		{
			if(height < minHeight)
				return minHeight;

			if(height > maxHeight)
				return maxHeight;

			return height;
		}
	}
}
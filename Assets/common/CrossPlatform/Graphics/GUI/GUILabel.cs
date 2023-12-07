using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUILabel : GUIElement
	{
		public List<GUIElement> elements;

		GUIStyle back;

		public Color backColor;

		public ContentLayout contentLayout;

		public int spacing = 1;

		public enum ContentLayout
		{
			Horizontal,
			Vertical,
		};

		public GUILabel(GUIElement[] elements = default(GUIElement[]), Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), ContentLayout contentLayout = ContentLayout.Horizontal, int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue, int spacing = -1) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
			if(animation != null)
				this.animation = animation;

			this.elements = elements != null ? new List<GUIElement>(elements) : new List<GUIElement>();

			this.contentLayout = contentLayout;

			this.style = style;
			SetStyle();

			if(spacing != -1)
				this.spacing = spacing;

			if(width != -1)
				this.minWidth = this.maxWidth = width;
			if(height != -1)
				this.minHeight = this.maxHeight = height;
		}

		public override bool IsPlayingAnimation()
		{
			if(isDisabled)
				return false;

			if(animation.IsPlaying())
				return true;

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				if(!elements[i].isDisabled && elements[i].IsPlayingAnimation())
					return true;

			return false;
		}

		public override void StopgAnimation()
		{
			animation.Stop();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].StopgAnimation();
		}

		public override void ResumeAnimation()
		{
			animation.Resume();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].ResumeAnimation();
		}

		public override void RestartAnimation()
		{
			animation.Restart();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].RestartAnimation();
		}

		public override void PlayInverseAnimation()
		{
			animation.PlayInverse();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].PlayInverseAnimation();
		}

		public override void SetStyle()
		{
			back = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].labelBackStyle];

			backColor = GUI.styles[(int)style].labelBackColor;

			spacing = GUI.styles[(int)style].labelSpacing;

			if(GUI.styles[(int)style].labelMinWidth != int.MinValue)
				minWidth = GUI.styles[(int)style].labelMinWidth;
			if(GUI.styles[(int)style].labelMinHeight != int.MinValue)
				minHeight = GUI.styles[(int)style].labelMinHeight;
			if(GUI.styles[(int)style].labelMaxWidth != int.MaxValue)
				maxWidth = GUI.styles[(int)style].labelMaxWidth;
			if(GUI.styles[(int)style].labelMaxHeight != int.MaxValue)
				maxHeight = GUI.styles[(int)style].labelMaxHeight;

			if(style != Game.GUIStyle.Default)
			{
				int ic = elements.Count;
				for(int i = 0; i < ic; i++)
					if(elements[i].style == Game.GUIStyle.Default)
					{
						elements[i].style = style;
						elements[i].SetStyle();
					}
			}
		}

		public override void RestSize()
		{
			width = height = 0;
			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].RestSize();
		}

		public override void CalcWidth()
		{
			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].CalcWidth();

			if(width == 0)
			{
				if(contentLayout == ContentLayout.Horizontal)
				{
					ic = elements.Count;
					for(int i = 0; i < ic; i++)
						width += elements[i].GetWidth() + spacing;
					if(elements.Count > 0) width -= spacing;
				}
				else
				{
					ic = elements.Count;
					for(int i = 0; i < ic; i++)
					{
						int itemWidth = elements[i].GetWidth();
						width = itemWidth > width ? itemWidth : width;
					}
				}

				width += back.border.left + back.border.right;
			}
		}

		public override void CalcHeight()
		{
			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].CalcHeight();

			if(height == 0)
			{
				if(contentLayout == ContentLayout.Horizontal)
				{
					ic = elements.Count;
					for(int i = 0; i < ic; i++)
					{
						int itemHeight = elements[i].GetHeight();
						height = itemHeight > height ? itemHeight : height;
					}
				}
				else
				{
					ic = elements.Count;
					for(int i = 0; i < ic; i++)
						height += elements[i].GetHeight() + spacing;
					if(elements.Count > 0) height -= spacing;
				}

				height += back.border.top + back.border.bottom;
			}
		}

		public override void OnGUI()
		{
			animation.OnGUIBegin();

			Color guiBaseColor = GUI.baseColor;
			GUI.baseColor = GUI.baseColor * animation.AnimateBaseColor(color);

			CalcWidth();
			CalcHeight();

			int w = animation.AnimateW(GetWidth());
			int h = animation.AnimateH(GetHeight());

			Rect pos = new Rect(animation.AnimateX(x), animation.AnimateY(y), w, h);

			if(back.normal.background != null)
			{
				GUI.SetColor(animation.AnimateBackColor(backColor));
				UnityEngine.GUI.Box(pos, "", back);
			}

			int xOffs = (int)pos.x + back.border.left, yOffs = (int)pos.y + back.border.top;

			if(contentLayout == ContentLayout.Horizontal)
			{
				List<GUIElement> anims = animation.AnimateElements(elements);
				int ic = anims.Count;
				for(int i = 0; i < ic; i++)
				{
					anims[i].SetPos(xOffs, yOffs + (height - back.border.top - back.border.bottom - anims[i].GetHeight()) / 2);
					if(!anims[i].isDisabled)
						anims[i].OnGUI();
					xOffs += anims[i].GetWidth() + spacing;
				}
			}
			else if(contentLayout == ContentLayout.Vertical)
			{
				List<GUIElement> anims = animation.AnimateElements(elements);
				int ic = anims.Count;
				for(int i = 0; i < ic; i++)
				{
					anims[i].SetPos(xOffs + (width - back.border.left - back.border.right - anims[i].GetWidth()) / 2, yOffs);
					if(!anims[i].isDisabled)
						anims[i].OnGUI();
					yOffs += anims[i].GetHeight() + spacing;
				}
			}

			GUI.baseColor = guiBaseColor;

			animation.OnGUIEnd();
		}
	}
}
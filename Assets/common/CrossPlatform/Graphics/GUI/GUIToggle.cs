using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIToggle : GUIButton
	{
		public GUIElement[] elements;

		public Color[] backColors;

		public int toggle;

		public GUIToggle(Game.ButtonID buttonID, GUIElement[] elements = default(GUIElement[]), Color[] backColors = default(Color[]), Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue, int id = 0, bool defaultFocus = false) : base(buttonID, elements[0], style, animation, width, height, minWidth, minHeight, maxWidth, maxHeight, id, defaultFocus)
		{
			this.elements = elements;
			this.backColors = backColors;
			toggle = 0;

			if(backColors != null)
				backColor = backColors[0];
		}

		public override void SetStyle()
		{
			base.SetStyle();

			if(elements != null && style != Game.GUIStyle.Default)
			{
				int ic = elements.Length;
				for(int i = 0; i < ic; i++)
					if(elements[i].style == Game.GUIStyle.Default)
					{
						elements[i].style = style;
						elements[i].SetStyle();
					}
			}
		}

		public override void CalcWidth()
		{
			if(width == 0)
			{
				int ic = elements.Length;
				for(int i = 0; i < ic; i++)
				{
					elements[i].CalcWidth();
					int itemWidth = elements[i].GetWidth();

					if(itemWidth > width)
						width = itemWidth;
				}

				width += unpressed.border.left + unpressed.border.right;
			}
		}

		public override void CalcHeight()
		{
			if(height == 0)
			{
				int ic = elements.Length;
				for(int i = 0; i < ic; i++)
				{
					elements[i].CalcHeight();
					int itemHeight = elements[i].GetHeight();

					if(itemHeight > height)
						height = itemHeight;
				}

				height += unpressed.border.top + unpressed.border.bottom;
			}
		}

		public void SetToggle(int toggle)
		{
			this.toggle = toggle = toggle % elements.Length;

			element = elements[toggle];

			if(backColors != null)
				backColor = backColors[toggle];
		}

		public override void OnGUI()
		{
			base.OnGUI();

			if(GUI.buttonPushed == this)
				SetToggle(toggle + 1);
		}
	}
}
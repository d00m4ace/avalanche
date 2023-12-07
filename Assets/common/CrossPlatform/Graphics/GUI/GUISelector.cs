using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUISelector : GUIElement
	{
		public GUIElement[] elements;

		public int elementMaxWidth;

		public GUIButton leftButton;
		public GUIButton rightButton;

		public int selector;

		public GUISelector(GUIElement[] elements = default(GUIElement[]), Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(minWidth, minHeight, maxWidth, maxHeight, width, height)
		{
			this.elements = elements;

			selector = 0;

			leftButton = new GUIButton(Game.ButtonID.Left, new GUIText("◀"), style);
			rightButton = new GUIButton(Game.ButtonID.Left, new GUIText("▶"), style);
		}

		public override void SetStyle()
		{
			base.SetStyle();

			if(style != Game.GUIStyle.Default)
			{
				leftButton.style = style;
				leftButton.SetStyle();

				rightButton.style = style;
				rightButton.SetStyle();
				
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

				elementMaxWidth = width;

				leftButton.CalcWidth();
				rightButton.CalcWidth();

				width += leftButton.GetWidth() + rightButton.GetWidth() + 2;
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

				leftButton.CalcHeight();
				rightButton.CalcHeight();

				if(leftButton.GetHeight() > height)
					height = leftButton.GetHeight();

				if(rightButton.GetHeight() > height)
					height = rightButton.GetHeight();
			}
		}

		public void SetSelector(int selector)
		{
			if(selector < 0)
				selector = elements.Length - 1;

			this.selector = selector = selector % elements.Length;
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

			int xOffs = (int)pos.x, yOffs = (int)pos.y;

			leftButton.SetPos(xOffs, yOffs + (height - leftButton.GetHeight()) / 2);
			leftButton.OnGUI();
			xOffs += leftButton.GetWidth() + 1;

			elements[selector].SetPos(xOffs + (elementMaxWidth - elements[selector].GetWidth()) / 2, yOffs + (height - elements[selector].GetHeight()) / 2);
			elements[selector].OnGUI();
			xOffs += elementMaxWidth + 1;

			rightButton.SetPos(xOffs, yOffs + (height - rightButton.GetHeight()) / 2);
			rightButton.OnGUI();
			xOffs += rightButton.GetWidth();

			if(GUI.buttonPushed != null)
			{
				if(GUI.buttonPushed == leftButton)
				{
					SetSelector(selector - 1);
					GUI.buttonPushed = null;
				}
				else if(GUI.buttonPushed == rightButton)
				{
					SetSelector(selector + 1);
					GUI.buttonPushed = null;
				}
			}

			GUI.baseColor = guiBaseColor;

			animation.OnGUIEnd();
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIHorizontalSlider : GUIElement
	{
		public GUIStyle back;
		public GUIStyle backShine;
		public GUIStyle bar;
		public GUIStyle barShine;

		public Color barColor, backColor;

		public Fixed progress;

		public int percentWidth;

		public GUIHorizontalSlider(Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int percentWidth = 0, int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
			if(animation != null)
				this.animation = animation;

			this.progress = 0;

			this.percentWidth = percentWidth;

			this.style = style;
			SetStyle();

			if(width != -1)
				this.minWidth = this.maxWidth = width;
			if(height != -1)
				this.minHeight = this.maxHeight = height;
		}

		public override void SetStyle()
		{
			back = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].progressBackStyle];
			backShine = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].progressBackShineStyle];
			bar = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].progressBarStyle];
			barShine = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].progressBarShineStyle];

			color = Color.White;

			barColor = GUI.styles[(int)style].progressBarColor;
			backColor = GUI.styles[(int)style].progressBackColor;

			if(GUI.styles[(int)style].progressBarMinWidth != int.MinValue)
				minWidth = GUI.styles[(int)style].progressBarMinWidth;
			if(GUI.styles[(int)style].progressBarMinHeight != int.MinValue)
				minHeight = GUI.styles[(int)style].progressBarMinHeight;
			if(GUI.styles[(int)style].progressBarMaxWidth != int.MaxValue)
				maxWidth = GUI.styles[(int)style].progressBarMaxWidth;
			if(GUI.styles[(int)style].progressBarMaxHeight != int.MaxValue)
				maxHeight = GUI.styles[(int)style].progressBarMaxHeight;
		}

		public void SetProgress(Fixed progress)
		{
			this.progress = progress;

			if(this.progress > 1)
				this.progress = 1;
			else if(this.progress < 0)
				this.progress = 0;
		}

		public override void RestSize()
		{
			width = height = 0;
		}

		public override void CalcWidth()
		{
			if(width == 0)
			{
				if(percentWidth != 0)
					width = (int)(percentWidth * GUI.PeekGuiClip().width) / 100;
			}
		}

		public override void CalcHeight()
		{
		}

		public float slider = 0.0F;

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

			Rect posShadow = new Rect(pos.x + 1, pos.y + 1, w, h);
			GUI.SetColor(backColor);
			UnityEngine.GUI.Box(posShadow, "", back);

			GUI.SetColor(backColor);
			UnityEngine.GUI.Box(pos, "", back);

			//GUI.SetColor(Color.White);
			//UnityEngine.GUI.Box(pos, "", backShine);

			int progressW = (int)((w - bar.border.left - bar.border.right) * progress);

			if(progress > 0)
			{
				Rect barPos = new Rect(pos.x, pos.y, progressW + bar.border.left + bar.border.right, h);

				GUI.SetColor(barColor);
				UnityEngine.GUI.Box(barPos, "", bar);

				GUI.SetColor(Color.White);
				UnityEngine.GUI.Box(barPos, "", barShine);
			}

			Rect posThumb = new Rect(pos.x - 4, pos.y - 4, w + 8, h + 8);
			GUI.SetColor(barColor);
			slider = UnityEngine.GUI.HorizontalSlider(posThumb, slider, 0.0F, 1.0F, Render.guiSkin.customStyles[(int)GUI.GUISkinCustomStyle.Empty], Render.guiSkin.customStyles[(int)GUI.GUISkinCustomStyle.Thumb]);

			//GUI.SetColor(indicatorBackColor);
			//UnityEngine.GUI.DrawTextureWithTexCoords(rect, imageIndicatorBack, new Rect(0, 0, rect.width / (imageIndicatorBack.width * scale), rect.height / (imageIndicatorBack.height * scale)));
			//rect = new Rect(animation.AnimateX(x), animation.AnimateY(y), (float)(w * progress), h);
			//GUI.SetColor(indicatorColor);
			//UnityEngine.GUI.DrawTextureWithTexCoords(rect, imageIndicator, new Rect(0, 0, rect.width / (imageIndicator.width * scale), rect.height / (imageIndicator.height * scale)));

			//text.CalcWidth();
			//text.CalcHeight();

			//text.x = (int)pos.x + (w - text.GetWidth()) / 2;
			//text.y = (int)pos.y + (h - text.GetHeight()) / 2;
			//text.OnGUI();

			GUI.baseColor = guiBaseColor;

			animation.OnGUIEnd();
		}
	}
}
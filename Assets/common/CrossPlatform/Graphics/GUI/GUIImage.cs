using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIImage : GUIElement
	{
		public Texture2D image;

		public Color imageColor;
		public Color shadowColor;

		public float scale = 1;

		public int imageShadowXOffset = 1, imageShadowYOffset = 1;

		public GUIImage(string image, Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, float scale = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue, Color imageColor = default(Color)) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
			if(animation != null)
				this.animation = animation;

			SetImage(image);

			this.style = style;
			SetStyle();

			if(imageColor.a != 0 || imageColor.r != 0 || imageColor.g != 0 || imageColor.b != 0)
				this.imageColor = imageColor;
			else
				this.imageColor = Color.White;

			if(scale != -1)
				this.scale = scale;

			if(width != -1)
				this.minWidth = this.maxWidth = width;
			if(height != -1)
				this.minHeight = this.maxHeight = height;
		}

		public override void SetStyle()
		{
			color = GUI.styles[(int)style].imageColor;
			shadowColor = GUI.styles[(int)style].imageShadowColor;
			scale = GUI.styles[(int)style].imageScale;

			if(GUI.styles[(int)style].imageMinWidth != int.MinValue)
				minWidth = GUI.styles[(int)style].imageMinWidth;
			if(GUI.styles[(int)style].imageMinHeight != int.MinValue)
				minHeight = GUI.styles[(int)style].imageMinHeight;
			if(GUI.styles[(int)style].imageMaxWidth != int.MaxValue)
				maxWidth = GUI.styles[(int)style].imageMaxWidth;
			if(GUI.styles[(int)style].imageMaxHeight != int.MaxValue)
				maxHeight = GUI.styles[(int)style].imageMaxHeight;

			imageShadowXOffset = GUI.styles[(int)style].imageShadowXOffset;
			imageShadowYOffset = GUI.styles[(int)style].imageShadowYOffset;
		}

		public void SetImage(string image)
		{
			this.image = Render.GetTexture(image);
			RestSize();
		}

		public override void RestSize()
		{
			width = height = 0;
		}

		public override void CalcWidth()
		{
			if(width == 0)
				width = (int)(image.width * scale);
		}

		public override void CalcHeight()
		{
			if(height == 0)
				height = (int)(image.height * scale);
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

			if(shadowColor.a > 0)
			{
				GUI.SetColor(shadowColor);
				Rect shadowPos = new Rect(pos.x + imageShadowXOffset, pos.y + imageShadowYOffset, w, h);
				UnityEngine.GUI.DrawTexture(shadowPos, image, ScaleMode.ScaleToFit);
			}

			GUI.SetColor(imageColor);
			UnityEngine.GUI.DrawTexture(pos, image, ScaleMode.ScaleToFit);

			GUI.baseColor = guiBaseColor;

			animation.OnGUIEnd();
		}
	}
}
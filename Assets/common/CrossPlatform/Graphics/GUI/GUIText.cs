using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIText : GUIElement
	{
		public Game.TEXT textID;

		public string text;

		public int textMaxLength = 0;

		public int defaultFontSize = 0;

		public Color shadowColor;

		public int textShadowXOffset = 1, textShadowYOffset = 1;

		public bool textUpperCase = false;

		public int percentWidth;
		public int heightTextLines;

		GUIStyle font;

		public Align align;

		public enum Align
		{
			TopLeft = 0,
			TopCenter,
			TopRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			BottomLeft,
			BottomCenter,
			BottomRight,
		};

		public GUIText(string text = "", Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int percentWidth = 0, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue, int heightTextLines = 0, Align align = Align.MiddleLeft, Color color = default(Color), int textMaxLength = 0, int defaultFontSize = 0) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
			this.text = text;
			this.textMaxLength = textMaxLength;
			this.percentWidth = percentWidth;
			this.heightTextLines = heightTextLines;
			this.align = align;
			this.defaultFontSize = defaultFontSize;
			Setup(style, animation, width, height, color);
		}

		public GUIText(Game.TEXT textID, Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int percentWidth = 0, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue, int heightTextLines = 0, Align align = Align.MiddleLeft, Color color = default(Color), int textMaxLength = 0, int defaultFontSize = 0) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
			this.textID = textID;
			this.textMaxLength = textMaxLength;
			this.percentWidth = percentWidth;
			this.heightTextLines = heightTextLines;
			this.align = align;
			this.defaultFontSize = defaultFontSize;
			Setup(style, animation, width, height, color);
		}

		void Setup(Game.GUIStyle style, GUIAnimation animation, int width, int height, Color color)
		{
			if(animation != null)
				this.animation = animation;

			this.style = style;
			SetStyle();

			if(color.r != 0 || color.g != 0 || color.b != 0 || color.a != 0)
				this.color = color;

			if(width != -1)
				minWidth = maxWidth = width;
			if(height != -1)
				minHeight = maxHeight = height;
		}

		public override void SetStyle()
		{
			color = GUI.styles[(int)style].textColor;
			shadowColor = GUI.styles[(int)style].textShadowColor;
			font = GUI.styles[(int)style].font;

			minWidth = GUI.styles[(int)style].textMinWidth;
			minHeight = GUI.styles[(int)style].textMinHeight;
			maxWidth = GUI.styles[(int)style].textMaxWidth;
			maxHeight = GUI.styles[(int)style].textMaxHeight;

			textShadowXOffset = GUI.styles[(int)style].textShadowXOffset;
			textShadowYOffset = GUI.styles[(int)style].textShadowYOffset;

			textUpperCase = GUI.styles[(int)style].textUpperCase;

			RestSize();
		}

		public void SetText(string text)
		{
			this.text = text;
			textID = Game.TEXT.None;
			RestSize();
		}

		public void SetText(Game.TEXT textID)
		{
			this.textID = textID;
			RestSize();
		}

		public override void RestSize()
		{
			if(textID != Game.TEXT.None)
				text = textID.GetText();

			if(textMaxLength != 0)
			{
				if(text.Length > textMaxLength)
					text = text.Substring(0, textMaxLength) + '.';
			}

			if(textUpperCase)
				text = text.ToUpper();

			width = height = 0;
		}

		public override void CalcWidth()
		{
			if(width == 0)
			{
				if(percentWidth != 0)
					width = (int)(percentWidth * GUI.PeekGuiClip().width) / 100;
			}

			if(width == 0)
			{
				float minWidth, maxWidth;
				font.alignment = TextAnchor.MiddleLeft;

				//Console.WriteLine(font.fontSize);
				bool fixFontSize = false;
				if(font.fontSize == 0)
				{
					fixFontSize = true;
					font.fontSize = defaultFontSize != 0 ? defaultFontSize : Utils.GetLanguageDefaultFontSize(Game.settings.language - 1);
				}

				font.CalcMinMaxWidth(new GUIContent(text), out minWidth, out maxWidth);
				width = (int)maxWidth;

				if(fixFontSize)
					font.fontSize = 0;
			}
		}

		public override void CalcHeight()
		{
			bool fixFontSize = false;
			if(font.fontSize == 0)
			{
				fixFontSize = true;
				font.fontSize = defaultFontSize != 0 ? defaultFontSize : Utils.GetLanguageDefaultFontSize(Game.settings.language - 1);
			}

			if(height == 0)
			{
				if(heightTextLines != 0)
					height = (int)(font.lineHeight + 1) * heightTextLines;
			}

			if(height == 0)
				height = (int)font.lineHeight + 1;

			if(fixFontSize)
				font.fontSize = 0;
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

			Rect fontPos = new Rect(animation.AnimateX(x), animation.AnimateY(y), w, h);

			font.wordWrap = true;

			font.alignment = (TextAnchor)align;

			bool fixFontSize = false;
			if(font.fontSize == 0)
			{
				fixFontSize = true;
				font.fontSize = defaultFontSize != 0 ? defaultFontSize : Utils.GetLanguageDefaultFontSize(Game.settings.language - 1);
			}

			string animationText = animation.AnimateText(text);

			if(shadowColor.a > 0)
			{
				GUI.SetColor(shadowColor);
				Rect fontShadowPos = new Rect(fontPos.x + textShadowXOffset, fontPos.y + textShadowYOffset, w, h);
				UnityEngine.GUI.Label(fontShadowPos, animationText, font);
			}

			GUI.SetColor(Color.White);
			UnityEngine.GUI.Label(fontPos, animationText, font);

			font.wordWrap = false;

			if(fixFontSize)
				font.fontSize = 0;

			GUI.baseColor = guiBaseColor;

			animation.OnGUIEnd();
		}
	}
}
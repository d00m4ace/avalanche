using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public struct Color
	{
		public byte r, g, b, a;

		public static Color C(int r, int g, int b, int a) { Color c; c.r = (byte)r; c.g = (byte)g; c.b = (byte)b; c.a = (byte)a; return c; }

		public static Color Lerp(Color from, Color to, float t)
		{
			int r = (int)((to.r - from.r) * t + from.r);
			int g = (int)((to.g - from.g) * t + from.g);
			int b = (int)((to.b - from.b) * t + from.b);
			int a = (int)((to.a - from.a) * t + from.a);
			return C(r, g, b, a);
		}

		public static Color operator *(Color a, Color b)
		{
			return C(((a.r * b.r) >> 8) + 1, ((a.g * b.g) >> 8) + 1, ((a.b * b.b) >> 8) + 1, ((a.a * b.a) >> 8) + 1);
		}

		public static readonly Color Zero = C(0, 0, 0, 0);
		public static readonly Color White = C(255, 255, 255, 255);
		public static readonly Color Black = C(0, 0, 0, 255);
		public static readonly Color Red = C(255, 0, 0, 255);
		public static readonly Color Green = C(0, 255, 0, 255);
		public static readonly Color Blue = C(0, 0, 255, 255);
		public static readonly Color Cyan = C(0, 255, 255, 255);
		public static readonly Color Magenta = C(255, 0, 255, 255);
		public static readonly Color Yellow = C(255, 255, 0, 255);

		public static readonly Color Purple = C(255, 0, 128, 255);
		public static readonly Color Orange = C(255, 128, 0, 255);
		public static readonly Color Violet = C(128, 0, 255, 255);
		public static readonly Color Pink = C(255, 128, 128, 255);
		public static readonly Color SkyBlue = C(0, 128, 255, 255);
		public static readonly Color DarkBlue = C(0, 0, 128, 255);
		public static readonly Color DarkGreen = C(0, 128, 0, 255);
		public static readonly Color DarkRed = C(128, 0, 0, 255);
		public static readonly Color Dark = C(51, 51, 51, 255);
		public static readonly Color LimeGreen = C(128, 255, 0, 255);
		public static readonly Color DarkCyan = C(0, 128, 128, 255);
		public static readonly Color DarkYellow = C(128, 128, 0, 255);
		public static readonly Color PaleMagenta = C(255, 128, 255, 255);

		public static readonly Color Brown = C(139, 69, 19, 255);
		public static readonly Color Crimson = C(220, 20, 60, 255);
		public static readonly Color Silver = C(192, 192, 192, 255);
		public static readonly Color Gray = C(169, 169, 169, 255);
		public static readonly Color DarkGray = C(128, 128, 128, 255);
		public static readonly Color Sienna = C(160, 82, 45, 255);
		public static readonly Color Gold = C(255, 215, 0, 255);
		public static readonly Color Chocolate = C(210, 69, 30, 255);
		public static readonly Color Peru = C(205, 133, 63, 255);
		public static readonly Color Lavender = C(230, 230, 250, 255);
		public static readonly Color RoyalBlue = C(65, 105, 225, 255);
		public static readonly Color SlateGray = C(112, 128, 144, 255);
		public static readonly Color LightSlateGray = C(119, 136, 153, 255);
		public static readonly Color Dimgray = C(105, 105, 105, 255);

		public static readonly Color LightSkyBlue = C(130, 190, 255, 255);

		public static readonly Color Snow = C(188, 222, 255, 255);

		public static readonly Color LightBlue = C(0, 133, 255, 255);
		public static readonly Color LightGreen = C(144, 225, 0, 255);
		public static readonly Color LightOrange = C(255, 167, 0, 255);

		public static readonly Color WhiteAlpha80 = C(255, 255, 255, 204);
		public static readonly Color WhiteAlpha75 = C(255, 255, 255, 191);
		public static readonly Color WhiteAlpha50 = C(255, 255, 255, 128);
		public static readonly Color WhiteAlpha35 = C(255, 255, 255, 89);
		public static readonly Color WhiteAlpha25 = C(255, 255, 255, 63);
		public static readonly Color WhiteAlpha0 = C(255, 255, 255, 0);
	}
}
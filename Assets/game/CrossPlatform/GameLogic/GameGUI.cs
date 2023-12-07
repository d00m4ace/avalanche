using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public partial class Game
	{
		public enum GUIStyle
		{
			Default,
			Empty,
			Block,
			BigFont,
			MediumFont,
			Keyboard,
			Button,
			BoxButton,
		}
			
		public static void CreateGUIStyles()
		{
#if !SERVER
			GUI.styles.Clear();

			{
				GUI.Style style = new GUI.Style();

				style.SetShadowTexture("gui/images/bar");

				style.SetFont("PressStart2P");

				GUI.styles.Add(style); //Default
			}

			{
				GUI.Style style = new GUI.Style();

				style.windowBackStyle = style.windowBackShineStyle =
				style.labelBackStyle =
				style.buttonUnpressedStyle = style.buttonPressedStyle = style.buttonUnpressedShineStyle = style.buttonPressedShineStyle = style.buttonFocusStyle =
				GUI.GUISkinCustomStyle.Empty;

				style.SetFont("PressStart2P");

				GUI.styles.Add(style); //Empty
			}

			{
				GUI.Style style = new GUI.Style();

				style.windowBackStyle = GUI.GUISkinCustomStyle.Block;
				style.windowBackShineStyle = GUI.GUISkinCustomStyle.Empty;
				style.labelBackStyle = GUI.GUISkinCustomStyle.Block;

				style.SetFont("PressStart2P");

				GUI.styles.Add(style); //Block
			}

			{
				GUI.Style style = new GUI.Style();

				style.labelSpacing = style.windowSpacing = 2 * 3;
				style.textMinHeight = 8 * 3 + 4;
				style.buttonMinWidth = 22 * 3;
				style.buttonMinHeight = 18 * 3;
				style.imageScale = 3;

				style.SetFont("PressStart2P", 8 * 3);

				style.SetShadowTexture("gui/images/bar");

				GUI.styles.Add(style); //BigFont
			}

			{
				GUI.Style style = new GUI.Style();

				style.labelSpacing = style.windowSpacing = 2 * 2;
				style.textMinHeight = 8 * 2 + 2;
				style.buttonMinWidth = 22 * 2;
				style.buttonMinHeight = 18 * 2;
				style.imageScale = 2;

				style.SetFont("PressStart2P", 8 * 2);

				style.SetShadowTexture("gui/images/bar");

				GUI.styles.Add(style); //MediumFont
			}

			{
				GUI.Style style = new GUI.Style();

				style.labelSpacing = style.windowSpacing = 2 * 2;
				style.textMinHeight = 8 * 2 + 2;
				style.buttonMinWidth = 22 * 2;
				style.buttonMinHeight = 18 * 2;
				style.imageScale = 2;

				style.SetFont("PressStart2P", 8 * 2);

				style.SetShadowTexture("gui/images/bar");

				GUI.styles.Add(style); //Keyboard
			}

			{
				GUI.Style style = new GUI.Style();

				style.labelSpacing = style.windowSpacing = 2;
				style.imageMinHeight = 16;
				style.buttonMinWidth = 8 * 8;
				style.buttonMinHeight = 8 * 5;
				style.textUpperCase = true;

				style.imageScale = 1;

				style.SetFont("PressStart2P");

				style.SetShadowTexture("gui/images/bar");

				GUI.styles.Add(style); //Button
			}

			{
				GUI.Style style = new GUI.Style();

				style.labelSpacing = style.windowSpacing = 2;
				style.imageMinHeight = 16;
				style.buttonMinWidth = 8 * 5;
				style.buttonMinHeight = 8 * 5;
				style.textUpperCase = true;

				style.imageScale = 1;

				style.SetFont("PressStart2P");

				style.SetShadowTexture("gui/images/bar");

				GUI.styles.Add(style); //BoxButton
			}
#endif
		}
	}
}
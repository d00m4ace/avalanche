using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIButton : GUIElement
	{
		public Game.ButtonID buttonID;

		public GUIElement element;

		public int id;

		public GUIStyle unpressed;
		public GUIStyle pressed;
		public GUIStyle unpressedShine;
		public GUIStyle pressedShine;
		public GUIStyle focus;

		public int buttonPressedXOffset = 0, buttonPressedYOffset = 2;

		public Color backColor;

		public Game.CollectionID clickSound;

		public Rect screenRect;

		public bool defaultFocus;

		public GUIButton(Game.ButtonID buttonID, GUIElement element = default(GUIElement), Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue, int id = 0, bool defaultFocus = false) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
			if(animation != null)
				this.animation = animation;

			this.buttonID = buttonID;
			this.id = id;

			this.defaultFocus = defaultFocus;

			this.element = element;

			this.style = style;
			SetStyle();

			if(width != -1)
				this.minWidth = this.maxWidth = width;
			if(height != -1)
				this.minHeight = this.maxHeight = height;
		}

		public override bool IsPlayingAnimation()
		{
			if(isDisabled)
				return false;

			return animation.IsPlaying() || element.IsPlayingAnimation();
		}

		public override void StopgAnimation()
		{
			animation.Stop();
			element.StopgAnimation();
		}

		public override void ResumeAnimation()
		{
			animation.Resume();
			element.ResumeAnimation();
		}

		public override void RestartAnimation()
		{
			animation.Restart();
			element.RestartAnimation();
		}

		public override void PlayInverseAnimation()
		{
			animation.PlayInverse();
			element.PlayInverseAnimation();
		}

		public override void SetStyle()
		{
			unpressed = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].buttonUnpressedStyle];
			pressed = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].buttonPressedStyle];
			unpressedShine = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].buttonUnpressedShineStyle];
			pressedShine = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].buttonPressedShineStyle];
			focus = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].buttonFocusStyle];

			backColor = GUI.styles[(int)style].buttonBackColor;

			clickSound = GUI.styles[(int)style].buttonClickSound;

			if(GUI.styles[(int)style].buttonMinWidth != int.MinValue)
				minWidth = GUI.styles[(int)style].buttonMinWidth;
			if(GUI.styles[(int)style].buttonMinHeight != int.MinValue)
				minHeight = GUI.styles[(int)style].buttonMinHeight;
			if(GUI.styles[(int)style].buttonMaxWidth != int.MaxValue)
				maxWidth = GUI.styles[(int)style].buttonMaxWidth;
			if(GUI.styles[(int)style].buttonMaxHeight != int.MaxValue)
				maxHeight = GUI.styles[(int)style].buttonMaxHeight;

			buttonPressedXOffset = GUI.styles[(int)style].buttonPressedXOffset;
			buttonPressedYOffset = GUI.styles[(int)style].buttonPressedYOffset;

			if(style != Game.GUIStyle.Default)
			{
				if(element.style == Game.GUIStyle.Default)
				{
					element.style = style;
					element.SetStyle();
				}
			}
		}

		public override void RestSize()
		{
			width = height = 0;
			element.RestSize();
		}

		public override void CalcWidth()
		{
			if(width == 0)
			{
				element.CalcWidth();
				width = element.GetWidth() + unpressed.border.left + unpressed.border.right;
			}
		}

		public override void CalcHeight()
		{
			if(height == 0)
			{
				element.CalcHeight();
				height = element.GetHeight() + unpressed.border.top + unpressed.border.bottom;
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

			bool pushed = false;

			if(GUI.isFocusWindow)
				GUI.focusedButtons.Add(this);

			screenRect = GUI.GetScreenRect(pos);

			if(GUI.buttonPushed == this)
			{
				pushed = true;
				if(GUI.playSound && clickSound != Game.CollectionID.none) Sound.Play(clickSound);
			}
			else if(!GUI.isCursorFree)
			{
				if(GUI.NoClip(GUI.cursorPos) && screenRect.Contains(GUI.cursorPos))
				{
					if(GUI.isCursorDown)
						pushed = true;

					if(GUI.isCursorFireUp)
					{
						GUI.buttonPushed = this;
						GUI.isCursorFree = true;

						if(GUI.playSound && clickSound != Game.CollectionID.none) Sound.Play(clickSound);
					}
				}
			}

			{
				int borderBorder = (int)(10 * GUI.scale);

				Rect buttonArea = new Rect(pos.x - borderBorder, pos.y - borderBorder, w + borderBorder * 2, h + borderBorder * 2);

				if(buttonArea.Contains(GUI.cursorPos))
					GUI.isCursorInButtonArea = true;
			}

			if(!pushed)
			{
				GUI.SetColor(animation.AnimateBackColor(backColor));
				UnityEngine.GUI.Box(pos, "", unpressed);

				GUI.SetColor(Color.White);
				UnityEngine.GUI.Box(pos, "", unpressedShine);

				if(GUI.focusedButton == this && focus != null && focus.normal.background != null)
				{
					GUI.SetColor(GUI.GetFocusColor());
					UnityEngine.GUI.Box(pos, "", focus);
				}
			}
			else
			{
				GUI.SetColor(animation.AnimateBackColor(backColor));
				UnityEngine.GUI.Box(pos, "", pressed);

				GUI.SetColor(Color.White);
				UnityEngine.GUI.Box(pos, "", pressedShine);
			}

			if(pushed)
				pos = new Rect(pos.x + buttonPressedXOffset, pos.y + buttonPressedYOffset, w, h);

			element.x = (int)pos.x + unpressed.border.left + (w - unpressed.border.left - unpressed.border.right - element.width) / 2;
			element.y = (int)pos.y + unpressed.border.top + (h - unpressed.border.top - unpressed.border.bottom - element.height) / 2;
			element.OnGUI();

			GUI.baseColor = guiBaseColor;

			animation.OnGUIEnd();
		}
	}
}
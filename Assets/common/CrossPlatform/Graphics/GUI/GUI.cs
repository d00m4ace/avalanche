using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUI
	{
		public static List<GUIWindow> windows;

		public static List<Rect> guiClip;

		public static GUIButton buttonPushed;

		public static List<GUIButton> focusedButtons;
		public static GUIButton focusedButton;

		public static List<GUIBubble> frontBubbles;
		public static List<GUIBubble> backBubbles;

		public static List<Style> styles;

		public class Style
		{
			public Color imageColor = Color.White;
			public Color imageShadowColor = Color.DarkGray;
			public int imageShadowXOffset = 1, imageShadowYOffset = 1;
			public int imageScale = 1;
			public int imageMinWidth = int.MinValue, imageMinHeight = int.MinValue;
			public int imageMaxWidth = int.MaxValue, imageMaxHeight = int.MaxValue;

			public Color progressBarColor = Color.Gold, progressBackColor = Color.LightBlue;
			public int progressBarMinWidth = int.MinValue, progressBarMinHeight = int.MinValue;
			public int progressBarMaxWidth = int.MaxValue, progressBarMaxHeight = int.MaxValue;
			public GUISkinCustomStyle progressBackStyle = GUISkinCustomStyle.ProgressBack;
			public GUISkinCustomStyle progressBackShineStyle = GUISkinCustomStyle.ProgressBarShine;
			public GUISkinCustomStyle progressBarStyle = GUISkinCustomStyle.ProgressBar;
			public GUISkinCustomStyle progressBarShineStyle = GUISkinCustomStyle.ProgressBarShine;

			public GUIStyle font;
			public Color textColor = Color.White;
			public Color textShadowColor = Color.DarkGray;
			public int textShadowXOffset = 1, textShadowYOffset = 1;
			public int textMinWidth = int.MinValue, textMinHeight = int.MinValue;
			public int textMaxWidth = int.MaxValue, textMaxHeight = int.MaxValue;
			public bool textUpperCase = false;

			public Color labelBackColor = Color.LightBlue;
			public Color labelBorderColor = Color.White;
			public int labelSpacing = 2;
			public int labelMinWidth = int.MinValue, labelMinHeight = int.MinValue;
			public int labelMaxWidth = int.MaxValue, labelMaxHeight = int.MaxValue;
			public GUISkinCustomStyle labelBackStyle = GUISkinCustomStyle.Empty;

			public Color buttonBackColor = Color.LightBlue;
			public Game.CollectionID buttonClickSound = Game.CollectionID.sound_click;
			public int buttonPressedXOffset = 0, buttonPressedYOffset = 2;
			public int buttonMinWidth = 24, buttonMinHeight = 24;
			public int buttonMaxWidth = int.MaxValue, buttonMaxHeight = int.MaxValue;
			public GUISkinCustomStyle buttonUnpressedStyle = GUISkinCustomStyle.Button;
			public GUISkinCustomStyle buttonPressedStyle = GUISkinCustomStyle.ButtonPressed;
			public GUISkinCustomStyle buttonUnpressedShineStyle = GUISkinCustomStyle.Shine;
			public GUISkinCustomStyle buttonPressedShineStyle = GUISkinCustomStyle.ShinePressed;
			public GUISkinCustomStyle buttonFocusStyle = GUISkinCustomStyle.ButtonFocussed;

			public Color windowBackColor = Color.LightBlue;
			public Color windowBorderColor = Color.White;
			public Texture2D windowShadowTexture;
			public Color windowShadowTextureColor = Color.LightBlue * Color.WhiteAlpha50;
			public int windowSpacing = 2;
			public int windowMinWidth = int.MinValue, windowMinHeight = int.MinValue;
			public int windowMaxWidth = int.MaxValue, windowMaxHeight = int.MaxValue;
			public GUISkinCustomStyle windowBackStyle = GUISkinCustomStyle.Window;
			public GUISkinCustomStyle windowBackShineStyle = GUISkinCustomStyle.Shine;

			public void SetShadowTexture(string image) { windowShadowTexture = Render.GetTexture(image); }

			public void SetFont(string font, int fontSize = 0)
			{
				this.font = new GUIStyle();
				this.font.font = Render.GetFont(font);
				this.font.fontSize = fontSize;
				this.font.normal.textColor = UnityEngine.Color.white;
				this.font.clipping = TextClipping.Clip;
				this.font.stretchWidth = false;
				this.font.stretchHeight = false;
				this.font.richText = true;
			}
		}

		public enum GUISkinCustomStyle
		{
			Empty,
			Button,
			ButtonPressed,
			ButtonFocussed,
			Window,
			Block,
			Shine,
			ShinePressed,
			WindowBorder,
			ProgressBack,
			ProgressBar,
			ProgressBarShine,
			Thumb,
		};

		public static bool playSound = true;

		public static void Setup()
		{
			if(!Input.gyro.enabled)
				Input.gyro.enabled = true;

			windows = new List<GUIWindow>();
			styles = new List<Style>();
			guiClip = new List<Rect>();
			focusedButtons = new List<GUIButton>();
			buttonPushed = null;
			frontBubbles = new List<GUIBubble>();
			backBubbles = new List<GUIBubble>();

			SetOptimalScale();
		}

		public static void Rest()
		{
			windows.Clear();
			guiClip.Clear();

			focusedButtons.Clear();
			buttonPushed = null;

			RemoveAllBackBubbles();
			RemoveAllFrontBubbles();

			SetOptimalScale();
		}

		public static void SetOptimalScale()
		{
			float currentAspect = (float)Screen.width / Screen.height;

			float scale;

			if(currentAspect > 1)
				scale = Screen.height / 300.0f;
			else
				scale = Screen.width / 300.0f;

			if(scale >= 1.4f && scale < 1.9f) scale = 1.4f;
			else scale = (int)(scale + 0.15f);

			if(scale > 3) scale = 3;
			Console.WriteLine(scale);
			SetScale(scale);
		}

		public static bool IsPortrait()
		{
			return Screen.height > Screen.width;
		}

		public static void Add(GUIWindow window)
		{
			windows.Add(window);
		}

		public static void Remove(GUIWindow window)
		{
			windows.Remove(window);
		}

		public static void AddFront(GUIBubble bubble)
		{
			frontBubbles.Add(bubble);
		}

		public static void RemoveFront(GUIBubble bubble)
		{
			frontBubbles.Remove(bubble);
		}

		public static void RemoveAllFrontBubbles()
		{
			frontBubbles.Clear();
		}

		public static void AddBack(GUIBubble bubble)
		{
			backBubbles.Add(bubble);
		}

		public static void RemoveBack(GUIBubble bubble)
		{
			backBubbles.Remove(bubble);
		}

		public static void RemoveAllBackBubbles()
		{
			backBubbles.Clear();
		}

		public static bool hideFrontBubbles = false;
		public static void HideFrontBubbles(bool hide) { hideFrontBubbles = hide; }

		public static bool hideBackBubbles = false;
		public static void HideBackBubbles(bool hide) { hideBackBubbles = hide; }

		public static bool isEndAnyAnimations;
		public static void EndAnyAnimations() { isEndAnyAnimations = true; }

		public static GUIWindow focusWindow;
		public static bool isFocusWindow;

		public static int GetScreenWidth() { return (int)(Screen.width / scale); }
		public static int GetScreenHeight() { return (int)(Screen.height / scale); }

		public static void OnGUI(bool screenSizeChanged)
		{
			//Debug.Log(Event.current.type);

			//if(Event.current.type != EventType.Repaint )
			//return;

			if(Event.current.type == EventType.Layout)
				return;

			isCursorInButtonArea = false;

			focusColorAnimationTimer.OnGUIBegin();

			if(UnityEngine.GUI.skin != Render.guiSkin)
				UnityEngine.GUI.skin = Render.guiSkin;

			if(screenSizeChanged && !scaleFixed)
				SetOptimalScale();

			UnityEngine.GUI.matrix = matrix;

			guiClip.Clear();
			guiClip.Add(new Rect(0, 0, GetScreenWidth(), GetScreenHeight()));

			baseColor = Color.White;
			SetColor(Color.White);

			int len;

			if(screenSizeChanged)
			{
				len = backBubbles.Count;
				for(int i = 0; i < len; i++)
					backBubbles[i].RestSize();
			}

			if(!hideBackBubbles)
			{
				bool saveIsEndAnyAnimations = isEndAnyAnimations;
				isEndAnyAnimations = false;

				for(int i = 0; i < backBubbles.Count;)
				{
					backBubbles[i].OnGUI();

					if(!backBubbles[i].IsAlive())
					{
						backBubbles.RemoveAt(i);
						continue;
					}

					i++;
				}

				isEndAnyAnimations = saveIsEndAnyAnimations;
			}

			buttonPushed = null;

			len = windows.Count;
			focusWindow = null;

			for(int i = len - 1; i >= 0; i--)
				if(windows[i].focusWindow)
				{
					focusWindow = windows[i];
					break;
				}

			focusedButtons.Clear();

			if(screenSizeChanged)
			{
				for(int i = 0; i < len; i++)
					windows[i].RestSize();
			}

			for(int i = 0; i < len; i++)
			{
				isCursorFree = focusWindow != windows[i] || windows[i].animation.IsPlaying();

				isFocusWindow = focusWindow == windows[i];

				if(isFocusWindow && focusPushed && focusedButton != null)
					buttonPushed = focusedButton;

				windows[i].OnGUI();

				if(buttonPushed != null)
					focusedButton = buttonPushed;
			}

			MakeFocus();

			if(screenSizeChanged)
			{
				len = frontBubbles.Count;
				for(int i = 0; i < len; i++)
					frontBubbles[i].RestSize();
			}

			isEndAnyAnimations = false;

			if(!hideFrontBubbles)
			{
				for(int i = 0; i < frontBubbles.Count;)
				{
					frontBubbles[i].OnGUI();

					if(!frontBubbles[i].IsAlive())
					{
						frontBubbles.RemoveAt(i);
						continue;
					}

					i++;
				}
			}
		}

		public static void RestSize()
		{
			for(int i = 0; i < windows.Count; i++)
			{
				windows[i].RestSize();
			}
		}

		public static void MakeFocus()
		{
			bool findFocusedButton = false;

			GUIButton defaultFocusButton = null;

			{
				int ic = focusedButtons.Count;
				for(int i = 0; i < ic; i++)
				{
					if(focusedButtons[i] == focusedButton)
					{
						findFocusedButton = true;
						break;
					}

					if(focusedButtons[i].defaultFocus)
						defaultFocusButton = focusedButtons[i];
				}
			}

			if(!findFocusedButton)
				focusedButton = defaultFocusButton;

			if(focusedButton == null)
				return;

			if(focusMoveRight || focusMoveLeft || focusMoveUp || focusMoveDown)
				if(playSound) Sound.Play( Game.CollectionID.sound_select);

			if(focusMoveRight)
			{
				GUIButton buttonFind = MoveFocusRight(focusedButton.screenRect.x, focusedButton.screenRect.y);

				if(buttonFind != null)
					focusedButton = buttonFind;
				else
				{
					buttonFind = MoveFocusRight(-1, focusedButton.screenRect.y);

					if(buttonFind != null)
						focusedButton = buttonFind;
				}
			}

			if(focusMoveLeft)
			{
				GUIButton buttonFind = MoveFocusLeft(focusedButton.screenRect.x, focusedButton.screenRect.y);

				if(buttonFind != null)
					focusedButton = buttonFind;
				else
				{
					buttonFind = MoveFocusLeft(Screen.width, focusedButton.screenRect.y);

					if(buttonFind != null)
						focusedButton = buttonFind;
				}
			}

			if(focusMoveUp)
			{
				GUIButton buttonFind = MoveFocusUp(focusedButton.screenRect.x, focusedButton.screenRect.y);

				if(buttonFind != null)
					focusedButton = buttonFind;
				else
				{
					buttonFind = MoveFocusUp(focusedButton.screenRect.x, Screen.height);

					if(buttonFind != null)
						focusedButton = buttonFind;
				}
			}

			if(focusMoveDown)
			{
				GUIButton buttonFind = MoveFocusDown(focusedButton.screenRect.x, focusedButton.screenRect.y);

				if(buttonFind != null)
					focusedButton = buttonFind;
				else
				{
					buttonFind = MoveFocusDown(focusedButton.screenRect.x, -1);

					if(buttonFind != null)
						focusedButton = buttonFind;
				}
			}
		}

		public static GUIButton MoveFocusRight(float x, float y)
		{
			GUIButton buttonFind = null;

			int ic = focusedButtons.Count;
			for(int i = 0; i < ic; i++)
			{
				if(focusedButtons[i] == focusedButton)
					continue;

				if(focusedButtons[i].screenRect.x > x && (buttonFind == null || Mathf.Abs(focusedButtons[i].screenRect.x - x) + Mathf.Abs(focusedButtons[i].screenRect.y - y) * Screen.width < Mathf.Abs(buttonFind.screenRect.x - x) + Mathf.Abs(buttonFind.screenRect.y - y) * Screen.width))
					buttonFind = focusedButtons[i];
			}

			return buttonFind;
		}

		public static GUIButton MoveFocusLeft(float x, float y)
		{
			GUIButton buttonFind = null;

			int ic = focusedButtons.Count;
			for(int i = 0; i < ic; i++)
			{
				if(focusedButtons[i] == focusedButton)
					continue;

				if(focusedButtons[i].screenRect.x < x && (buttonFind == null || Mathf.Abs(focusedButtons[i].screenRect.x - x) + Mathf.Abs(focusedButtons[i].screenRect.y - y) * Screen.width < Mathf.Abs(buttonFind.screenRect.x - x) + Mathf.Abs(buttonFind.screenRect.y - y) * Screen.width))
					buttonFind = focusedButtons[i];
			}

			return buttonFind;
		}

		public static GUIButton MoveFocusUp(float x, float y)
		{
			GUIButton buttonFind = null;

			int ic = focusedButtons.Count;
			for(int i = 0; i < ic; i++)
			{
				if(focusedButtons[i] == focusedButton)
					continue;

				if(focusedButtons[i].screenRect.y < y && (buttonFind == null || Mathf.Abs(focusedButtons[i].screenRect.x - x) + Mathf.Abs(focusedButtons[i].screenRect.y - y) * Screen.width < Mathf.Abs(buttonFind.screenRect.x - x) + Mathf.Abs(buttonFind.screenRect.y - y) * Screen.width))
					buttonFind = focusedButtons[i];
			}

			return buttonFind;
		}

		public static GUIButton MoveFocusDown(float x, float y)
		{
			GUIButton buttonFind = null;

			int ic = focusedButtons.Count;
			for(int i = 0; i < ic; i++)
			{
				if(focusedButtons[i] == focusedButton)
					continue;

				if(focusedButtons[i].screenRect.y > y && (buttonFind == null || Mathf.Abs(focusedButtons[i].screenRect.x - x) + Mathf.Abs(focusedButtons[i].screenRect.y - y) * Screen.width < Mathf.Abs(buttonFind.screenRect.x - x) + Mathf.Abs(buttonFind.screenRect.y - y) * Screen.width))
					buttonFind = focusedButtons[i];
			}

			return buttonFind;
		}

		public static Rect PeekGuiClip()
		{
			return guiClip[guiClip.Count - 1];
		}

		public static void BeginClip(Rect clip)
		{
			guiClip.Add(new Rect(guiClip[guiClip.Count - 1].x + clip.x, guiClip[guiClip.Count - 1].y + clip.y, clip.width, clip.height));
			UnityEngine.GUI.BeginGroup(clip);
		}

		public static void EndClip()
		{
			guiClip.RemoveAt(guiClip.Count - 1);
			UnityEngine.GUI.EndGroup();
		}

		public static bool NoClip(UnityEngine.Vector2 point)
		{
			int ic = guiClip.Count;
			for(int i = 0; i < ic; i++)
				if(!guiClip[i].Contains(point))
					return false;

			return true;
		}

		public static Rect GetScreenRect(Rect rect)
		{
			rect.x += guiClip[guiClip.Count - 1].x;
			rect.y += guiClip[guiClip.Count - 1].y;
			return rect;
		}

		public static bool scaleFixed;
		public static float scale;
		public static Matrix4x4 matrix;
		public static void SetScale(float scale, bool scaleFixed = false)
		{
			GUI.scaleFixed = scaleFixed;
			GUI.scale = scale;
			matrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
		}

		public static bool isCursorInButtonArea;
		public static bool isCursorFree;
		public static bool isCursorDown;
		public static bool isCursorFireUp;

		public static UnityEngine.Vector2 cursorPos;

		public static void UpdateCursor()
		{
			UnityEngine.Vector2 mousePos;
			mousePos.x = Input.mousePosition.x;
			mousePos.y = Input.mousePosition.y;
			mousePos.y = Screen.height - mousePos.y;
			mousePos.x /= scale;
			mousePos.y /= scale;
			cursorPos = mousePos;

			isCursorDown = Input.GetMouseButton(0);
			isCursorFireUp = Input.GetMouseButtonUp(0);
		}

		public static bool isAnyKeyDown;

		public static bool focusMoveUp, focusMoveDown, focusMoveLeft, focusMoveRight, focusPushed;

		public static float lastCursorPositionX, lastCursorPositionY;
		public static float deltaCursorPositionX, deltaCursorPositionY;

		static bool androidBackButtonPushed = false;

		static void UpdateAndroidGUI()
		{
			androidBackButtonPushed = false;

			if(Input.GetKeyUp(KeyCode.Escape))
			{
#if !UNITY_EDITOR
				if(Application.platform == RuntimePlatform.Android)
#endif
					androidBackButtonPushed = true;
			}
		}

		public static bool IsAndroidBackButtonPushed()
		{
			return androidBackButtonPushed;
		}

		public static void UpdateCursorAndFocus()
		{
			UpdateCursor();

			focusMoveLeft = Input.GetKeyDown(KeyCode.LeftArrow);
			focusMoveRight = Input.GetKeyDown(KeyCode.RightArrow);
			focusMoveUp = Input.GetKeyDown(KeyCode.UpArrow);
			focusMoveDown = Input.GetKeyDown(KeyCode.DownArrow);
			focusPushed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space);

			isAnyKeyDown = Input.anyKeyDown;

			deltaCursorPositionX = deltaCursorPositionY = 0;

			if(Input.GetMouseButtonDown(0))
			{
				lastCursorPositionX = Input.mousePosition.x;
				lastCursorPositionY = Input.mousePosition.y;
			}
			else
			{
				deltaCursorPositionX = Input.mousePosition.x - lastCursorPositionX;
				deltaCursorPositionY = Input.mousePosition.y - lastCursorPositionY;

				lastCursorPositionX = Input.mousePosition.x;
				lastCursorPositionY = Input.mousePosition.y;
			}

			UpdateAndroidGUI();
		}

		public static Color focusColor = Color.Yellow;
		public static Color baseColor;
		public static Color color;

		public static GUIAnimationTimer focusColorAnimationTimer = new GUIAnimationTimer(0.65f, false, true);

		public static void SetFocusColor(Color color) { focusColor = color; }
		public static Color GetFocusColor() { Color c = focusColor; c.a = 0; return Color.Lerp(c, focusColor, focusColorAnimationTimer.t); }

		public static void SetColor(Color color)
		{
			GUI.color = baseColor * color;
			Color32 c = new Color32(0,0,0,0);
			c.r = GUI.color.r; c.g = GUI.color.g; c.b = GUI.color.b; c.a = GUI.color.a;
			UnityEngine.GUI.color = c;
		}
	}
}
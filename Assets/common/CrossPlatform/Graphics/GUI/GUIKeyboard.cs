using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIKeyboard : GUIWindow
	{
		List<GUIButton> buttons;
		GUIWindow textLineWindow;
		GUIText textLine;

		StringBuilder sb;

		public class KeyboardLayout
		{
			public string[][] keys;
		}

		public static List<KeyboardLayout> keyboards;

		KeyboardLayout keyboard;

		int keyboardIndex;
		Layout layout;

		public int textMaxLength = 16;

		public enum Layout
		{
			Uppercase,
			Lowercase,
			Symbols,
		}

		public GUIKeyboard(Game.GUIStyle style = Game.GUIStyle.Empty, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(null, style, animation, width, height, minWidth, minHeight, maxWidth, maxHeight)
		{
			SetContentLayout(ContentLayout.Vertical);

			shadowBackTexture = null;

			focusWindow = true;

			buttons = new List<GUIButton>();

			sb = new StringBuilder();

			sb.Append(' ');

			if(keyboards == null)
			{
				keyboards = new List<KeyboardLayout>();

				{
					keyboard = new KeyboardLayout();

					keyboard.keys = new string[][]
					{
						new string[]
						{
							"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
							"A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
							"K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
							"U", "V", "W", "X", "Y", "Z", ".", "?", "!", "@",
							"-", "+", "_", " ", "←", "→", "◂", "¿", "€", "↰",
						},

						new string[]
						{
							"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
							"a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
							"k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
							"u", "v", "w", "x", "y", "z", ".", "?", "!", "@",
							"-", "+", "_", " ", "←", "→", "◂", "¿", "€", "↰",
						},

						new string[]
						{
							"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
							"!", "@", "#", "$", "%", "^", "&", "*", "(", ")",
							"-", "+", "{", "}", "[", "]", "|", "\\", "/", "©",
							"?", "<", ">", ":", ";", "'", "\"", "_", "~", "®",
							".", "™", "=", " ", "←", "→", "◂", "¿", "€", "↰",
						},
					};

					keyboards.Add(keyboard);
				}

				{
					keyboard = new KeyboardLayout();

					keyboard.keys = new string[][]
					{
						new string[]
						{
							"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
							"А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И",
							"Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т",
							"У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь",
							"Э", "Ю", "Я", " ", "←", "→", "◂", "¿", "€", "↰",
						},

						new string[]
						{
							"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
							"а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и",
							"й", "к", "л", "м", "н", "о", "п", "р", "с", "т",
							"у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь",
							"э", "ю", "я", " ", "←", "→", "◂", "¿", "€", "↰",
						},

						new string[]
						{
							"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
							"!", "@", "#", "$", "%", "^", "&", "*", "(", ")",
							"-", "+", "{", "}", "[", "]", "|", "\\", "/", "©",
							"?", "<", ">", ":", ";", "'", "\"", "_", "~", "®",
							".", "™", "=", " ", "←", "→", "◂", "¿", "€", "↰",
						},
					};

					keyboards.Add(keyboard);
				}

				{
					keyboard = new KeyboardLayout();

					keyboard.keys = new string[][]
					{
						new string[]
						{
							"Ñ", "Å", "Á", "É", "Í", "Ú", "Ý", "Ó", "Ť", "Ð",
							"A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
							"K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
							"U", "V", "W", "X", "Y", "Z", "Ä", "Ö", "Ü", "Æ",
							"Ø", "Š", "Ž", " ", "←", "→", "◂", "¿", "€", "↰",
						},

						new string[]
						{
							"ñ", "å", "á", "é", "í", "ú", "ý", "ó", "ť", "ð",
							"a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
							"k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
							"u", "v", "w", "x", "y", "z", "ä", "ö", "ü", "æ",
							"ø", "š", "ž", " ", "←", "→", "◂", "¿", "€", "↰",
						},

						new string[]
						{

							"1", "2", "3", "4", "5", "6", "7", "8", "9", "0",
							"!", "@", "#", "$", "%", "^", "&", "*", "(", ")",
							"-", "+", "{", "}", "[", "]", "|", "\\", "/", "©",
							"?", "<", ">", ":", ";", "'", "\"", "_", "~", "®",
							".", "™", "=", " ", "←", "→", "◂", "¿", "€", "↰",
						},
					};

					keyboards.Add(keyboard);
				}
			}

			keyboard = keyboards[keyboardIndex];

			Setup();
		}

		public override void RestSize()
		{
			RemoveKeyboardKeysWindows();
			AddKeyboardKeys();
			base.RestSize();
		}

		void SetupKeyboardKeys()
		{
			int keysCount = keyboard.keys[0].Length;
			for(int i = 0; i < keysCount; i++)
			{
				string key = keyboard.keys[(int)layout][i];

				GUIButton button = new GUIButton(Game.ButtonID.Keyboard, new GUIText(key), Game.GUIStyle.Keyboard);

				if(key == "↰")
				{
					button.animation = new GUIAnimationBlinkBackColor(0.5f, Color.Gold, false, true);
					button.defaultFocus = true;
				}
				else if(key == "←" || key == "→")
				{
					button.backColor = Color.RoyalBlue;
				}
				else if(key == "◂")
				{
					button.backColor = Color.DarkRed;
				}
				else if(key == "¿")
				{
					button.backColor = Color.Orange;
				}
				else if(key == "€")
				{
					button.backColor = Color.DarkGreen;
				}

				buttons.Add(button);
			}
		}

		void AddKeyboardKeys()
		{
			int keysPerRow = (int)(Screen.width / ((buttons[0].GetWidth() + 2) * GUI.scale));

			if(keysPerRow > 10)
				keysPerRow = 10;

			int keysCount = keyboard.keys[0].Length;
			int i = 0;

			while(true)
			{
				GUIWindow window = new GUIWindow(style: Game.GUIStyle.Empty);

				for(int x = 0; x < keysPerRow; x++)
				{
					window.Add(buttons[i++]);

					if(i == keysCount)
						break;
				}

				Add(window);

				if(i == keysCount)
					break;
			}
		}

		void RemoveKeyboardKeysWindows()
		{
			while(elements.Count != 1)
			{
				((GUIWindow)elements[1]).FreeAllElements();
				elements.RemoveAt(1);
			}
		}

		public void Setup()
		{
			{
				GUIWindow window = textLineWindow = new GUIWindow(style: Game.GUIStyle.Empty);
				window.paddingHeight = 4;
				window.Add(new GUILabel(new GUIElement[] { textLine = new GUIText(sb.ToString()) }, Game.GUIStyle.Keyboard));
				Add(window);
			}
			SetupKeyboardKeys();
			AddKeyboardKeys();
		}

		public void SetLayout()
		{
			for(int i = 0; i < buttons.Count; i++)
			{
				((GUIText)buttons[i].element).SetText(keyboard.keys[(int)layout][i]);
				buttons[i].RestSize();
			}
		}

		int cursor;
		char cursorChar;
		float cursorBlinkTime;

		public override void OnGUI()
		{
			base.OnGUI();

			if(GUI.buttonPushed != null && GUI.buttonPushed.buttonID == Game.ButtonID.Keyboard)
			{
				string key = ((GUIText)GUI.buttonPushed.element).text;

				if(key == "¿")
				{
					layout = (Layout)((((int)layout) + 1) % 3);
					SetLayout();
				}
				else if(key == "€")
				{
					keyboardIndex = (++keyboardIndex) % keyboards.Count;
					keyboard = keyboards[keyboardIndex];
					SetLayout();
				}
				else if(key == "←")
				{
					RemoveCursorChar();

					cursor--;
					if(cursor < 0)
						cursor = 0;

					cursorBlinkTime = 1;
				}
				else if(key == "→")
				{
					RemoveCursorChar();

					cursor++;
					if(cursor >= sb.Length)
						cursor = sb.Length - 1;

					cursorBlinkTime = 1;
				}
				else if(key == "◂")
				{
					RemoveCursorChar();

					if(cursor > 0)
					{
						sb.Remove(cursor - 1, 1);
						cursor--;
					}

					if(sb.Length == 0)
						sb.Append(' ');

					cursorBlinkTime = 1;
				}
				else if(key == "↰")
				{
					return;
				}
				else
				{
					if(sb.Length <= textMaxLength)
					{
						RemoveCursorChar();
						sb.Insert(cursor, key);
						cursor++;

						cursorBlinkTime = 1;
					}
				}

				GUI.buttonPushed = null;
			}

			cursorBlinkTime += Time.deltaTime;

			if(cursorBlinkTime > 0.35f)
			{
				if(cursorChar == 0)
				{
					cursorChar = sb[cursor];
					sb[cursor] = 'Ѱ';

					textLine.SetText(GetCursorString());
					textLineWindow.RestSize();
				}
				else
				{
					sb[cursor] = cursorChar;
					cursorChar = (char)0;

					textLine.SetText(GetCursorString());
					textLineWindow.RestSize();
				}

				cursorBlinkTime = 0;
			}
		}

		public string GetText()
		{
			RemoveCursorChar();
			return sb.ToString(0, sb.Length - 1);
		}

		public void SetText(string text)
		{
			RemoveCursorChar();
			sb.Length = 0;
			sb.Append(text);
			sb.Append(' ');
			cursor = sb.Length - 1;
		}

		void RemoveCursorChar()
		{
			if(cursorChar != 0)
			{
				sb[cursor] = cursorChar;
				cursorChar = (char)0;
			}
		}

		int stringViewMaxLength = 24;

		string GetCursorString()
		{
			int start = 0, len;

			if(cursor > stringViewMaxLength)
				start = cursor - stringViewMaxLength;

			len = stringViewMaxLength + 1;

			if(start + len > sb.Length)
				len = sb.Length - start;

			return sb.ToString(start, len);
		}
	}
}
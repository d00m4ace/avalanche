using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HEXPLAY
{
	public static class Utils
	{
		public static int Locate(byte[] data, int dataLength, int startPosition, byte[] candidate)
		{
			for(int i = startPosition; i < dataLength; i++)
			{
				if(!IsMatch(data, dataLength, i, candidate))
					continue;

				return i;
			}

			return -1;
		}

		public static bool IsMatch(byte[] data, int dataLength, int startPosition, byte[] candidate)
		{
			if(candidate.Length > (dataLength - startPosition))
				return false;

			for(int i = 0; i < candidate.Length; i++)
				if(data[startPosition + i] != candidate[i])
					return false;

			return true;
		}

		static StringBuilder sb = new StringBuilder();

		public static byte[] Xor(this byte[] bytes, byte[] key)
		{
			unchecked
			{
				for(int i = 0; i < bytes.Length; i++)
					bytes[i] ^= key[i % key.Length];
			}
			return bytes;
		}

		public static byte[] BlockCopy(this byte[] bytes)
		{
			byte[] copy = new byte[bytes.Length];
			Buffer.BlockCopy(bytes, 0, copy, 0, bytes.Length);
			return copy;
		}

		public static bool IsEqual(this byte[] a1, byte[] a2)
		{
			if(a1.Length != a2.Length)
				return false;

			for(int i = 0; i < a1.Length; i++)
				if(a1[i] != a2[i])
					return false;

			return true;
		}

		public static string ToHexString(this byte[] bytes, string format = "X2")
		{
			sb.Length = 0;
			for(int i = 0; i < bytes.Length; i++)
				sb.Append(bytes[i].ToString(format));
			return sb.ToString();
		}

		public static string ToBase64(this string s)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
		}

		public static string ToBase64(this byte[] b)
		{
			return Convert.ToBase64String(b);
		}

		public static byte[] FromBase64(this string s)
		{
			return Convert.FromBase64String(s);
		}

		public static string StringFromBase64(this string s)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(s));
		}

		public static byte[] ToUTF8Bytes(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		public static string ToUTF8String(this byte[] b)
		{
			return Encoding.UTF8.GetString(b);
		}

		public class Array<T> where T : class
		{
			T[] array;
			int nextID = 1;

			public Array(int startLength = 100) { array = new T[startLength]; }

			public int NextID { get { return nextID; } }

			public T this[int i]
			{
				get { return 0 < i && i < array.Length ? array[i] : null; }
				set { array[i] = value; }
			}

			public int CreateNextID(int addLength = 100)
			{
				if(nextID == array.Length)
				{
					T[] newArray = new T[array.Length + addLength];
					array.CopyTo(newArray, 0);
					array = newArray;
				}

				return nextID++;
			}

			public T AddNext(T t) { return array[NextID - 1] = t; }
		}

		public static bool Has(this int a, int f) { return (a & f) != 0; }

		public static int Abs(this int i)
		{
			return i >= 0 ? i : -i;
		}

#if !SERVER
		public static UnityEngine.Color32 GetColor32(this Color color)
		{
			UnityEngine.Color32 c = new Color32(0,0,0,0);
			c.r = color.r; c.g = color.g; c.b = color.b; c.a = color.a;
			return c;
		}
#endif

		public static string GetText(this Game.TEXT text)
		{
			if(Game.text.ContainsKey(text))
				return Game.text[text];
			return "";
		}

		public static void Write(this MemoryBuffer mb, Game.TEXT text)
		{
			int i = (int)text;
			mb.Write(i);
		}

		public static bool Read(this MemoryBuffer mb, out Game.TEXT text)
		{
			text = Game.TEXT.None;
			int i;

			if(!mb.Read(out i))
				return false;

			text = (Game.TEXT)i;
			return true;
		}

		public static void Write(this MemoryBuffer mb, Game.CollectionID id)
		{
			int i = (int)id;
			mb.Write(i);
		}

		public static bool Read(this MemoryBuffer mb, out Game.CollectionID id)
		{
			id = Game.CollectionID.none;
			int i;

			if(!mb.Read(out i))
				return false;

			id = (Game.CollectionID)i;
			return true;
		}

		public static void Write(this MemoryBuffer mb, Game.ObjectType id)
		{
			int i = (int)id;
			mb.Write(i);
		}

		public static bool Read(this MemoryBuffer mb, out Game.ObjectType id)
		{
			id = Game.ObjectType.None;
			int i;

			if(!mb.Read(out i))
				return false;

			id = (Game.ObjectType)i;
			return true;
		}

		public static string GetDateTimeStamp()
		{
			return DateTime.Now.ToString("ddMMyyHHmmssffff");
		}

		public static string TimeToMMSSmm(Fixed time)
		{
			int timeMinutes = ((int)time / 60) % 60;
			int timeSeconds = (int)time % 60;
			int timeMilliseconds = (int)(time * 100) % 100;

			sb.Length = 0;
			sb.Append(timeMinutes.ToString("00"));
			sb.Append(':');
			sb.Append(timeSeconds.ToString("00"));
			sb.Append(':');
			sb.Append(timeMilliseconds.ToString("00"));

			return sb.ToString();
		}

		public static string TimeToMMSS(Fixed time)
		{
			int timeMinutes = ((int)time / 60) % 60;
			int timeSeconds = (int)time % 60;

			sb.Length = 0;
			sb.Append(timeMinutes.ToString("00"));
			sb.Append(':');
			sb.Append(timeSeconds.ToString("00"));

			return sb.ToString();
		}

		public static void Shuffle<T>(this IList<T> list, PseudoRandom random)
		{
			int n = list.Count;
			while(n > 1)
			{
				n--;
				int k = random.Random(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static void FileWrite(string dir, string file, byte[] buffer)
		{
#if !SERVER
			string dirName = UnityEngine.Application.persistentDataPath + dir;
#else
			string dirName = dir;
#endif

			if(!Directory.Exists(dirName))
				Directory.CreateDirectory(dirName);

			string fileName = dirName + '/' + file;

			using(FileStream fs = File.Create(fileName))
			{
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
			}
		}

		public static bool FileRead(string dir, string file, out byte[] buffer)
		{
#if !SERVER
			string dirName = UnityEngine.Application.persistentDataPath + dir;
#else
			string dirName = dir;
#endif

			string fileName = dirName + '/' + file;

			if(File.Exists(fileName))
			{
				using(FileStream fs = File.Open(fileName, FileMode.Open))
				{
					buffer = new byte[fs.Length];

					fs.Read(buffer, 0, buffer.Length);
					fs.Close();
				}

				return true;
			}

			buffer = null;
			return false;
		}

#if !SERVER
		public static bool LoadAsset(string dir, string file, out byte[] buffer)
		{
			UnityEngine.TextAsset bindata = UnityEngine.Resources.Load<UnityEngine.TextAsset>(dir + '/' + file);

			if(bindata == null)
			{
				buffer = null;
				return false;
			}

			buffer = bindata.bytes;
			return true;
		}
#endif

		public static void LoadText(string locale)
		{
#if !SERVER
			byte[] bytes;
			LoadAsset("text", locale, out bytes);

			int start = 0, len = -1, s = 0;
			string[] str = new string[] { null, null };

			for(int i = 0; i < bytes.Length; i++)
			{
				if(bytes[i] == 0x0D && bytes[i + 1] == 0x0A)
				{
					len = i - 1 - start + 1;
					i += 2;
				}
				else if(i == bytes.Length - 1)
					len = i - start + 1;

				if(len != -1)
				{
					str[s] = Encoding.UTF8.GetString(bytes, start, len);
					start = i;
					len = -1;
					s++;
				}

				if(s == 2)
				{
					s = 0;

					if(locale != "en")
					{
						if(str[0] == "Copyright" || str[0] == "CreditsText")
							continue;
					}

					if(Enum.IsDefined(typeof(Game.TEXT), str[0]))
					{
						//TEXT t = (TEXT)Enum.Parse(typeof(TEXT), str[0]);
						//Console.WriteLine("Set TEXT[{0}] = {1}", t, str[1]);
						Game.text[(Game.TEXT)Enum.Parse(typeof(Game.TEXT), str[0])] = str[1].Replace("[>]", "\n");
					}
					else
						Console.WriteLine("Enum TEXT not found for : " + str[0] + " in locale " + locale);
				}
			}
#endif
		}

		public static void SetLanguage()
		{
#if !SERVER
			string[] language = { "en", "ru", "de", "fr", "it", "es", "zh-CN", "zh-TW", "ko", "ja", "th", "vi", "ar", "id", "pl", "no", "pt", "fi", "nl", "cs", "ca", "el", "is", "tr", "sv" };

			if(Game.settings.language == 0)
				Game.settings.language = GetSystemLanguage() + 1;

			LoadText(language[Game.settings.language - 1]);
#else
			Utils.LoadText("en");
#endif
		}

		public static int GetSystemLanguage()
		{
#if !SERVER
			switch(UnityEngine.Application.systemLanguage)
			{
				case UnityEngine.SystemLanguage.English: return 0;
				case UnityEngine.SystemLanguage.Russian: return 1;
				case UnityEngine.SystemLanguage.German: return 2;
				case UnityEngine.SystemLanguage.French: return 3;
				case UnityEngine.SystemLanguage.Italian: return 4;
				case UnityEngine.SystemLanguage.Spanish: return 5;
				case UnityEngine.SystemLanguage.Chinese: return 6;
				case UnityEngine.SystemLanguage.ChineseSimplified: return 6;
				case UnityEngine.SystemLanguage.ChineseTraditional: return 7;
				case UnityEngine.SystemLanguage.Korean: return 8;
				case UnityEngine.SystemLanguage.Japanese: return 9;
				case UnityEngine.SystemLanguage.Thai: return 10;
				case UnityEngine.SystemLanguage.Vietnamese: return 11;
				case UnityEngine.SystemLanguage.Arabic: return 12;
				case UnityEngine.SystemLanguage.Indonesian: return 13;
				case UnityEngine.SystemLanguage.Polish: return 14;
				case UnityEngine.SystemLanguage.Norwegian: return 15;
				case UnityEngine.SystemLanguage.Portuguese: return 16;
				case UnityEngine.SystemLanguage.Finnish: return 17;
				case UnityEngine.SystemLanguage.Dutch: return 18;
				case UnityEngine.SystemLanguage.Czech: return 19;
				case UnityEngine.SystemLanguage.Catalan: return 20;
				case UnityEngine.SystemLanguage.Greek: return 21;
				case UnityEngine.SystemLanguage.Icelandic: return 22;
				case UnityEngine.SystemLanguage.Turkish: return 23;
				case UnityEngine.SystemLanguage.Swedish: return 24;
				default: return 0;
			}
#else
			return 0;
#endif
		}

		public static int GetLanguageDefaultFontSize()
		{
			return GetLanguageDefaultFontSize(Game.settings.language - 1);
		}

		public static int GetLanguageDefaultFontSize(int language)
		{
#if !SERVER
			switch(language)
			{
				case 6: return 16;
				case 7: return 16;
				case 8: return 16;
				case 9: return 16;
				case 10: return 16;
				case 12: return 16;
				default: return 8;
			}
#else
			return 0;
#endif
		}
	}
}
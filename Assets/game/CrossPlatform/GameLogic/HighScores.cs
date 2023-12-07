using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class HighScores
	{
		public HEXInt socre;
		public int heroID;
		public string name;
		public string replayFileName;

		public void Write(MemoryBuffer mb)
		{
			mb.Write(socre);
			mb.Write(heroID);
			mb.Write(name);
			mb.Write(replayFileName);
		}

		public bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out socre))
				return false;

			if(!mb.Read(out heroID))
				return false;

			if(!mb.Read(out name))
				return false;

			if(!mb.Read(out replayFileName))
				return false;

			return true;
		}

		public string GetNameWithDots()
		{
			return name + "........".Substring(name.Length);
		}

		public static int FindPlace(List<HighScores> highScores, int score)
		{
			int i;

			for(i = 0; i < highScores.Count; i++)
				if(score > highScores[i].socre)
					return i;

			return i;
		}

		public static void Add(List<HighScores> highScores, HighScores newHighScores)
		{
			int place = FindPlace(highScores, newHighScores.socre);
			highScores.Insert(place, newHighScores);
		}

		public static void RemoveAllBelowN(List<HighScores> highScores, int n)
		{
			if(highScores.Count <= n)
				return;

			highScores.RemoveRange(n, highScores.Count - n);
		}

		public static void SaveReplay(string dir, string replayFileName, MemoryBuffer mb)
		{
			Utils.FileWrite(dir, replayFileName, mb.buffer);
		}

		public static bool LoadReplay(string dir, string replayFileName, MemoryBuffer mb)
		{
			mb.Rest();

			byte[] buffer;
			if(!Utils.FileRead(dir, replayFileName, out buffer))
				return false;

			mb.Set(buffer);
			buffer = null;
			return true;
		}

		public static void Save(string dir, List<HighScores> highScores)
		{
			using(MemoryBuffer mb = new MemoryBuffer(1024))
			{
				mb.Write(highScores, (b, x) => x.Write(b));
				Utils.FileWrite(dir, "HighScores", mb.buffer);
			}
		}

		public static void Load(string dir, List<HighScores> highScores)
		{
			byte[] buffer;
			if(!Utils.FileRead(dir, "HighScores", out buffer))
				return;

			using(MemoryBuffer mb = new MemoryBuffer(buffer))
			{
				mb.Read(highScores, (b) => { HighScores hs = new HighScores(); if(hs.Read(b)) return hs; return null; });
			}

			buffer = null;
		}

		public static void DeleteDropout(string top, List<HighScores> highScores)
		{
			string dirName = Application.persistentDataPath + top;

			if(!Directory.Exists(dirName))
				return;

			string[] files = Directory.GetFiles(dirName, "*.replay");

			int ic = files.Length;
			for(int i = 0; i < ic; i++)
			{
				//Console.WriteLine(file);
				string replayFileName = Path.GetFileNameWithoutExtension(files[i]) + ".replay";
				bool foundReplay = false;

				int jc = highScores.Count;
				for(int j = 0; j < jc; j++)
				{
					if(replayFileName == highScores[j].replayFileName)
					{
						foundReplay = true;
						break;
					}
				}

				if(!foundReplay)
				{
					File.Delete(files[i]);
					//Console.WriteLine("Delete : "+ replayFileName);
				}
			}
		}
	}
}
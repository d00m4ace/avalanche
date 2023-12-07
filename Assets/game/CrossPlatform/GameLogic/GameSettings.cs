using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace HEXPLAY
{
	public partial class Game
	{
		public class Settings
		{
			public int language = 0;

			public bool soundOn = true;
			public bool musicOn = true;
			public bool shadowsOn = true;

			public HEXInt coins = 0;
			public int heroSelected = 1;
			public string playerName = "Player";

			public HEXInt distance = 100;

			public bool faceCamOn = true;
			public bool videoRecOn = true;

			public bool socialSignIn = true;

			public bool achievement1000Points = false;
			public bool achievement2000Points = false;
			public bool achievement5000Points = false;
			public bool achievement10000Points = false;
			public bool achievement50000Points = false;

			public int gameRunCount = 0;

			public bool rateUsCompleted = false;

			public bool hasIAP = false;
		}

		public static void WriteSettings(MemoryBuffer mb)
		{
			mb.Write(settings.language);
			mb.Write(settings.soundOn);
			mb.Write(settings.musicOn);
			mb.Write(settings.shadowsOn);
			mb.Write(settings.coins);
			mb.Write(settings.heroSelected);
			mb.Write(settings.playerName);
			mb.Write(settings.distance);
			mb.Write(settings.faceCamOn);
			mb.Write(settings.videoRecOn);
			mb.Write(settings.socialSignIn);
			mb.Write(settings.achievement1000Points);
			mb.Write(settings.achievement2000Points);
			mb.Write(settings.achievement5000Points);
			mb.Write(settings.achievement10000Points);
			mb.Write(settings.achievement50000Points);
			mb.Write(settings.gameRunCount);
			mb.Write(settings.rateUsCompleted);
			mb.Write(settings.hasIAP);
		}

		public static bool ReadSettings(MemoryBuffer mb)
		{
			if(!mb.Read(out settings.language))
				return false;

			if(!mb.Read(out settings.soundOn))
				return false;

			if(!mb.Read(out settings.musicOn))
				return false;

			if(!mb.Read(out settings.shadowsOn))
				return false;

			if(!mb.Read(out settings.coins))
				return false;

			if(!mb.Read(out settings.heroSelected))
				return false;

			if(!mb.Read(out settings.playerName))
				return false;

			if(!mb.Read(out settings.distance))
				return false;

			if(!mb.Read(out settings.faceCamOn))
				return false;

			if(!mb.Read(out settings.videoRecOn))
				return false;

			if(!mb.Read(out settings.socialSignIn))
				return false;

			if(!mb.Read(out settings.achievement1000Points))
				return false;

			if(!mb.Read(out settings.achievement2000Points))
				return false;

			if(!mb.Read(out settings.achievement5000Points))
				return false;

			if(!mb.Read(out settings.achievement10000Points))
				return false;

			if(!mb.Read(out settings.achievement50000Points))
				return false;

			if(!mb.Read(out settings.gameRunCount))
				return false;

			if(!mb.Read(out settings.rateUsCompleted))
				return false;

			if(!mb.Read(out settings.hasIAP))
				return false;

			return true;
		}

		public static void SaveSettings()
		{
			using(MemoryBuffer mb = new MemoryBuffer(1024))
			{
				WriteSettings(mb);
				Utils.FileWrite("", "Settings", mb.buffer);
			}
		}

		public static void LoadSettings()
		{
			byte[] buffer;
			if(!Utils.FileRead("", "Settings", out buffer))
				return;

			using(MemoryBuffer mb = new MemoryBuffer(buffer))
			{
				ReadSettings(mb);
			}

			buffer = null;

			if(settings.distance == 0)
				settings.distance = 100;
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class Sound
	{
#if !SERVER
		public static Dictionary<Game.CollectionID, AudioClip[]> soundCache;
		public static AudioSource[] soundSources;
#endif

		public static void Setup()
		{
#if !SERVER
			soundCache = new Dictionary<Game.CollectionID, AudioClip[]>();
#endif
			random = new PseudoRandom();
			CreateSoundSources();
		}

		public static void SetSoundVolume(float volume)
		{
#if !SERVER
			AudioListener.volume = volume;
#endif
		}

		public static float GetSoundVolume()
		{
#if !SERVER
			return AudioListener.volume;
#else
			return 1;
#endif
		}

		public static void CreateSoundSources(int sources = 16)
		{
#if !SERVER
			soundSources = new AudioSource[sources];

			for(int i = 0; i < sources; i++)
			{
				GameObject go = new GameObject("soundSource" + i.ToString("00"));
				soundSources[i] = go.AddComponent<AudioSource>();
				go.transform.parent = Camera.main.transform;
			}
#endif
		}

		public static void Rest()
		{
			StopSounds();
		}

		public static void StopSounds()
		{
#if !SERVER
			for(int i = 0; i < soundSources.Length; i++)
				if(soundSources[i].isPlaying && (soundSources[i].loop || soundSources[i].clip.length > 5))
					soundSources[i].Stop();
#endif
		}

		public static void Play(Game.CollectionID sound)
		{
			Play(sound, 1, 1);
		}

		public static void Play(Game.CollectionID sound, Fixed volume, Fixed pitch)
		{
			if(!Game.settings.soundOn)
				return;

			PlaySound(sound, volume, pitch);
		}

		public static void PlaySound(Game.CollectionID sound, Fixed volume, Fixed pitch)
		{
#if !SERVER
			AudioClip clip = GetSound(sound);

			for(int i = 0; i < soundSources.Length; i++)
			{
				if(!soundSources[i].isPlaying)
				{
					soundSources[i].clip = clip;

					soundSources[i].volume = (float)volume;
					soundSources[i].pitch = (float)pitch;
					soundSources[i].loop = false;

					soundSources[i].Play();
					return;
				}
			}
#endif
		}

		public static void PlayMusic(Game.CollectionID sound, Fixed volume, Fixed pitch, bool loop)
		{
			if(!Game.settings.musicOn)
				return;
#if !SERVER
			AudioClip clip = GetSound(sound);

			for(int i = 0; i < soundSources.Length; i++)
			{
				if(soundSources[i].clip == clip)
				{
					soundSources[i].volume = (float)volume;
					soundSources[i].pitch = (float)pitch;
					soundSources[i].loop = loop;

					if(!soundSources[i].isPlaying)
						soundSources[i].Play();

					return;
				}
			}

			for(int i = 0; i < soundSources.Length; i++)
			{
				if(!soundSources[i].isPlaying)
				{
					soundSources[i].clip = clip;

					soundSources[i].volume = (float)volume;
					soundSources[i].pitch = (float)pitch;
					soundSources[i].loop = loop;

					soundSources[i].Play();
					return;
				}
			}
#endif
		}

		public static void PlayOne(Game.CollectionID sound, Fixed volume, Fixed pitch, bool loop)
		{
			if(!Game.settings.soundOn)
				return;
#if !SERVER
			AudioClip clip = GetSound(sound);

			for(int i = 0; i < soundSources.Length; i++)
			{
				if(soundSources[i].clip == clip)
				{
					soundSources[i].volume = (float)volume;
					soundSources[i].pitch = (float)pitch;
					soundSources[i].loop = loop;

					if(!soundSources[i].isPlaying)
						soundSources[i].Play();

					return;
				}
			}

			for(int i = 0; i < soundSources.Length; i++)
			{
				if(!soundSources[i].isPlaying)
				{
					soundSources[i].clip = clip;

					soundSources[i].volume = (float)volume;
					soundSources[i].pitch = (float)pitch;
					soundSources[i].loop = loop;

					soundSources[i].Play();
					return;
				}
			}
#endif
		}

		public static void StopOne(Game.CollectionID sound)
		{
#if !SERVER
			AudioClip clip = GetSound(sound);

			for(int i = 0; i < soundSources.Length; i++)
			{
				if(soundSources[i].clip == clip)
				{
					if(soundSources[i].isPlaying)
						soundSources[i].Stop();

					return;
				}
			}
#endif
		}

		public static bool IsPlayingOne(Game.CollectionID sound)
		{
#if !SERVER
			AudioClip clip = GetSound(sound);

			for(int i = 0; i < soundSources.Length; i++)
			{
				if(soundSources[i].clip == clip)
					return soundSources[i].isPlaying;
			}
#endif
			return false;
		}

		public static void PlayAtPoint(Game.CollectionID sound, Fixed volume, Vector2 pos, Fixed z = default(Fixed))
		{
			if(!Game.settings.soundOn)
				return;
#if !SERVER
			AudioClip clip = GetSound(sound);
			AudioSource.PlayClipAtPoint(clip, new Vector3((float)pos.x, (float)z, (float)pos.y), (float)volume);
#endif
		}

		public static void PreloadSound(params Game.CollectionID[] sounds)
		{
#if !SERVER
			for(int i = 0; i < sounds.Length; i++)
			{
				startup.StartCoroutine(LoadSoundAsync(sounds[i]));
			}
#endif
		}

#if !SERVER
		static IEnumerator LoadSoundAsync(Game.CollectionID sound)
		{
			ResourceRequest request = Resources.LoadAsync(Game.CollectionIDNames[sound], typeof(AudioClip));

			while(!request.isDone)
				yield return 0;

			List<AudioClip> clips = new List<AudioClip>();
			clips.Add(request.asset as AudioClip);

			soundCache[sound] = clips.ToArray();

			startup.EndCoroutine();
	}
#endif

	public static PseudoRandom random;

#if !SERVER
		public static AudioClip GetSound(Game.CollectionID sound)
		{
			if(!soundCache.ContainsKey(sound))
			{
				//Console.WriteLine("load sound:" + sound);

				List<AudioClip> clips = new List<AudioClip>();

				AudioClip clip = Resources.Load<AudioClip>(Game.CollectionIDNames[sound]);

				if(clip != null)
					clips.Add(clip);
				else
				{
					for(int i = 1; i < 100; i++)
					{
						clip = Resources.Load<AudioClip>(Game.CollectionIDNames[sound] + i.ToString("00"));

						if(clip == null)
							break;

						clips.Add(clip);
					}
				}

				if(clips.Count == 0)
					Console.WriteLine("SOUND NOT FOUND: " + sound);

				soundCache[sound] = clips.ToArray();
			}

			if(soundCache[sound].Length == 1)
				return soundCache[sound][0];
			else
				return soundCache[sound][random.Random(soundCache[sound].Length)];
		}
#endif
	}
}

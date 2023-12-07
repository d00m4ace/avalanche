using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class Model
	{
		public readonly int id;

		public readonly Game.CollectionID modelID;

		public Entity2D entity;

		public void Link(Entity2D entity)
		{
			this.entity = entity;
			entity.model = this;
		}

		public Vector2 pos, dir, localRotDir, worldRotDir;
		public Fixed z, localRotAngle, worldRotAngle, scaleX, scaleY, scaleZ;

		public Vector2 Pos { get { return pos; } }
		public Fixed Z { get { return z; } }
		public Vector2 Dir { get { return dir; } }
		public Vector2 LocalRotDir { get { return localRotDir; } }
		public Vector2 WorldRotDir { get { return worldRotDir; } }
		public Fixed LocalRotAngle { get { return localRotAngle; } }
		public Fixed WorldRotAngle { get { return worldRotAngle; } }
		public Fixed ScaleX { get { return scaleX; } }
		public Fixed ScaleY { get { return scaleY; } }
		public Fixed ScaleZ { get { return scaleZ; } }

		public Game.CollectionID lastAnimation;

		public int flags;

		public class Flags
		{
			public const int None = 0,
			Dirty = 1,
			Disabled = 1 << 1,
			Loading = 1 << 2;
		}

		public void Rest()
		{
			SetScale(1);

			SetPos(Vector2.Zero);
			SetDir(Vector2.XAxis);

			SetLocalRotation(Vector2.XAxis, 0);
			SetWorldRotation(Vector2.XAxis, 0);

#if !SERVER
			RestTransformDataRecursive();
#endif
			Enable(false);
#if !SERVER
			RestGameObjectAnimation();
#endif
		}

#if !SERVER
		public GameObject go;

		public Vector3 position;
		public Quaternion rotation;
		public Vector3 localScale;

		public Animation anim;

		public GameObjectAnimation goa;

		public static Dictionary<Game.CollectionID, GameObject> prefabCache = new Dictionary<Game.CollectionID, GameObject>();

		void Instantiate(GameObject prefab)
		{
			go = (GameObject)UnityEngine.Object.Instantiate(prefab, new Vector3((float)pos.x, (float)z, (float)pos.y), Quaternion.LookRotation(new Vector3((float)dir.x, 0, (float)dir.y)));
			go.name = "ID" + id;

			/*
						Renderer renderer = go.GetComponent<Renderer>();
						if(renderer != null)
						{
							if(renderer.material != null && renderer.material.shader.name == "UNOShader/_Library/UNLIT/UNOShaderUNLIT VertexCol SH ")
							{
								renderer.material.EnableKeyword("_VERTEXCOLOR");
								//Console.WriteLine("_VERTEXCOLOR");
							}
						}
			*/

			go.SetActive(false);

			anim = go.GetComponent<Animation>();
			if(anim != null)
				SaveTransformDataRecursive();

			GameObjectHelper goh = go.AddComponent<GameObjectHelper>();
			goh.model = this;

			flags |= Flags.Dirty | Flags.Disabled;
			flags &= ~Flags.Loading;
		}
#endif

		void Setup(Action<Model> onLoad)
		{
#if !SERVER
			GameObject prefab = null;

			if(prefabCache.ContainsKey(modelID))
				prefab = prefabCache[modelID];

			if(prefab != null)
			{
				Instantiate(prefab);

				if(onLoad != null)
					onLoad(this);
			}
			else
			{
				startup.StartCoroutine(LoadAsync(onLoad));
			}
#else
			flags |= Flags.Dirty | Flags.Disabled;

			if(onLoad != null)
				onLoad(this);
#endif
		}

#if !SERVER
		IEnumerator LoadAsync(Action<Model> onLoad)
		{
			GameObject prefab;

			if(!prefabCache.ContainsKey(modelID))
			{
				prefabCache[modelID] = null;
				ResourceRequest request = Resources.LoadAsync(Game.CollectionIDNames[modelID], typeof(GameObject));
				while(!request.isDone)
					yield return 0;
				prefabCache[modelID] = prefab = request.asset as GameObject;
				//Console.WriteLine(name);
			}
			else
			{
				while(prefabCache[modelID] == null)
					yield return 0;
				prefab = prefabCache[modelID];
			}

			Instantiate(prefab);

			if(onLoad != null)
				onLoad(this);

			startup.EndCoroutine();
	}
#endif

#if !SERVER
		public GameObjectAnimation GetGameObjectAnimation()
		{
			if(goa == null)
			{
				goa = go.AddComponent<GameObjectAnimation>();
				goa.model = this;
			}

			return goa;
		}

		public void RestGameObjectAnimation()
		{
			if(goa != null)
				goa.goAnimation = null;
		}
#endif

		public Model(int id, Game.CollectionID modelID, Action<Model> onLoad)
		{
			this.id = id;
			this.modelID = modelID;

			pos = Vector2.Zero;
			dir = Vector2.XAxis;
			scaleX = scaleY = scaleZ = 1;
#if !SERVER
			flags |= Flags.Loading;
			Setup(onLoad);
#else
			Setup(onLoad);
#endif
		}

#if !SERVER
		public void SetSharedMaterialColor(string nameStartsWith, Color color)
		{
			Color32 c32 = color.GetColor32();

			MeshFilter mf = go.GetComponent<MeshFilter>();

			if(mf != null)
			{
				if(mf.sharedMesh.name.StartsWith(nameStartsWith))
				{
					Renderer r = go.GetComponent<Renderer>();
					r.sharedMaterial.color = c32;
				}
			}

			for(int i = 0; i < go.transform.childCount; i++)
			{
				Transform child = go.transform.GetChild(i);

				mf = child.GetComponent<MeshFilter>();

				if(mf != null)
				{
					if(mf.sharedMesh.name.StartsWith(nameStartsWith))
					{
						Renderer r = child.GetComponent<Renderer>();
						r.sharedMaterial.color = c32;
					}
				}
			}
		}

		public void SetMaterialColor(string nameStartsWith, Color color)
		{
			Color32 c32 = color.GetColor32();

			MeshFilter mf = go.GetComponent<MeshFilter>();

			if(mf != null)
			{
				if(mf.mesh.name.StartsWith(nameStartsWith))
				{
					Renderer r = go.GetComponent<Renderer>();
					r.material.color = c32;
				}
			}

			for(int i = 0; i < go.transform.childCount; i++)
			{
				Transform child = go.transform.GetChild(i);

				mf = child.GetComponent<MeshFilter>();

				if(mf != null)
				{
					if(mf.mesh.name.StartsWith(nameStartsWith))
					{
						Renderer r = child.GetComponent<Renderer>();
						r.material.color = c32;
					}
				}
			}
		}
#endif

#if !SERVER
		public class TransformData
		{
			public Vector3 position, localPosition;
			public Quaternion rotation, localRotation;

			public TransformData(Vector3 position, Vector3 localPosition, Quaternion rotation, Quaternion localRotation)
			{
				this.position = position;
				this.localPosition = localPosition;
				this.rotation = rotation;
				this.localRotation = localRotation;
			}
		}

		public Dictionary<string, TransformData> transformData;

		public void SaveTransformDataRecursive(Transform t)
		{
			transformData[t.name] = new TransformData(t.position, t.localPosition, t.rotation, t.localRotation);

			for(int i = 0; i < t.childCount; i++)
			{
				Transform child = t.GetChild(i);
				SaveTransformDataRecursive(child);
			}
		}

		public void SaveTransformDataRecursive()
		{
			if(transformData == null)
				transformData = new Dictionary<string, TransformData>();

			for(int i = 0; i < go.transform.childCount; i++)
			{
				Transform child = go.transform.GetChild(i);
				SaveTransformDataRecursive(child);
			}
		}

		public void RestTransformDataRecursive(Transform t)
		{
			if(!transformData.ContainsKey(t.name))
				return;

			TransformData td = transformData[t.name];
			t.position = td.position;
			t.localPosition = td.localPosition;
			t.rotation = td.rotation;
			t.localRotation = td.localRotation;

			for(int i = 0; i < t.childCount; i++)
			{
				Transform child = t.GetChild(i);
				RestTransformDataRecursive(child);
			}
		}

		public void RestTransformDataRecursive()
		{
			if(transformData == null)
				return;

			for(int i = 0; i < go.transform.childCount; i++)
			{
				Transform child = go.transform.GetChild(i);
				RestTransformDataRecursive(child);
			}
		}

		public void LogTransformRecursive(Transform t)
		{
			LogTransform(t);

			for(int i = 0; i < t.childCount; i++)
			{
				Transform child = t.GetChild(i);
				LogTransformRecursive(child);
			}
		}

		public void LogTransform(Transform t)
		{
			Console.WriteLine("name: " + t.name);

			Console.WriteLine("position: " + t.position);
			Console.WriteLine("localPosition: " + t.localPosition);

			Console.WriteLine("rotation: " + t.rotation);
			Console.WriteLine("localRotation: " + t.localRotation);

			Console.WriteLine("localScale: " + t.localScale);
			Console.WriteLine("lossyScale: " + t.lossyScale);
		}
#endif

		public void SetPos(Vector2 pos)
		{
			this.pos = pos;
			flags |= Flags.Dirty;
		}

		public void SetZ(Fixed z)
		{
			this.z = z;
			flags |= Flags.Dirty;
		}

		public void SetDir(Vector2 dir)
		{
			this.dir = dir;
			flags |= Flags.Dirty;
		}

		public void SetWorldRotation(Vector2 rotDir, Fixed angle)
		{
			worldRotDir = rotDir;
			worldRotAngle = angle;
			flags |= Flags.Dirty;
		}

		public void SetLocalRotation(Vector2 rotDir, Fixed angle)
		{
			localRotDir = rotDir;
			localRotAngle = angle;
			flags |= Flags.Dirty;
		}

		public void SetScale(Fixed scale)
		{
			scaleX = scaleY = scaleZ = scale;
			flags |= Flags.Dirty;
		}

		public void SetScale(Fixed scaleX, Fixed scaleY, Fixed scaleZ)
		{
			this.scaleX = scaleX;
			this.scaleY = scaleY;
			this.scaleZ = scaleZ;
			flags |= Flags.Dirty;
		}

		public void Update()
		{
#if !SERVER
			if(flags.Has(Flags.Dirty) && go != null)
			{
				Quaternion modelDir = rotation = Quaternion.LookRotation(new Vector3((float)dir.x, 0, (float)dir.y));

				if(localRotAngle != 0)
					rotation *= Quaternion.AngleAxis((float)(180 * localRotAngle * Math.InversePI), new Vector3((float)localRotDir.x, 0, (float)localRotDir.y));

				if(worldRotAngle != 0)
				{
					Vector3 worldDir = Quaternion.Inverse(modelDir) * new Vector3((float)worldRotDir.x, 0, (float)worldRotDir.y);
					rotation *= Quaternion.AngleAxis((float)(180 * worldRotAngle * Math.InversePI), worldDir);
				}

				go.transform.position = position = new Vector3((float)pos.x, (float)z, (float)pos.y);
				go.transform.rotation = rotation;
				go.transform.localScale = localScale = new Vector3((float)scaleX, (float)scaleZ, (float)scaleY);
			}
#endif
			flags &= ~Flags.Dirty;
		}

		public void Enable(bool enable)
		{
#if !SERVER
			if(go == null)
				return;

			go.SetActive(enable);

			if(enable)
				PlayAnimation();
			else
				StopAnimation();

			StopSound();
#endif

			if(enable)
				flags &= ~Flags.Disabled;
			else
				flags |= Flags.Disabled;
		}

#if !SERVER
		AudioSource audioSource;
#endif

		public void PlaySoundOneShot(Game.CollectionID sound, Fixed volume)
		{
#if !SERVER
			if(go == null)
				return;

			if(!Game.settings.soundOn)
				return;

			AudioClip clip = Sound.GetSound(sound);

			if(audioSource == null)
				audioSource = go.AddComponent<AudioSource>();

			audioSource.PlayOneShot(clip, (float)volume);
#endif
		}

		public void PlaySound(Game.CollectionID sound)
		{
			PlaySound(sound, 1, 1, false);
		}

		public void PlaySound(Game.CollectionID sound, Fixed volume, Fixed pitch, bool loop)
		{
#if !SERVER
			if(go == null)
				return;

			if(!Game.settings.soundOn)
				return;

			AudioClip clip = Sound.GetSound(sound);

			if(audioSource == null)
				audioSource = go.AddComponent<AudioSource>();

			audioSource.clip = clip;
			audioSource.volume = (float)volume;
			audioSource.pitch = (float)pitch;
			audioSource.loop = loop;
			audioSource.Play();
#endif
		}

		public void StopSound()
		{
#if !SERVER
			if(go == null)
				return;

			if(audioSource == null)
				return;

			audioSource.Stop();
			audioSource.clip = null;
#endif
		}

		public bool IsPlayingSound()
		{
#if !SERVER
			if(audioSource == null)
				return false;

			return audioSource.isPlaying;
#else
			return false;
#endif
		}

		public void SetSoundVolume(Fixed volume)
		{
#if !SERVER
			if(go == null)
				return;

			if(audioSource == null)
				return;

			audioSource.volume = (float)volume;
#endif
		}

		public void SetSoundDopplerLevel(Fixed dopplerLevel)
		{
#if !SERVER
			if(go == null)
				return;

			if(audioSource == null)
				return;

			audioSource.dopplerLevel = (float)dopplerLevel;
#endif
		}

		public void SetSoundDistance(Fixed minDistance, Fixed maxDistance)
		{
#if !SERVER
			if(go == null)
				return;

			if(audioSource == null)
				return;

			audioSource.minDistance = (float)minDistance;
			audioSource.maxDistance = (float)maxDistance;
#endif
		}

		public void SetSoundPitch(Fixed pitch)
		{
#if !SERVER
			if(go == null)
				return;

			if(audioSource == null)
				return;

			audioSource.pitch = (float)pitch;
#endif
		}

		public void PlayAnimation(Game.CollectionID animation, Fixed speed = default(Fixed))
		{
			lastAnimation = animation;

#if !SERVER
			if(anim != null)
			{
				if(anim[Game.CollectionIDNames[animation]] != null)
				{
					anim.Play(Game.CollectionIDNames[animation]);
					if(speed != 0)
						anim[Game.CollectionIDNames[animation]].speed = (float)speed;
				}
			}
#endif
		}

		public void StopAnimation()
		{
			lastAnimation =  Game.CollectionID.animation_none;

#if !SERVER
			if(anim != null)
				anim.Stop();
#endif
		}

		// start play the default animation also known as the first animation
		public void PlayAnimation()
		{
			lastAnimation = Game.CollectionID.animation_default;

#if !SERVER
			if(anim != null)
				anim.Play();
#endif
		}

		public bool IsPlayingAnimation()
		{
#if !SERVER
			if(anim != null)
				return anim.isPlaying;
#endif
			return false;
		}

		public bool IsPlayingAnimation(Game.CollectionID animation)
		{
#if !SERVER
			if(anim != null)
				return anim.IsPlaying(Game.CollectionIDNames[animation]);
#endif
			return false;
		}

		public void SetAnimationSpeed(Fixed speed)
		{
#if !SERVER
			if(anim != null)
			{
				float fspeed = (float)speed;
				foreach(AnimationState a in anim)
					a.speed = fspeed;
			}
#endif
		}

		public void SetAnimationSpeed(Game.CollectionID animation, Fixed speed)
		{
#if !SERVER
			if(anim != null)
			{
				if(anim[Game.CollectionIDNames[animation]] != null)
					anim[Game.CollectionIDNames[animation]].speed = (float)speed;
			}
#endif
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class Render
	{
		public static Utils.Array<Model> models;

#if !SERVER
		public static Dictionary<string, Texture2D> texturesCache;
		public static Dictionary<Game.CollectionID, GameObject> particlesCache;
		public static Dictionary<string, Font> fontCache;
		public static Dictionary<string, Material> materialCache;

		public static GUISkin guiSkin;

		public static float shadowDistance;
#endif

		public static void Setup()
		{
#if !SERVER
			texturesCache = new Dictionary<string, Texture2D>();
			particlesCache = new Dictionary<Game.CollectionID, GameObject>();
			fontCache = new Dictionary<string, Font>();
			materialCache = new Dictionary<string, Material>();

			shadowDistance = QualitySettings.shadowDistance;
#endif
			models = new Utils.Array<Model>();

			Game.collection.Setup(Game.ObjectType.Particles, (name) =>
			{
				Particles particles = SetupParticles(name);
				return particles;
			});
		}

		public static void StopAllParticles()
		{
			List<Collection.Element> elements = Game.collection.elementsPool[Game.ObjectType.Particles];
			int count = elements.Count;
			for(int i = 0; i < count; i++)
			{
				if(elements[i].elementUsed)
					((Particles)elements[i]).Stop();
			}
		}

		public static void Rest()
		{
			StopAllParticles();
			SetCameraZoom(1.0f);
			ShakeCamera(0);
		}

		public static void SetQualitySettings()
		{
#if !SERVER
			QualitySettings.shadowDistance = Game.settings.shadowsOn ? shadowDistance : 0;
			//QualitySettings.antiAliasing = isShadowsOn ? 2 : 0;
#endif
		}

		public static Model GetModel(int id) { return models[id]; }
		public static Model CreateModel(Game.CollectionID modelID, Action<Model> onLoad = null)
		{
			return models.AddNext(new Model(models.CreateNextID(), modelID, onLoad));
		}

#if !SERVER
		public static Vector3 camVelocity = Vector3.zero;
#endif

		public static float portraitCameraFOV = 60;
		public static float landscapeCameraFOV = 60;

		public static float portraitCameraOrthographicSize = 8;
		public static float landscapeCameraOrthographicSize = 8;

		public static bool portraitCameraOffsetAspectFix = true;

		public static float cameraZoom = 1.0f;

		public static void MoveCameraToPos(Vector2 pos, float yOffset = -10, float cameraHeight = 10, float smoothTime = 0.35f)
		{
#if !SERVER
			if(Camera.main.aspect < 1 && portraitCameraOffsetAspectFix)
				yOffset += yOffset * Camera.main.aspect;

			Vector3 camPos = new Vector3((float)pos.x, 0, (float)pos.y);
			Vector3 camOffs = Camera.main.transform.forward * cameraHeight / Camera.main.transform.forward.y;
			Vector3 yOffs = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * new Vector3(0, 0, yOffset);
			Vector3 target = camPos + camOffs + yOffs;

			Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, target, ref camVelocity, smoothTime);
#endif
		}

		public static void SetCameraPos(Vector2 pos, float yOffset = -10, float cameraHeight = 10)
		{
#if !SERVER
			camVelocity = Vector3.zero;

			if(Camera.main.aspect < 1 && portraitCameraOffsetAspectFix)
				yOffset += yOffset * Camera.main.aspect;

			Vector3 camPos = new Vector3((float)pos.x, 0, (float)pos.y);
			Vector3 camOffs = Camera.main.transform.forward * cameraHeight / Camera.main.transform.forward.y;
			Vector3 yOffs = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * new Vector3(0, 0, yOffset);
			Vector3 target = camPos + camOffs + yOffs;

			Camera.main.transform.position = target;
#endif
		}

		public static void SetCameraZoom()
		{
			SetCameraZoom(cameraZoom);
		}

		public static void SetCameraZoom(float zoom)
		{
			cameraZoom = zoom;
			zoom = 1.0f / zoom;

#if !SERVER
			float currentAspect = (float)Screen.width / Screen.height;

			if(Camera.main.orthographic)
				Camera.main.orthographicSize = currentAspect > 1 ? landscapeCameraOrthographicSize * zoom : portraitCameraOrthographicSize * (1 / currentAspect) * zoom;
			else
				Camera.main.fieldOfView = currentAspect > 1 ? landscapeCameraFOV * zoom : portraitCameraFOV * zoom;
#endif
		}

		public static void SetCameraAngles(float xDegrees, float yDegrees, float zDegrees)
		{
#if !SERVER
			Camera.main.transform.rotation = Quaternion.Euler(new Vector3(xDegrees, yDegrees, zDegrees));
#endif
		}

		public static void ShakeCamera(float shakeDuration, float shakeMagnitude = 1)
		{
#if !SERVER
			CameraHelper ch = Camera.main.GetComponent<CameraHelper>();
			ch.Shake(shakeDuration, shakeMagnitude);
#endif
		}

		public static void LoadGUISkin(string skin)
		{
#if !SERVER
			startup.StartCoroutine(LoadGUISkinAsync(skin));
#endif
		}

#if !SERVER
		static IEnumerator LoadGUISkinAsync(string skin)
		{
			ResourceRequest request = Resources.LoadAsync("gui/" + skin, typeof(GUISkin));
			while(!request.isDone)
				yield return 0;
			guiSkin = request.asset as GUISkin;
			startup.EndCoroutine();
		}
#endif

		public static void PreloadTextures(params string[] textures)
		{
#if !SERVER
			for(int i = 0; i < textures.Length; i++)
			{
				startup.StartCoroutine(LoadTextureAsync(textures[i]));
			}
#endif
		}

#if !SERVER
		static IEnumerator LoadTextureAsync(string texture)
		{
			ResourceRequest request = Resources.LoadAsync("textures/" + texture, typeof(Texture2D));
			while(!request.isDone)
				yield return 0;
			texturesCache[texture] = request.asset as Texture2D;
			startup.EndCoroutine();
	}
#endif

#if !SERVER
		public static Texture2D GetTexture(string texture)
		{
			if(texturesCache.ContainsKey(texture))
				return texturesCache[texture];
			else
			{
				texturesCache[texture] = Resources.Load<Texture2D>(texture);
				texturesCache[texture].filterMode = FilterMode.Point;
				//texturesCache[texture].wrapMode = TextureWrapMode.Repeat;
				return texturesCache[texture];
			}
		}
#endif

#if !SERVER
		public static Material GetMaterial(string material)
		{
			if(materialCache.ContainsKey(material))
				return materialCache[material];
			else
			{
				materialCache[material] = Resources.Load<Material>("materials/" + material);
				return materialCache[material];
			}
		}
#endif

#if !SERVER
		public static Font GetFont(string font)
		{
			if(fontCache.ContainsKey(font))
				return fontCache[font];
			else
			{
				fontCache[font] = Resources.Load<Font>("gui/fonts/" + font);
				fontCache[font].material.mainTexture.filterMode = FilterMode.Point;
				return fontCache[font];
			}
		}
#endif

		public static void PreloadParticles(params Game.CollectionID[] particles)
		{
#if !SERVER
			for(int i = 0; i < particles.Length; i++)
			{
				startup.StartCoroutine(LoadParticleAsync(particles[i]));
			}
#endif
		}

#if !SERVER
		static IEnumerator LoadParticleAsync(Game.CollectionID particlesID)
		{
			ResourceRequest request = Resources.LoadAsync( Game.CollectionIDNames[particlesID], typeof(GameObject));
			while(!request.isDone)
				yield return 0;
			particlesCache[particlesID] = request.asset as GameObject;
			startup.EndCoroutine();
	}
#endif

		public static Particles SetupParticles(Game.CollectionID particlesID)
		{
			Particles p = new Particles();

#if !SERVER
			p.particles = UnityEngine.Object.Instantiate(particlesCache[particlesID]);
			p.restRotation = p.particles.transform.localRotation;
			ParticlesHelper ph = p.particles.AddComponent<ParticlesHelper>();
			ph.particles = p;
#endif

			p.elementID = particlesID;
			p.elementUsed = true;
			return p;
		}
	}
}
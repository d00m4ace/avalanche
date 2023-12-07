using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class Particles : Collection.Element
	{
		public Model model;
		public bool freeOnStop = true;

#if !SERVER
		public GameObject particles;
		public Quaternion restRotation;
#endif

		public void Rest()
		{
#if !SERVER
			particles.transform.position = particles.transform.localPosition = Vector3.zero;
			particles.transform.rotation = particles.transform.localRotation = restRotation;
			particles.transform.localScale = Vector3.one;

			Stop();
			Clear();
#endif
		}

		public void SetTexture(string image)
		{
#if !SERVER
			ParticleSystemRenderer pr = particles.GetComponent<ParticleSystemRenderer>();
			pr.material.mainTexture = Render.GetTexture(image);
#endif
		}

		public void SetPos(Vector2 pos, Fixed z)
		{
#if !SERVER
			particles.transform.localPosition = new Vector3((float)pos.x, (float)z, (float)pos.y);
#endif
		}

		public void SetPosToCamera()
		{
#if !SERVER
			particles.transform.position = Camera.main.transform.position;
#endif
		}

		public void SetDir(Vector2 dir)
		{
#if !SERVER
			particles.transform.localRotation = Quaternion.LookRotation(new Vector3((float)dir.x, 0, (float)dir.y));
#endif
		}

		public void AttachToMainCamera(bool freeOnStop = true)
		{
			this.freeOnStop = freeOnStop;
#if !SERVER
			particles.transform.parent = Camera.main.transform;
#endif
		}

		public void AttachToModel(Model model, bool freeOnStop = true)
		{
			this.freeOnStop = freeOnStop;
			this.model = model;
#if !SERVER
			particles.transform.parent = model.go.transform;
#endif
		}

		public void Detach()
		{
#if !SERVER
			particles.transform.parent = null;
#endif
			model = null;
		}

		public void SetGravityModifier(float gravityModifier)
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();
				ps.gravityModifier = gravityModifier;
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps != null)
					ps.gravityModifier = gravityModifier;
			}
#endif
		}

		public void SetStartSpeed(float startSpeed)
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();
				ps.startSpeed = startSpeed;
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps != null)
					ps.startSpeed = startSpeed;
			}
#endif
		}

		public void SetStartColor(Color color)
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();
				ps.startColor = color.GetColor32();
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps != null)
					ps.startColor = color.GetColor32();
			}
#endif
		}

		public void SetColorGradientOverLifetime(Color[] colors, float[] times)
		{
#if !SERVER
			ParticleSystem.MinMaxGradient grad = new ParticleSystem.MinMaxGradient(CreateGradient(colors, times));

			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();
				var col = ps.colorOverLifetime;
				col.enabled = true;
				col.color = grad;
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps != null)
				{
					var col = ps.colorOverLifetime;
					col.enabled = true;
					col.color = grad;
				}
			}
#endif
		}

#if !SERVER
		public Gradient CreateGradient(Color[] colors, float[] times)
		{
			GradientColorKey[] gck = new GradientColorKey[colors.Length];
			GradientAlphaKey[] gak = new GradientAlphaKey[colors.Length];

			for(int i = 0; i < colors.Length; i++)
			{
				gck[i].color = colors[i].GetColor32();
				gck[i].time = times[i];

				gak[i].alpha = colors[i].a / 255.0f;
				gak[i].time = times[i];
			}

			Gradient g = new Gradient();
			g.SetKeys(gck, gak);
			return g;
		}
#endif

		public void SetPlaybackSpeed(float speed)
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();
				ps.playbackSpeed = speed;
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps != null)
					ps.playbackSpeed = speed;
			}
#endif
		}

		public void Emit(int count)
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();
				ps.Emit(count);
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps != null)
					ps.Emit(count);
			}
#else
			if(freeOnStop)
			{
				Detach();
				elementUsed = false;
			}
#endif
		}

		public void Play()
		{
#if !SERVER
			startTime = Time.time;

			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();

				if(!ps.isPlaying)
					ps.Play();
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(!ps.isPlaying)
					ps.Play();
			}
#else
			if(freeOnStop)
			{
				Detach();
				elementUsed = false;
			}
#endif
		}

		public void Stop()
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();

				if(ps.isPlaying)
					ps.Stop();
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps.isPlaying)
					ps.Stop();
			}
#endif
		}

		public bool IsPlaying()
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();

				if(ps.isPlaying)
					return true;
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps.isPlaying)
					return true;
			}

			return false;
#else
			return false;
#endif
		}

		public float startTime;

		public bool IsAlive()
		{
#if !SERVER
			float time = Time.time - startTime;

			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();

				if(ps.loop || time < ps.duration * 2)
					return true;

				//if(ps.IsAlive()) return true;
			}

			/*
			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();

				if(ps.IsAlive()) return true;
			}
			*/

			return false;
#else
			return false;
#endif
		}

		public void Clear()
		{
#if !SERVER
			{
				ParticleSystem ps = particles.GetComponent<ParticleSystem>();
				ps.Clear();
			}

			for(int i = 0; i < particles.transform.childCount; i++)
			{
				ParticleSystem ps = particles.transform.GetChild(i).GetComponent<ParticleSystem>();
				ps.Clear();
			}
#endif
		}

		public void Free()
		{
			Stop();
			Detach();
			Clear();

			elementUsed = false;
		}
	}
}

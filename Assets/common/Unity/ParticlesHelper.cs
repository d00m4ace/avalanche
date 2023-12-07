using System.Collections;
using System;

using UnityEngine;

namespace HEXPLAY
{
	public class ParticlesHelper : MonoBehaviour
	{
		public Particles particles;

		void Update()
		{
			if(particles.elementUsed && particles.freeOnStop)
			{
				if(!particles.IsAlive())
					particles.Free();
			}
		}
	}
}
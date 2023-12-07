using System.Collections;
using System;

using UnityEngine;

namespace HEXPLAY
{
	public class CameraHelper : MonoBehaviour
	{
		Camera thisCamera;

		void Start()
		{
			thisCamera = GetComponent<Camera>();
		}

		void Update()
		{
			if(shakeElapsed < shakeDuration)
				ShakeUpdate();
		}

		public float shakeDuration = 0.0f;
		public float shakeElapsed = 0.0f;
		public float shakeMagnitude = 1.0f;
		public Vector3 shakePos;

		public void Shake(float shakeDuration, float shakeMagnitude)
		{
			this.shakeDuration = shakeDuration;
			this.shakeMagnitude = shakeMagnitude;
			shakePos = thisCamera.transform.position;
			shakeElapsed = 0;
		}

		void ShakeUpdate()
		{
			shakeElapsed += Time.deltaTime;

			if(shakeElapsed >= shakeDuration)
			{
				shakeElapsed = shakeDuration = 0;
				thisCamera.transform.position = shakePos;
				return;
			}

			float percentComplete = shakeElapsed / shakeDuration;
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = UnityEngine.Random.value * 2.0f - 1.0f;
			float y = UnityEngine.Random.value * 2.0f - 1.0f;

			x *= shakeMagnitude * damper;
			y *= shakeMagnitude * damper;

			thisCamera.transform.position = shakePos + new Vector3(x, 0, y);
		}
	}
}
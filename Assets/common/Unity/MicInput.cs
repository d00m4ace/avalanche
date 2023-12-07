using System.Collections;
using System;

using UnityEngine;

namespace HEXPLAY
{
	public class MicInput : MonoBehaviour
	{
		public static bool isCreated = false;

		public static float MicLoudness;

		private string _device;

		void Start()
		{
			isCreated = true;
		}

		//mic initialization
		void InitMic()
		{
			if(_device == null) _device = Microphone.devices[0];
			_clipRecord = Microphone.Start(_device, true, 999, 44100);
		}

		void StopMicrophone()
		{
			Microphone.End(_device);
		}

		AudioClip _clipRecord = null;
		//AudioClip _clipRecord = new AudioClip();
		int _sampleWindow = 128;

		//get data from microphone into audioclip
		float LevelMax()
		{
			float levelMax = 0;
			float[] waveData = new float[_sampleWindow];
			int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
			if(micPosition < 0) return 0;
			_clipRecord.GetData(waveData, micPosition);
			// Getting a peak on the last 128 samples
			for(int i = 0; i < _sampleWindow; i++)
			{
				float wavePeak = waveData[i] * waveData[i];
				if(levelMax < wavePeak)
				{
					levelMax = wavePeak;
				}
			}
			return levelMax;
		}

		public const float RefValue = 0.1f;

		//get data from microphone into audioclip
		float GetDB()
		{
			float[] waveData = new float[_sampleWindow];
			int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
			if(micPosition < 0) return 0;

			_clipRecord.GetData(waveData, micPosition);

			float sum = 0;

			// Getting a peak on the last 128 samples
			for(int i = 0; i < _sampleWindow; i++)
			{
				sum += waveData[i] * waveData[i];
			}

			float RmsValue = Mathf.Sqrt(sum / _sampleWindow); // rms = square root of average
			float DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
			if(DbValue < -160) DbValue = -160; // clamp it to -160dB min
											   // get sound spectrum

			return DbValue;
		}

		void Update()
		{
			// levelMax equals to the highest normalized value power 2, a small number because < 1
			// pass the value to a static var so we can access it from anywhere
			MicLoudness = LevelMax();
			//MicLoudness = GetDB();
		}

		bool _isInitialized;
		// start mic when scene starts
		void OnEnable()
		{
			InitMic();
			_isInitialized = true;
		}

		//stop mic when loading a new level or quit application
		void OnDisable()
		{
			StopMicrophone();
		}

		void OnDestroy()
		{
			StopMicrophone();
		}

		// make sure the mic gets started & stopped when application gets focused
		void OnApplicationFocus(bool focus)
		{
			if(focus)
			{
				//Debug.Log("Focus");

				if(!_isInitialized)
				{
					//Debug.Log("Init Mic");
					InitMic();
					_isInitialized = true;
				}
			}
			if(!focus)
			{
				//Debug.Log("Pause");
				StopMicrophone();
				//Debug.Log("Stop Mic");
				_isInitialized = false;
			}
		}
	}
}
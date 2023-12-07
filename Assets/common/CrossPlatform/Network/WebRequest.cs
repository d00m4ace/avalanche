using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

#if false
		{
			{
				byte[] bytes = new byte[1024 * 500];
				WebRequest webRequest = new WebRequest();
				webRequest.AddPostField("field1", "1ч1ч1");
				webRequest.AddPostField("field2", 123);
				webRequest.AddPostBinaryData("bytes", bytes);
				webRequest.Post("http://localhost/index.htm");
				...
				if(webRequest != null && webRequest.isSuccessful)
				{
					Console.WriteLine(webRequest.GetDownloadText());
					webRequest.Dispose();
					webRequest = null;
				}
			}
		}
#endif

namespace HEXPLAY
{
	public class WebRequest
	{
		AsyncOperation ao;
		UnityWebRequest www;
		WWWForm wwwForm;

		float startTime;
		float downloadTime;

		public long responseCode = 0;
		public bool isSuccessful = false;
		public bool isCompleted = false;
		public bool isAborted = false;
		public bool autoDispose = false;

		public WebRequest()
		{
		}

		public void Get(string uri)
		{
			startTime = Time.time;
			www = UnityWebRequest.Get(uri);
			startup.monoBehaviour.StartCoroutine(SendRoutine());
		}

		public void Post(string uri)
		{
			startTime = Time.time;
			www = UnityWebRequest.Post(uri, wwwForm);
			startup.monoBehaviour.StartCoroutine(SendRoutine());
		}

		public void AddPostField(string fieldName, string value)
		{
			if(wwwForm == null)
				wwwForm = new WWWForm();

			wwwForm.AddField(fieldName, value);
		}

		public void AddPostField(string fieldName, int value)
		{
			if(wwwForm == null)
				wwwForm = new WWWForm();

			wwwForm.AddField(fieldName, value);
		}

		public void AddPostBinaryData(string fieldName, byte[] contents, string fileName = null, string mimeType = null)
		{
			if(wwwForm == null)
				wwwForm = new WWWForm();

			wwwForm.AddBinaryData(fieldName, contents, fileName, mimeType);
		}

		public void Abort()
		{
			if(isAborted)
				return;

			www.Abort();
			isAborted = true;
		}

		public float GetDownloadTime()
		{
			if(!isSuccessful)
				downloadTime = Time.time - startTime;

			return downloadTime;
		}

		public float GetDownloadProgress()
		{
			return www.downloadedBytes > 0 ? www.downloadProgress : 0;
		}

		public ulong GetDownloadBytes()
		{
			return www.downloadedBytes;
		}

		public string GetDownloadText()
		{
			return www.downloadHandler.text;
		}

		public byte[] GetDownloadData()
		{
			return www.downloadHandler.data;
		}

		public void Dispose()
		{
			if(www != null)
			{
				Console.WriteLine("Web Request Dispose {0}", www.url);
				www.Dispose();
			}

			www = null;
			ao = null;
			wwwForm = null;
		}

		IEnumerator SendRoutine()
		{
			yield return ao = www.Send();

			isCompleted = true;

			if(www.isNetworkError)
			{
				Console.WriteLine("Web Request Error {0} {1}", www.url, www.error);
				isSuccessful = false;
				responseCode = -1;
			}
			else
			{
				Console.WriteLine("Web Request Successful {0}", www.url);
				downloadTime = Time.time - startTime;
				isSuccessful = true;
				responseCode = www.responseCode;
			}

			if(autoDispose)
				Dispose();
		}

		public void Log()
		{
			Console.WriteLine("www isDone:{0} downloadProgress:{1} downloadedBytes:{2} ao.progress:{3} time:{4}", www.isDone, www.downloadProgress, www.downloadedBytes, ao.progress, GetDownloadTime());
		}
	}
}
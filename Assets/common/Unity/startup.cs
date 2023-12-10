using System.Collections;
using System;

using HEXPLAY;

using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

public class startup : MonoBehaviour
{
	public static MonoBehaviour monoBehaviour;
	public static int coroutinesCount;

	public static new Coroutine StartCoroutine(IEnumerator routine) { coroutinesCount++; return monoBehaviour.StartCoroutine(routine); }
	public static void EndCoroutine() { coroutinesCount--; }

	public const int targetFrameRate = 30;

	public static bool isSetupCompleted = false;

	public static string GetSystemInfo()
	{
		return

		"applicationVersion: " + Application.version + "\n" +
		"deviceModel: " + SystemInfo.deviceModel + "\n" +
		"operatingSystem: " + SystemInfo.operatingSystem + "\n" +
		"systemLanguage: " + Application.systemLanguage + "\n" +
		"processorCount: " + SystemInfo.processorCount + "\n" +
		"processorType: " + SystemInfo.processorType + "\n" +
		"systemMemorySize: " + SystemInfo.systemMemorySize + "\n" +
		"graphicsDeviceName: " + SystemInfo.graphicsDeviceName + "\n" +
		"graphicsShaderLevel: " + SystemInfo.graphicsShaderLevel + "\n" +
		"graphicsMemorySize: " + SystemInfo.graphicsMemorySize;
	}

	void Awake()
	{
#if !UNITY_WEBGL
		if(Application.targetFrameRate != targetFrameRate) Application.targetFrameRate = targetFrameRate;
#endif

		monoBehaviour = this;
		coroutinesCount = 0;

		ConsoleRedirector.Redirect();

		Console.WriteLine(GetSystemInfo());

		Game.PreSetup();
	}

	#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern void RateMeNative();
	#endif

	// Use this for initialization
	IEnumerator Start()
	{
		Debug.Log("startup");

		QualitySettings.vSyncCount = 0;

		//while(!RuntimePermissions.isActionPermissionsOnStartRequestCompleted) yield return null;

		Game.Setup();

		Console.WriteLine(Game.settings.gameRunCount + ":" + Game.settings.rateUsCompleted);

		Debug.Log("iOS ver = " + UnityEngine.iOS.Device.systemVersion);

		if(Game.settings.gameRunCount % 10 == 0 && !Game.settings.rateUsCompleted)
		{
			
			#if UNITY_IOS && !UNITY_EDITOR
			string[] ver = UnityEngine.iOS.Device.systemVersion.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries);

			if (ver.Length > 0) 
			{
				Debug.Log("ver = " + ver[ver.Length-1]);
				ver = ver[ver.Length-1].Split(new char[] {'.'},StringSplitOptions.RemoveEmptyEntries);
			}

			if ((ver.Length >= 2) &&  (System.Convert.ToInt32(ver[0]) >= 10) && (System.Convert.ToInt32(ver[1]) >= 3)) 
			{
				Debug.Log("RateMeNative show");
				RateMeNative();
			}
			else
			#endif
			{
				//Platform.RateUsPopUp();
				//while(!Platform.RateUsPopUpClosed) yield return null;
			}
		}

		Game.CreateGame();

		isSetupCompleted = true;

		RenderSettings.ambientMode = AmbientMode.Flat;
		RenderSettings.ambientLight = new UnityEngine.Color(0.5f, 0.5f, 0.5f);
		RenderSettings.ambientIntensity = 0.0f;

		yield return null;
	}

	int lastScreenWidth = 0, lastScreenHeight = 0;
	bool screenSizeChanged = false;

	void LateUpdate()
	{
		if(!isSetupCompleted)
			return;

		//float vFov, hFov = 60.0f;
		//vFov = Mathf.Atan(Mathf.Tan(Mathf.Deg2Rad * hFov / 2) / currentAspect) * 2;
		//float fieldOfView = vFov * Mathf.Rad2Deg;

		screenSizeChanged = false;

		if(lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
		{
			screenSizeChanged = true;

			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;

			Render.SetCameraZoom();
		}

		if(Input.GetKeyDown("f9"))
		{
			ScreenCapture.CaptureScreenshot(Utils.GetDateTimeStamp() + ".png");
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(!isSetupCompleted)
			return;

		Game.gamePDA.OnUpdate();

		HEXPLAY.GUI.UpdateCursorAndFocus();
	}

	void FixedUpdate()
	{
	}

	void OnGUI()
	{
		if(!isSetupCompleted)
			return;

		HEXPLAY.GUI.OnGUI(screenSizeChanged);
	}

	void OnDrawGizmos()
	{
	}

	public static bool hasUnpaused = false;

	void OnApplicationPause(bool pauseStatus)
	{
		if(!isSetupCompleted)
			return;

		if(!pauseStatus)
			hasUnpaused = true;

		Debug.Log("OnApplicationPause:" + pauseStatus);
	}

	static StringBuilder logBuffer = new StringBuilder();

	void HandleLog(string logString, string stackTrace, LogType type)
	{
		lock(logBuffer)
		{
			logBuffer.AppendLine(logString + "\n\t\t" + type + ": " + stackTrace.Replace("\n", "\n\t\t"));
		}
	}

	public static string GetLog()
	{
		lock(logBuffer)
		{
			return logBuffer.ToString();
		}
	}

	void OnEnable()
	{
		Application.logMessageReceivedThreaded += HandleLog;
	}
}
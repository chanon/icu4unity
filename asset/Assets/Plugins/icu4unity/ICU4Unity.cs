using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Runtime.InteropServices;
using System.Text;

#if UNITY_IOS
	using AOT;
#endif

public class ICU4Unity {

	// setup libraryName

#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
	private const string libraryName = "__Internal";
#else
	private const string libraryName = "icu4unity";
#endif

	// handling for debug printing

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void DebugDelegate(string str);

#if UNITY_IOS
	[MonoPInvokeCallback(typeof(DebugDelegate))]
#endif
	private static void CallBackFunction(string str) { Debug.Log(str); }

	[DllImport(libraryName)]
	private static extern void ICU4USetDebugFunction(IntPtr fp);

	// icu4unity C interface

	[DllImport(libraryName)]
	private static extern void ICU4USetICUDataPath(string path);

	[DllImport(libraryName)]
	private static extern bool ICU4USetICUData(byte[] bytes);

	[DllImport(libraryName)]
	private static extern bool ICU4UIsDataLoaded();

	[DllImport(libraryName)]
	private static extern bool ICU4USetLocale(string locale);

	[DllImport(libraryName)]
	private static extern void ICU4UInsertLineBreaks([In, Out] StringBuilder chars, int breakCharacter);

	// singleton / constructor

	private static ICU4Unity _instance = null;
	public static ICU4Unity instance {
		get {
			if (_instance == null) {
				_instance = new ICU4Unity();
			}
			return _instance;
		}
	}

	private ICU4Unity() {
		// setup debug 
		#if ENABLE_ILCPP
			ICU4USetDebugFunction(CallBackFunction);
		#else
			DebugDelegate callbackDelegate = new DebugDelegate(CallBackFunction);
			IntPtr intptrDelegate = Marshal.GetFunctionPointerForDelegate(callbackDelegate);
			ICU4USetDebugFunction(intptrDelegate);
		#endif
	}

	// public interface

	private byte[] _icudata;
	private bool _icuDataLoadStarted = false;
	private bool _icuDataLoaded = false;
	private string _savedLocale = "";
	private string _activeLocale = "";

	public bool icuDataIsLoaded {
		get { return _icuDataLoaded; }
	}

	public IEnumerator InitData() {
#if UNITY_ANDROID && !UNITY_EDITOR
		yield return LoadICUData();
#else
		SetICUDataDirectory();
		yield break;
#endif
	}

	private void SetICUDataDirectory() {
		if (ICU4UIsDataLoaded()) {
			_icuDataLoaded = true;
			return;
		}

		string loadPath = Application.streamingAssetsPath + "/icudt63l.dat";
		ICU4USetICUDataPath(loadPath);
		_icuDataLoaded = true;
	}

	private IEnumerator LoadICUData() {
		if (ICU4UIsDataLoaded()) {
			_icuDataLoaded = true;
			yield break;
		}

		if (_icuDataLoadStarted) {
			yield break;
		}

		_icuDataLoadStarted = true;

		string loadPath = "icudt63l.dat";
		string loadPrefix = "file://";

#if (UNITY_ANDROID)
		loadPrefix = "";
#endif

		//Debug.Log("streamingAssetsPath is: " + Application.streamingAssetsPath);
		//Debug.Log("dataPath is: " + Application.dataPath);

		loadPath = loadPrefix + Application.streamingAssetsPath + "/" + loadPath;


		// load ICU4C data
		Debug.Log("ICU4C: Loading ICU4C data from: " + loadPath);
		UnityWebRequest www = UnityWebRequest.Get(loadPath);
		yield return www.SendWebRequest();
		if (www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}
		else {
			_icudata = www.downloadHandler.data;
			Debug.Log("ICU4U: Data loaded successfully");
			ICU4USetICUData(_icudata);
			Debug.Log("ICU4U: Data set successfully");
			_icuDataLoaded = true;
		}
	}

	public void SetLocale(string locale) {
		if (_activeLocale != locale) {
			_savedLocale = locale;

			if (!_icuDataLoaded) {
				return;
			}

			ICU4USetLocale(locale);
			_activeLocale = locale;
		}
	}

	private StringBuilder _builder = new StringBuilder(1023);

	// max text length is 1023
	public string InsertLineBreaks(string text, Char separator = '\u200B') {
		// make sure locale is set
		SetLocale(_savedLocale);

		if (_activeLocale != _savedLocale) {
			return text;
		}

		if (_activeLocale == "") {
			return text;
		}

		if (!_icuDataLoaded) {
			return text;
		}

		if (text.Length > 1022) {
			Debug.LogError("ICU4U: text is too long to break");
			return text;
		}

		//Debug.Log("we are actually doing it...");

		_builder.Clear();
		_builder.Append(text);

		// send to have line breaks inserted
		ICU4UInsertLineBreaks(_builder, separator);

		// return new string
		return _builder.ToString();
	}
}
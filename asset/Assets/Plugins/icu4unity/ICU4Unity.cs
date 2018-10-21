using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Runtime.InteropServices;
using System.Text;

#if ENABLE_IL2CPP
	using AOT;
#endif

public class ICU4Unity {

	///////////////////////////////////////////////////////////////////////////
	// C++ <-> C# Interface

	// setup libraryName

#if (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
	private const string libraryName = "__Internal";
#else
	private const string libraryName = "icu4unity";
#endif

	// handling for debug printing

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void DebugDelegate(string str);

#if ENABLE_IL2CPP
	[MonoPInvokeCallback(typeof(DebugDelegate))]
#endif
	private static void DebugCallBackFunction(string str) { Debug.Log(str); }

	[DllImport(libraryName)]
	private static extern void ICU4USetDebugFunction(IntPtr fp);

	// callback to return string for insert line break result

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void ReturnStringDelegate(string str, int requestNo);

#if ENABLE_IL2CPP
	[MonoPInvokeCallback(typeof(ReturnStringDelegate))]
#endif
	private static void ReturnStringCallBackFunction(string str, int requestNo) { _results[requestNo] = str; }

	[DllImport(libraryName)]
	private static extern void ICU4USetReturnStringFunction(IntPtr fp);

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
	private static extern void ICU4UInsertLineBreaks(string chars, int reqNo, int breakCharacter);

	///////////////////////////////////////////////////////////////////////////
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
		ICU4USetDebugFunction(DebugCallBackFunction);
		ICU4USetReturnStringFunction(ReturnStringCallBackFunction);
#else
		DebugDelegate callbackDelegate = new DebugDelegate(DebugCallBackFunction);
		IntPtr intptrDelegate = Marshal.GetFunctionPointerForDelegate(callbackDelegate);
		ICU4USetDebugFunction(intptrDelegate);

		ReturnStringDelegate callbackDelegate2 = new ReturnStringDelegate(ReturnStringCallBackFunction);
		IntPtr intptrDelegate2 = Marshal.GetFunctionPointerForDelegate(callbackDelegate2);
		ICU4USetReturnStringFunction(intptrDelegate2);
#endif

		// load data
		_InitData();

		// default locale to en
		SetLocale("en");
	}

	private void _InitData() {
#if UNITY_ANDROID && !UNITY_EDITOR
		_LoadICUData();
#else
		_SetICUDataDirectory();
#endif
	}

	private void _SetICUDataDirectory() {
		if (ICU4UIsDataLoaded()) {
			_icuDataLoaded = true;
			return;
		}

		string loadPath = Application.streamingAssetsPath + "/icudt63l.dat";
		ICU4USetICUDataPath(loadPath);
		_icuDataLoaded = true;
	}

	UnityWebRequest _www;

	private void _LoadICUData() {
		if (ICU4UIsDataLoaded()) {
			_icuDataLoaded = true;
			return;
		}

		if (_icuDataLoadStarted) {
			return;
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
		_www = UnityWebRequest.Get(loadPath);
		UnityWebRequestAsyncOperation result = _www.SendWebRequest();
		result.completed += _OnLoadComplete;
	}

	private void _OnLoadComplete(AsyncOperation op) {
		if (_www.isNetworkError || _www.isHttpError) {
			Debug.Log(_www.error);
		}
		else {
			_icudata = _www.downloadHandler.data;
			Debug.Log("ICU4U: Data loaded successfully");
			ICU4USetICUData(_icudata);
			Debug.Log("ICU4U: Data set successfully");
			_icuDataLoaded = true;
		}
	}

	///////////////////////////////////////////////////////////////////////////
	// public interface implementation

	private byte[] _icudata;
	private bool _icuDataLoadStarted = false;
	private bool _icuDataLoaded = false;
	private string _savedLocale = "";
	private string _activeLocale = "";
	private static Dictionary<int, string> _results = new Dictionary<int, string>();
	private int _nextRequestNo = 1;

	public bool icuDataIsLoaded {
		get {
#if UNITY_EDITOR
			if (ICU4UIsDataLoaded()) {
				_icuDataLoaded = true;
			}
#endif
			return _icuDataLoaded;
		}
	}

	/// <summary>
	/// Sets a locale to use. Defaults to "en". Correct locale not actually needed for it to work.
	/// </summary>
	/// <param locale="text">The locale, such as "en-US" or "th" to set.</param>
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

	/// <summary>
	/// Inserts Line Break characters into text with an optional separator for proper word wrapping of Asian text.
	/// </summary>
	/// <param name="text">The text to insert line breaks to.</param>
	/// <param separator="text">The separator character to insert. Defaults to the 'Zero Width Space' unicode character.</param>
	/// <returns>The text with separator character inserted at line break positions.</returns>
	/// <remarks>
	/// TextMeshPro supports the 'Zero Width Space' for line breaking / word wrapping.
	/// However as of this writing, Unity UI Text does not.
	/// </remarks>
	public string InsertLineBreaks(string text, Char separator = '\u200B') {
		// make sure locale is set
		//Debug.Log("SetLocale");
		SetLocale(_savedLocale);

		if (_activeLocale != _savedLocale) {
			//Debug.Log("not locale");
			return text;
		}

		if (_activeLocale == "") {
			//Debug.Log("not active locale");
			return text;
		}

		if (!_icuDataLoaded) {
			//Debug.Log("not loaded");
			return text;
		}

		//Debug.Log("sending request to C++ side...");

		// send to have line breaks inserted
		int reqNo = _nextRequestNo++;
		ICU4UInsertLineBreaks(text, reqNo, separator);
		// get the result
		if (_results.ContainsKey(reqNo)) {
			string result = _results[reqNo];
			_results.Remove(reqNo);
			//Debug.Log("C# side got result: " + result);
			//Debug.Log("number of keys existing are: " + _results.Keys.Count);
			return result;
		}
		else {
			//Debug.Log("C# side did not get any result!");
			return text;
		}
	}
}
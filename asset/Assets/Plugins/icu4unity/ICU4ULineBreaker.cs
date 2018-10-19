using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Add this to any GameObject with a TextMeshPro Text component
/// It will automatically insert line breaks
/// It works by checking the TextMeshPro Text for text changes every frame
[ExecuteInEditMode]
public class ICU4ULineBreaker : MonoBehaviour {
	private TMPro.TextMeshProUGUI _tmpText = null;
	private string _prevValue;

	void Start() {
		_tmpText = GetComponent<TMPro.TextMeshProUGUI>();
	}

	void Update() {
		if (_tmpText != null) {
			if (_tmpText.text != _prevValue) {
				_tmpText.text = ICU4Unity.instance.InsertLineBreaks(_tmpText.text);
				_prevValue = _tmpText.text;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// An alternative to ICU4ULineBreaker
/// Add this to any GameObject with a TextMeshPro Text component
/// Then, to set text you must set the text field on the ICU4UText component
/// NOT on the TextMeshPro Text component
public class ICU4UText : MonoBehaviour {

	private TMPro.TextMeshProUGUI _tmpText = null;

	void Start() {
		_tmpText = GetComponent<TMPro.TextMeshProUGUI>();

		// trigger update
		this.Text = text;
	}

	[SerializeField, ICU4USetPropertyAttribute("Text")]
	private string text;

	public string Text {
		get {
			return text;
		}
		set {
			text = value;
			string result = ICU4Unity.instance.InsertLineBreaks(text);
#if UNITY_EDITOR
			if (_tmpText == null) {
				// try finding it
				_tmpText = GetComponent<TMPro.TextMeshProUGUI>();
			}
#endif
			if (_tmpText != null) {
				_tmpText.text = result;
			}
		}
	}
}
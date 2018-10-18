using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ICU4UnityWordBreaker : MonoBehaviour {
	private UnityEngine.UI.Text text;

	void Start() {
		text = GetComponent<UnityEngine.UI.Text>();
		StartCoroutine(ICU4Unity.instance.InitData());
		ICU4Unity.instance.SetLocale("th");
	}

	void Update() {
		text.text = ICU4Unity.instance.InsertLineBreaks("ทดสอบภาษาไทยนะครับ มัยตัดถูกไหม and english also-ok", '|');
		text.text = ICU4Unity.instance.InsertLineBreaks(text.text);
	}
}

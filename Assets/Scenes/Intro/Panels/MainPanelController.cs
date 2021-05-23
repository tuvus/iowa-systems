using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainPanelController : MonoBehaviour {
	public void DisplayPanel(bool _trueOrFalse = true) {
		GetComponent<Image>().enabled = _trueOrFalse;
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(_trueOrFalse);
		}
	}
}

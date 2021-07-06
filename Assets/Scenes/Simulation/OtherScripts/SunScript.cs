using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour {

	public float sunSpeed;

	public void SetupSun() {
		User.Instance.changedSettings += OnSettingsChanged;
	}

	public void OnSettingsChanged(User _user, SettingsEventArgs _settings) {
		GetComponentInChildren<LensFlare>().enabled = _settings.sun;
		GetComponentInChildren<Light>().enabled = _settings.shadows;
		if (_settings.shadows == false) {
			RenderSettings.ambientLight = new Color(20, 20, 20, 0);
        } else {
			RenderSettings.ambientLight = new Color(6, 6, 6, 0);
		}
	}

	void FixedUpdate () {
		transform.Rotate(0, sunSpeed * Time.fixedDeltaTime, 0);
		transform.position = new Vector3(0, 0, 0);
		transform.Translate(transform.forward * -1000, Space.World);
	}
}

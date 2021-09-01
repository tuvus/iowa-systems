using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour {
	EarthScript earth;
	public float sunSpeed;

	public void SetupSun(EarthScript earth) {
		User.Instance.ChangedSettings += OnSettingsChanged;
		this.earth = earth;
	}

	public void OnSettingsChanged(User _user, SettingsEventArgs _settings) {
		GetComponentInChildren<LensFlare>().enabled = _settings.Sun;
		GetComponentInChildren<Light>().enabled = _settings.Shadows;
		if (_settings.Shadows == false) {
			RenderSettings.ambientLight = new Color(20, 20, 20, 0);
        } else {
			RenderSettings.ambientLight = new Color(6, 6, 6, 0);
		}
	}

	public void UpdateSun () {
		transform.Rotate(0, sunSpeed * earth.simulationDeltaTime, 0);
		transform.position = new Vector3(0, 0, 0);
		transform.Translate(transform.forward * -1000, Space.World);
	}
}

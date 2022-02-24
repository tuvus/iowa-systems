using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour {
	EarthScript earth;
	public float sunSpeed;
	LensFlare lensFlare;
	Light sunLight;

	public void SetupSun(EarthScript earth) {
		this.earth = earth;
		lensFlare = GetComponentInChildren<LensFlare>();
		sunLight = GetComponentInChildren<Light>();
	}

	public void OnSettingsChanged(bool renderSun, bool renderShadows) {
		lensFlare.enabled = renderSun;
		sunLight.enabled = renderShadows;
		if (renderShadows) {
			RenderSettings.ambientLight = new Color(6, 6, 6, 0);
        } else {
			RenderSettings.ambientLight = new Color(20, 20, 20, 0);
		}
	}

	public void UpdateSun () {
		transform.Rotate(0, sunSpeed * earth.simulationDeltaTime, 0);
		transform.position = new Vector3(0, 0, 0);
		transform.Translate(transform.forward * -1000, Space.World);
	}
}

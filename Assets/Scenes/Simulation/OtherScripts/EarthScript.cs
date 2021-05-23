using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthScript : MonoBehaviour {
	public Transform sunTransform;

	public int size;
	public float worldTime;
	public int maxTime;
	public float time;
	public float tempeture;
	public float humidity;
	public float humidityTarget;

	public void SetUpEarth(int _size) {
		size = _size;
		transform.localScale = new Vector3(size, size, size);
		User.Instance.changedSettings += OnSettingsChanged;
		SetHumidityTarget();
	}

	void FixedUpdate() {
		worldTime += Time.fixedDeltaTime;
		time -= Time.fixedDeltaTime;
		if (time <= 0) {
			time = maxTime;
		}

		UpdateHumidity();
	}


	public void UpdateHumidity() {
		if (humidityTarget > humidity) {
			humidity += Random.Range(-0.25f * Time.fixedDeltaTime, 1f * Time.fixedDeltaTime);
			if (humidityTarget <= humidity) {
				SetHumidityTarget();
            }
			return;
        }
		if (humidityTarget < humidity) {
			humidity += Random.Range(-1f * Time.fixedDeltaTime, 0.25f * Time.fixedDeltaTime);
			if (humidityTarget >= humidity) {
				SetHumidityTarget();
			}
			return;
		}
	}

	public void SetHumidityTarget() {
		humidityTarget = Random.Range(0, 100f);
    }

	public float FindSunValue(Vector3 _position) {
		if (SimulationScript.Instance.sunRotationEffect) {
			float objectDistanceFromSun = Vector3.Distance(_position, sunTransform.position);
			float sunDistanceFromEarth = Vector3.Distance(transform.position, sunTransform.position);
			float sunValue = (objectDistanceFromSun - sunDistanceFromEarth) / size * 2;
			if (sunValue < 0)
				return 0;
			return sunValue;
		} else {
			return 0.5f;
        }
	}

	public void OnSettingsChanged(User _user, SettingsEventArgs _settings) {
		GetComponent<MeshRenderer>().enabled = _settings.rendering;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) {
			renderer.enabled = _settings.rendering;
        }
		if (_settings.rendering) {
			if (QualitySettings.GetQualityLevel() <= 1) {
				GetAtmosphereRenderer().enabled = false;
			} else {
				GetAtmosphereRenderer().enabled = true;
			}
		}
    }

	public Transform GetOrganismsTransform () {
		return transform.GetChild(1);
    }



	MeshRenderer GetAtmosphereRenderer () {
		return transform.GetChild(0).GetComponent<MeshRenderer>();
    }
}

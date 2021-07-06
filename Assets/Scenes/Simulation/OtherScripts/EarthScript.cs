using System;
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

    List<BasicOrganismScript> organisms = new List<BasicOrganismScript>();
	List<BasicFoodScript> foods = new List<BasicFoodScript>();

	bool shouldNotChangeOrganismsList;

	public event EventHandler<OnOrganismArgs> OnCreateNewOrganism;
	public event EventHandler<OnOrganismArgs> OnDestroyNewOrganism;

	public class OnOrganismArgs : EventArgs {
		public BasicOrganismScript organism;
		public BasicSpeciesScript species;
	}

	public event EventHandler<OnFoodArgs> OnCreateNewFood;
	public event EventHandler<OnFoodArgs> OnDestroyNewFood;

	public class OnFoodArgs : EventArgs {
		public BasicFoodScript foodScript;
	}

	public event EventHandler<EventArgs> OnEndFrame;

	public void SetUpEarth(int _size) {
		size = _size;
		transform.localScale = new Vector3(size, size, size);
		User.Instance.changedSettings += OnSettingsChanged;
	}

    #region Runtime

    void FixedUpdate() {
		UpdateWorldTime();
		UpdateHumidity();

		shouldNotChangeOrganismsList = true;
		RefreshOrganisms();
		UpdateOrganisms();
		UpdateFoods();
		shouldNotChangeOrganismsList = false;
		OnEndFrame?.Invoke(this, new EventArgs { });
	}

	void UpdateWorldTime() {
		worldTime += Time.fixedDeltaTime;
		time -= Time.fixedDeltaTime;
		if (time <= 0) {
			time = maxTime;
		}
	}

	void UpdateHumidity() {
		if (humidityTarget > humidity) {
			humidity += UnityEngine.Random.Range(-0.5f * Time.fixedDeltaTime, 2f * Time.fixedDeltaTime);
			if (humidityTarget <= humidity) {
				SetHumidityTarget();
            }
			return;
        }
		if (humidityTarget < humidity) {
			humidity += UnityEngine.Random.Range(-2f * Time.fixedDeltaTime, 0.5f * Time.fixedDeltaTime);
			if (humidityTarget >= humidity) {
				SetHumidityTarget();
			}
			return;
		}
	}
    
	public void SetHumidityTarget() {
		humidityTarget = UnityEngine.Random.Range(0, 100f);
    }

	void RefreshOrganisms() {
        foreach (var organism in organisms) {
			organism.RefreshOrganism();
        }
    }

	void UpdateOrganisms() {
        foreach (var organism in organisms) {
			organism.UpdateOrganism();
        }
    }

	void UpdateFoods() {
        foreach (var food in foods) {
			food.UpdateFood();
        }
    }

    #endregion

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

    #region ObjectControlls
    public void AddObject(BasicOrganismScript newOrganism, BasicSpeciesScript species) {
		if (shouldNotChangeOrganismsList)
			Debug.LogWarning("Changing organism list while in a loop:" + newOrganism);
		organisms.Add(newOrganism);
		OnCreateNewOrganism?.Invoke(this, new OnOrganismArgs { organism = newOrganism, species = species});
    }

	public void AddObject(BasicFoodScript foodScript) {
		if (shouldNotChangeOrganismsList)
			Debug.LogWarning("Changing organism list while in a loop:" + foodScript);
		foods.Add(foodScript);
		OnCreateNewFood?.Invoke(this, new OnFoodArgs { foodScript = foodScript });
	}

	public void RemoveObject(BasicOrganismScript removeOrganism, BasicSpeciesScript species) {
		if (shouldNotChangeOrganismsList)
			Debug.LogWarning("Changing organism list while in a loop:" + removeOrganism);
		organisms.Remove(removeOrganism);
        OnDestroyNewOrganism?.Invoke(this, new OnOrganismArgs { organism = removeOrganism, species = species});
    }

	public void RemoveObject(BasicFoodScript foodScript) {
		if (shouldNotChangeOrganismsList)
			Debug.LogWarning("Changing organism list while in a loop:" + foodScript);
		foods.Remove(foodScript);
		OnDestroyNewFood?.Invoke(this, new OnFoodArgs { foodScript = foodScript });
	}
	#endregion

	#region GetMethods
	public float GetSunValue(Vector3 _position) {
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

    public Transform GetOrganismsTransform () {
		return transform.GetChild(1);
    }

	public List<BasicOrganismScript> GetAllOrganisms() {
		return organisms;
    }

	MeshRenderer GetAtmosphereRenderer () {
		return transform.GetChild(0).GetComponent<MeshRenderer>();
    }
    #endregion
}

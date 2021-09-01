using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class EarthScript : MonoBehaviour {
	enum SimulationUpdateStatus {
		Intializing = 0,
		SettingUp = 1,
		Calculating = 2,
		Updating = 3,
		CleaningUp = 4,
    }

	public Transform sunTransform;
	FrameManager frameManager;

    #region EarthVariables
    public int size;
	public float worldTime;
	public int maxTime;
	public float simulationDeltaTime;
	public float tempeture;
	public float humidity;
	public float humidityTarget;
    readonly List<BasicOrganismScript> organisms = new List<BasicOrganismScript>();
	List<BasicFoodScript> foods = new List<BasicFoodScript>();

    #endregion

    #region Events
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
    #endregion

    List<JobHandle> activeJobs = new List<JobHandle>();
	SimulationUpdateStatus simulationUpdateStatus = SimulationUpdateStatus.Intializing;
	bool shouldNotChangeOrganismsList;

	public void SetUpEarth(int size, float simulationSpeed) {
		this.simulationDeltaTime = simulationSpeed / 10;
		this.size = size;
		transform.localScale = new Vector3(size, size, size);
		User.Instance.ChangedSettings += OnSettingsChanged;
		frameManager = GetComponent<FrameManager>();
		frameManager.SetWantedItterationsPerFrame(User.Instance.GetFramesPerSeccondUserPref());
	}

	public void StartSimulation() {
		simulationUpdateStatus = SimulationUpdateStatus.Intializing;
    }

    #region Runtime
    private void Update() {
		if (!SimulationScript.Instance.simulationInitialised)
			return;
		frameManager.UpdateFrameStartTime();
		for (int i = 0; i < frameManager.GetItterationsPerFrame(); i++) {
			if (i == 0 || frameManager.CanStartNewItterationBeforeNextFrame()) {
				frameManager.LogSimulationItterationStart();
				UpdateSimulationWithoutDelay();
				frameManager.LogSimulationItterationEnd();
			} else {
				frameManager.SetItterationsThatOccuredThisFrame(i);
				return;
            }
		}
	}

	void UpdateSimualtion() {
		if (simulationUpdateStatus == SimulationUpdateStatus.SettingUp) {
			UpdateWorldTime();
			UpdateHumidity();
			SimulationScript.Instance.GetSun().UpdateSun();
			shouldNotChangeOrganismsList = true;
			StartOrganismJobs();
			simulationUpdateStatus = SimulationUpdateStatus.Calculating;
			return;
        }
		if (simulationUpdateStatus == SimulationUpdateStatus.Calculating) {
			UpdateJobList();
			if (activeJobs.Count == 0) {
				simulationUpdateStatus = SimulationUpdateStatus.Updating;
			}
		}
		if (simulationUpdateStatus == SimulationUpdateStatus.Updating) {
			UpdateOrganismsBehavior();
			UpdateOrganisms();
			UpdateFoods();
			simulationUpdateStatus = SimulationUpdateStatus.CleaningUp;
        }
		if (simulationUpdateStatus == SimulationUpdateStatus.CleaningUp) {
			shouldNotChangeOrganismsList = false;
			OnEndFrame?.Invoke(this, new EventArgs { });
			simulationUpdateStatus = SimulationUpdateStatus.SettingUp;
        }
	}

	void UpdateSimulationWithoutDelay() {
		UpdateWorldTime();
		UpdateHumidity();
		shouldNotChangeOrganismsList = true;
        StartOrganismJobs();
        simulationUpdateStatus = SimulationUpdateStatus.Calculating;
        CompleteJobs();
        simulationUpdateStatus = SimulationUpdateStatus.Updating;
        UpdateOrganismsBehavior();
        UpdateOrganisms();
        UpdateFoods();
        simulationUpdateStatus = SimulationUpdateStatus.CleaningUp;
        shouldNotChangeOrganismsList = false;
        OnEndFrame?.Invoke(this, new EventArgs { });
		UpdateSpeciesMotorGraphData();
        simulationUpdateStatus = SimulationUpdateStatus.SettingUp;
    }

	void UpdateWorldTime() {
		worldTime += simulationDeltaTime;
	}

	void UpdateHumidity() {
		if (humidityTarget > humidity) {
			humidity += UnityEngine.Random.Range(-0.5f * simulationDeltaTime, 2f * simulationDeltaTime);
			if (humidityTarget <= humidity) {
				SetHumidityTarget();
            }
			return;
        }
		if (humidityTarget < humidity) {
			humidity += UnityEngine.Random.Range(-2f * simulationDeltaTime, 0.5f * simulationDeltaTime);
			if (humidityTarget >= humidity) {
				SetHumidityTarget();
			}
			return;
		}
	}
    
	public void SetHumidityTarget() {
		humidityTarget = UnityEngine.Random.Range(0, 100f);
    }

	void StartOrganismJobs() {
		List<BasicAnimalSpecies> allSpecies = SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies();
        for (int i = 0; i < allSpecies.Count; i++) {
		  	JobHandle newJob = allSpecies[i].GetBasicJobController().StartJob();
			activeJobs.Add(newJob);
        }
    }

	void UpdateJobList() {
		for (int i = 0; i < activeJobs.Count; i++) {
			if (activeJobs[i].IsCompleted) {
				activeJobs[i].Complete();
                activeJobs.RemoveAt(i);
				i--;
			}
		}
	}

	void CompleteJobs() {
		for (int i = 0; i < activeJobs.Count; i++) {
			activeJobs[i].Complete();
			activeJobs.RemoveAt(i);
			i--;
		}
	}

	void UpdateOrganismsBehavior() {
		List<BasicSpeciesScript> allSpecies = GetAllSpecies();
		for (int i = 0; i < allSpecies.Count; i++) {
			allSpecies[i].UpdateOrganismsBehavior();
        }
    }

	void UpdateOrganisms() {
		List<BasicSpeciesScript> allSpecies = GetAllSpecies();
		for (int i = 0; i < allSpecies.Count; i++) {
			allSpecies[i].UpdateOrganisms();
		}
	}

	void UpdateFoods() {
        foreach (var food in foods) {
			food.UpdateFood();
        }
    }

	void UpdateSpeciesMotorGraphData() {
		SpeciesManager.Instance.GetSpeciesMotor().UpdateSpeciesGraphData();
    }

    #endregion

	public void OnSettingsChanged(User _user, SettingsEventArgs _settings) {
		GetComponent<MeshRenderer>().enabled = _settings.Rendering;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) {
			renderer.enabled = _settings.Rendering;
        }
		if (_settings.Rendering) {
			if (QualitySettings.GetQualityLevel() <= 1) {
				GetAtmosphereRenderer().enabled = false;
			} else {
				GetAtmosphereRenderer().enabled = true;
			}
		}
		frameManager.SetWantedItterationsPerFrame(_settings.FramesPerSeccond);
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
	public List<BasicSpeciesScript> GetAllSpecies() {
		return SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies();
    }


	public List<BasicJobController> GetAllJobControllers() {
		List<BasicJobController> jobControllers = new List<BasicJobController>();
        foreach (var animalSpecies in SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies()) {
			jobControllers.Add(animalSpecies.GetBasicJobController());
        }
		return jobControllers;
	}
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

	public FrameManager GetFrameManager() {
		return frameManager;
    }
    #endregion
}

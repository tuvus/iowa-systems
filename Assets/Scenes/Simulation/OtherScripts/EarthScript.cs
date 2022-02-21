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
	ZoneController zoneController;

    #region EarthVariables
    public int size;
	public float worldTime;
	public int maxTime;
	public float simulationDeltaTime;
	public float tempeture;
	public float humidity;
	public float humidityTarget;
	public EarthState earthState { private set; get; }
    #endregion

	public event EventHandler<EventArgs> OnEndFrame;

    List<JobHandle> activeJobs = new List<JobHandle>();
	SimulationUpdateStatus simulationUpdateStatus = SimulationUpdateStatus.Intializing;
	List<string> typeIndex;

	public struct EarthState {
		public Vector3 sunPostion;
		public float earthRadius;
		public float humidity;
		public float temperature;
		public EarthState SetEarthState(EarthScript earthScript) {
			sunPostion = earthScript.GetSunPosition();
			earthRadius = earthScript.GetRadius();
			humidity = earthScript.humidity;
			temperature = earthScript.tempeture;
			return this;
        }
	}

	public void SetUpEarth(int size, float simulationSpeed) {
		this.simulationDeltaTime = simulationSpeed / 10;
		this.size = size;
		transform.localScale = new Vector3(size, size, size);
		SetupFoodTypeIndex();
		SetupSpeciesFoodType();
		User.Instance.ChangedSettings += OnSettingsChanged;
		frameManager = GetComponent<FrameManager>();
		frameManager.SetWantedItterationsPerFrame(User.Instance.GetFramesPerSeccondUserPref());
		zoneController = GetComponent<ZoneController>();
		zoneController.SetupZoneController(this);
		zoneController.SpawnZones(size,SimulationScript.Instance.numberOfZones,SimulationScript.Instance.maxNeiboringZones, SpeciesManager.Instance.GetAllStartingPlantsAndSeeds() * 5, SpeciesManager.Instance.GetAllStartingAnimals() * 5, SimulationScript.Instance.zoneSetup);
		earthState = new EarthState();
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
		frameManager.SetItterationsThatOccuredThisFrame(frameManager.GetItterationsPerFrame());
	}

	void UpdateSimualtion() {
		if (simulationUpdateStatus == SimulationUpdateStatus.SettingUp) {
			StartFindZoneJobs();
			UpdateWorldTime();
			UpdateHumidity();
            SimulationScript.Instance.GetSun().UpdateSun();
			UpdateEarthState();
			CompleteFindZoneJobs();
			UpdateOrganismData();
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
			simulationUpdateStatus = SimulationUpdateStatus.CleaningUp;
        }
		if (simulationUpdateStatus == SimulationUpdateStatus.CleaningUp) {
			UpdateOrganismLists();
			OnEndFrame?.Invoke(this, new EventArgs { });
			simulationUpdateStatus = SimulationUpdateStatus.SettingUp;
        }
	}

	void UpdateSimulationWithoutDelay() {
		StartFindZoneJobs();
		UpdateWorldTime();
		UpdateHumidity();
		SimulationScript.Instance.GetSun().UpdateSun();
		UpdateEarthState();
		CompleteFindZoneJobs();
		UpdateOrganismData();
        StartOrganismJobs();
        simulationUpdateStatus = SimulationUpdateStatus.Calculating;
        CompleteJobs();
        simulationUpdateStatus = SimulationUpdateStatus.Updating;
        UpdateOrganismsBehavior();
        UpdateOrganisms();
        simulationUpdateStatus = SimulationUpdateStatus.CleaningUp;
		UpdateOrganismLists();
        OnEndFrame?.Invoke(this, new EventArgs { });
		UpdateSpeciesMotorGraphData();
        simulationUpdateStatus = SimulationUpdateStatus.SettingUp;
    }

	public void StartFindZoneJobs() {
		activeJobs.Add(zoneController.FindZoneController.StartUpdateJob());
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

	void UpdateEarthState() {
		earthState = earthState.SetEarthState(this);
    }

	public void CompleteFindZoneJobs() {
		CompleteJobs();
		zoneController.FindZoneController.CompleteZoneJob();
    }

	void UpdateOrganismData() {
        for (int i = 0; i < GetAllSpecies().Count; i++) {
			GetAllSpecies()[i].UpdateOrganismData();
        }
    }

	void StartOrganismJobs() {
		List<BasicSpeciesScript> allSpecies = SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies();
        for (int i = 0; i < allSpecies.Count; i++) {
			activeJobs.Add(allSpecies[i].GetBasicJobController().StartUpdateJob());
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
		for (int i = activeJobs.Count - 1; i >= 0; i--) {
			activeJobs[i].Complete();
			activeJobs.RemoveAt(i);
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

	void UpdateOrganismLists() {
		List<BasicSpeciesScript> allSpecies = GetAllSpecies();
		for (int i = 0; i < allSpecies.Count; i++) {
			allSpecies[i].UpdateOrganismLists();
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

    #region FoodTypeManagment
	void SetupFoodTypeIndex() {
		typeIndex = new List<string>();
        for (int i = 0; i < GetAllSpecies().Count; i++) {
            for (int f = 0; f < GetAllSpecies()[i].GetOrganismFoodTypes().Count; f++) {
				AddFoodTypeToList(GetAllSpecies()[i].GetOrganismFoodTypes()[f]);
            }
        }
    }
	void AddFoodTypeToList(string foodType) {
		if (!typeIndex.Contains(foodType)) {
			typeIndex.Add(foodType);
		}
    }

	public void SetupSpeciesFoodType() {
        for (int i = 0; i < GetAllSpecies().Count; i++) {
			GetAllSpecies()[i].SetupSpeciesFoodType();
        }
        for (int i = 0; i < GetAllAnimalSpecies().Count; i++) {
			GetAllAnimalSpecies()[i].SetupAnimalPredatorSpeciesFoodType();
        }
    }


	public int GetIndexOfFoodType(string foodType) {
        for (int i = 0; i < typeIndex.Count; i++) {
			if (typeIndex[i] == foodType) {
				return i;
			}
        }
		return -1;
    }
    #endregion

    #region GetMethods
    public List<BasicSpeciesScript> GetAllSpecies() {
		return SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies();
    }

	public List<PlantSpecies> GetAllPlantSpecies() {
		return SpeciesManager.Instance.GetSpeciesMotor().GetAllPlantSpecies();
    }

	public List<AnimalSpecies> GetAllAnimalSpecies () {
		return SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies();
	}

	public List<BasicJobController> GetAllJobControllers() {
		List<BasicJobController> jobControllers = new List<BasicJobController>();
        foreach (var animalSpecies in SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()) {
			jobControllers.Add(animalSpecies.GetBasicJobController());
        }
		return jobControllers;
	}

    public Transform GetOrganismsTransform () {
		return transform.GetChild(1);
    }

	public Transform GetInactiveObjectsTransform() {
		return transform.GetChild(2);
    }

	MeshRenderer GetAtmosphereRenderer () {
		return transform.GetChild(0).GetComponent<MeshRenderer>();
    }

	public FrameManager GetFrameManager() {
		return frameManager;
    }

	public ZoneController GetZoneController() {
		return zoneController;
    }

	public float GetRadius() {
		return size;
    }

	public Vector3 GetSunPosition() {
		return sunTransform.position;
    }
    #endregion
}

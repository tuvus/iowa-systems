using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class Earth : MonoBehaviour {
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
    public long worldTime;
    public int maxTime;
    //The acuracy of the simulation 1f = 1 hour
    public float simulationDeltaTime;
    public float tempeture;
    public float humidity;
    public float humidityTarget;
    public EarthState earthState { private set; get; }
    #endregion

    public event EventHandler<EventArgs> OnEndFrame;

    List<JobHandle> activeJobs = new List<JobHandle>();
    SimulationUpdateStatus simulationUpdateStatus;
    List<string> typeIndex;

    public struct EarthState {
        public Vector3 sunPostion;
        public float earthRadius;
        public float humidity;
        public float temperature;
        public EarthState SetEarthState(Earth earthScript) {
            sunPostion = earthScript.GetSunPosition();
            earthRadius = earthScript.GetRadius();
            humidity = earthScript.humidity;
            temperature = earthScript.tempeture;
            return this;
        }
    }

    public void SetUpEarth(int size, float simulationSpeed) {
        this.simulationDeltaTime = simulationSpeed;
        this.size = size;
        transform.localScale = new Vector3(size, size, size);
        SetupFoodTypeIndex();
        SetupSpeciesFoodType();
        frameManager = GetComponent<FrameManager>();
        zoneController = GetComponent<ZoneController>();
        zoneController.SetupZoneController(this);
        zoneController.SpawnZones(size, Simulation.Instance.numberOfZones, Simulation.Instance.maxNeiboringZones, SpeciesManager.Instance.GetAllStartingPlantsAndSeeds() * 5, SpeciesManager.Instance.GetAllStartingAnimals() * 5, Simulation.Instance.zoneSetup);
        earthState = new EarthState();
    }

    public void StartSimulation() {
        simulationUpdateStatus = SimulationUpdateStatus.Intializing;
    }

    #region Runtime
    /// <summary>
    /// Finishes any previous half updates that might have occured.
    /// Then updates the simulation a number of times based on input and preformance capabilities of the computer.
    /// </summary>
    private void Update() {
        if (!Simulation.Instance.simulationInitialised)
            return;
        frameManager.UpdateFrameStartTime();
        if (simulationUpdateStatus != SimulationUpdateStatus.Intializing && simulationUpdateStatus != SimulationUpdateStatus.SettingUp) {
            UpdateSimualtionWithDelay();
        }
        int iterationsThisFrame = 0;
        if (frameManager.GetWantedIterationsPerSeccond() > 0) {
            int wantedIterationsThisFrame = frameManager.GetWantedIterationsThisFrame();
            if (wantedIterationsThisFrame > 0) {
                do {
                    if (iterationsThisFrame == 0 || frameManager.CanStartNewIterationBeforeNextFrame()) {
                        frameManager.LogSimulationIterationStart();
                        UpdateSimulationWithoutDelay();
                        frameManager.LogSimulationIterationEnd();
                    } else {
                        UpdateSimualtionWithDelay();
                        iterationsThisFrame++;
                        break;
                    }
                    iterationsThisFrame++;
                } while (iterationsThisFrame < wantedIterationsThisFrame && frameManager.ShouldStartNewIteration());
            }
            frameManager.EndFrame(iterationsThisFrame);
        }
    }

    /// <summary>
    /// Updates the simulation until frameManager.IsInIterationTimePeriod is false or the calculation is complete.
    /// </summary>
    void UpdateSimualtionWithDelay() {
        if (simulationUpdateStatus == SimulationUpdateStatus.SettingUp) {
            StartFindZoneJobs();
            UpdateWorldTime();
            UpdateHumidity();
            Simulation.Instance.GetSun().UpdateSun();
            UpdateEarthState();
            CompleteFindZoneJobs();
            UpdateOrganismData();
            StartOrganismJobs();
            simulationUpdateStatus = SimulationUpdateStatus.Calculating;
            if (!frameManager.IsInIterationTimePeriod())
                return;
        }
        if (simulationUpdateStatus == SimulationUpdateStatus.Calculating) {
            UpdateJobList();
            if (activeJobs.Count == 0) {
                simulationUpdateStatus = SimulationUpdateStatus.Updating;
            }
            if (!frameManager.IsInIterationTimePeriod())
                return;
        }
        if (simulationUpdateStatus == SimulationUpdateStatus.Updating) {
            UpdateOrganismsBehavior();
            UpdateOrganisms();
            simulationUpdateStatus = SimulationUpdateStatus.CleaningUp;
            if (!frameManager.IsInIterationTimePeriod())
                return;
        }
        if (simulationUpdateStatus == SimulationUpdateStatus.CleaningUp) {
            UpdateOrganismLists();
            OnEndFrame?.Invoke(this, new EventArgs { });
            simulationUpdateStatus = SimulationUpdateStatus.SettingUp;
        }
    }

    /// <summary>
    /// Updates the simulation without any delay.
    /// </summary>
    void UpdateSimulationWithoutDelay() {
        StartFindZoneJobs();
        UpdateWorldTime();
        UpdateHumidity();
        Simulation.Instance.GetSun().UpdateSun();
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
        worldTime++;
    }

    void UpdateHumidity() {
        if (humidityTarget > humidity) {
            humidity += Simulation.randomGenerator.NextFloat(-0.5f * simulationDeltaTime, 2f * simulationDeltaTime);
            if (humidityTarget <= humidity) {
                SetHumidityTarget();
            }
            return;
        }
        if (humidityTarget < humidity) {
            humidity += Simulation.randomGenerator.NextFloat(-2f * simulationDeltaTime, 0.5f * simulationDeltaTime);
            if (humidityTarget >= humidity) {
                SetHumidityTarget();
            }
            return;
        }
    }

    public void SetHumidityTarget() {
        humidityTarget = Simulation.randomGenerator.NextFloat(0, 100f);
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
        List<Species> allSpecies = SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies();
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
        List<Species> allSpecies = GetAllSpecies();
        for (int i = 0; i < allSpecies.Count; i++) {
            allSpecies[i].UpdateOrganismsBehavior();
        }
    }

    void UpdateOrganisms() {
        List<Species> allSpecies = GetAllSpecies();
        for (int i = 0; i < allSpecies.Count; i++) {
            allSpecies[i].UpdateOrganisms();
        }
    }

    void UpdateOrganismLists() {
        List<Species> allSpecies = GetAllSpecies();
        for (int i = 0; i < allSpecies.Count; i++) {
            allSpecies[i].UpdateOrganismLists();
        }
    }

    void UpdateSpeciesMotorGraphData() {
        SpeciesManager.Instance.GetSpeciesMotor().UpdateSpeciesGraphData();
    }
    #endregion

    public void OnSettingsChanged(bool renderWorld, int framesPerSeccond) {
        GetComponent<MeshRenderer>().enabled = renderWorld;
        if (renderWorld && QualitySettings.GetQualityLevel() > 1) {
            GetAtmosphereRenderer().enabled = true;
        } else {
            GetAtmosphereRenderer().enabled = false;
        }
        for (int i = 0; i < GetAllSpecies().Count; i++) {
            GetAllSpecies()[i].OnSettingsChanged(renderWorld);
        }
        frameManager.SetFramesPerSeccond(framesPerSeccond);
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
    public List<Species> GetAllSpecies() {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies();
    }

    public List<PlantSpecies> GetAllPlantSpecies() {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllPlantSpecies();
    }

    public List<AnimalSpecies> GetAllAnimalSpecies() {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies();
    }

    public List<JobController> GetAllJobControllers() {
        List<JobController> jobControllers = new List<JobController>();
        foreach (var animalSpecies in SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()) {
            jobControllers.Add(animalSpecies.GetBasicJobController());
        }
        return jobControllers;
    }

    public Transform GetOrganismsTransform() {
        return transform.GetChild(1);
    }

    public Transform GetInactiveObjectsTransform() {
        return transform.GetChild(2);
    }

    MeshRenderer GetAtmosphereRenderer() {
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

    public void OnDestroy() {
        CompleteJobs();
    }
}

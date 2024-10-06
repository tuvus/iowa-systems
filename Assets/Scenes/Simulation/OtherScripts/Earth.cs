using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

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
    [Tooltip("The radius of the earth model")]
    public int size;
    [Tooltip("The max time in hours that the simulation has been running")]
    public long worldTime;
    [Tooltip("The max time in hours until the simulation ends")]
    public long maxTime;
    [Tooltip("The change in time of this frame in hours")]
    public float simulationDeltaTime;
    [Tooltip("The average temperature in celcius")]
    public float temperature;
    public float humidity;
    public float humidityTarget;
    public EarthState earthState { private set; get; }

    public static Earth earth;
    #endregion

    public event EventHandler<EventArgs> OnEndFrame;

    HashSet<Thread> activeThreads = new HashSet<Thread>();
    SimulationUpdateStatus simulationUpdateStatus;
    public readonly HashSet<string> footTypesUsed = new HashSet<string>();

    public struct EarthState {
        public Vector3 sunPostion;
        public float earthRadius;
        public float humidity;
        public float temperature;
        public EarthState SetEarthState(Earth earthScript) {
            sunPostion = earthScript.GetSunPosition();
            earthRadius = earthScript.GetRadius();
            humidity = earthScript.humidity;
            temperature = earthScript.temperature;
            return this;
        }
    }

    public void SetUpEarth(int size, float simulationSpeed) {
        earth = this;
        this.simulationDeltaTime = simulationSpeed;
        this.size = size;
        transform.localScale = new Vector3(size, size, size);
        SetupFoodTypes();
        SetupSpeciesFoodType();
        frameManager = GetComponent<FrameManager>();
        zoneController = GetComponent<ZoneController>();
        zoneController.SetupZoneController(this);
        zoneController.SpawnZones(size, Simulation.Instance.numberOfZones, Simulation.Instance.maxNeiboringZones, SpeciesManager.Instance.GetAllStartingPlantsAndSeeds() * 5, SpeciesManager.Instance.GetAllStartingAnimals() * 5, Simulation.Instance.zoneSetup);
        earthState = new EarthState();
        humidity = 50;
        temperature = 100;
        EndUpdate();
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
        if (!Simulation.Instance.simulationRunning)
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
            Profiler.BeginSample("Setup");
            UpdateWorldTime();
            UpdateHumidity();
            Simulation.Instance.GetSun().UpdateSun();
            UpdateEarthState();
            Profiler.EndSample();
            Profiler.BeginSample("JobsSetup");
            StartOrganismJobs();
            Profiler.EndSample();
            simulationUpdateStatus = SimulationUpdateStatus.Calculating;
            if (!frameManager.IsInIterationTimePeriod())
                return;
        }
        if (simulationUpdateStatus == SimulationUpdateStatus.Calculating) {
            UpdateJobList();
            if (activeThreads.Count == 0) {
                simulationUpdateStatus = SimulationUpdateStatus.Updating;
            }
            if (!frameManager.IsInIterationTimePeriod())
                return;
        }
        if (simulationUpdateStatus == SimulationUpdateStatus.Updating) {
            Profiler.BeginSample("UpdateBehaviour");
            Profiler.EndSample();
            Profiler.BeginSample("Update");
            UpdateOrganismActions();
            Profiler.EndSample();
            simulationUpdateStatus = SimulationUpdateStatus.CleaningUp;
            if (!frameManager.IsInIterationTimePeriod())
                return;
        }
        if (simulationUpdateStatus == SimulationUpdateStatus.CleaningUp) {
            Profiler.BeginSample("CleaningUp");
            OnEndFrame?.Invoke(this, new EventArgs { });
            EndUpdate();
            Profiler.EndSample();
            simulationUpdateStatus = SimulationUpdateStatus.SettingUp;
        }
    }

    /// <summary>
    /// Updates the simulation without any delay.
    /// </summary>
    void UpdateSimulationWithoutDelay() {
        Profiler.BeginSample("Setup");
        UpdateWorldTime();
        UpdateHumidity();
        Simulation.Instance.GetSun().UpdateSun();
        UpdateEarthState();
        Profiler.EndSample();
        Profiler.BeginSample("Jobs");
        StartOrganismJobs();
        simulationUpdateStatus = SimulationUpdateStatus.Calculating;
        CompleteJobs();
        Profiler.EndSample();
        Profiler.BeginSample("UpdateBehaviour");
        simulationUpdateStatus = SimulationUpdateStatus.Updating;
        Profiler.EndSample();
        Profiler.BeginSample("Update");
        UpdateOrganismActions();
        Profiler.EndSample();
        Profiler.BeginSample("CleaningUp");
        simulationUpdateStatus = SimulationUpdateStatus.CleaningUp;
        OnEndFrame?.Invoke(this, new EventArgs { });
        EndUpdate();
        UpdateSpeciesMotorGraphData();
        Profiler.EndSample();
        simulationUpdateStatus = SimulationUpdateStatus.SettingUp;
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

    void StartOrganismJobs() {
        List<Species> allSpecies = SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies();
        foreach (var species in allSpecies) {
            species.StartJobs(activeThreads);
        }
    }

    void UpdateJobList() {
        activeThreads.RemoveWhere(t => !t.IsAlive);
    }

    void CompleteJobs() {
        while (activeThreads.Count != 0) {
            activeThreads.First().Join();
        }
        activeThreads.RemoveWhere(t => !t.IsAlive);
    }

    //TODO: Add recursive asynchronous organismAction handling
    //TODO: Add linear CleanActiveOrganismList handling
    //TODO: Change speciesOrgan to attribute and make an organ that inherits from it.

    void UpdateOrganismActions() {
        List<Species> allSpecies = GetAllSpecies();
        for (int i = 0; i < allSpecies.Count; i++) {
            Profiler.BeginSample("UpdateOrganismActions" + allSpecies[i].speciesName);
            // allSpecies[i].UpdateOrganismActions();
            Profiler.EndSample();
        }
    }

    void EndUpdate() {
        GetAllSpecies().ForEach(s => s.EndUpdate());
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
            //GetAllSpecies()[i].OnSettingsChanged(renderWorld);
        }
        frameManager.SetFramesPerSeccond(framesPerSeccond);
    }

    #region FoodTypeManagment
    void SetupFoodTypes() {
        foreach (var species in GetAllSpecies()) {
            foreach (var footType in species.GetOrganismFoodTypes()) {
                footTypesUsed.Add(footType);
            }
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
    #endregion

    #region GetMethods
    public Species GetSpecies(int index) {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[index];

    }
    public List<Species> GetAllSpecies() {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies();
    }

    public List<PlantSpecies> GetAllPlantSpecies() {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllPlantSpecies();
    }

    public List<AnimalSpecies> GetAllAnimalSpecies() {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies();
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
}

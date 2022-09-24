using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public abstract class Species : MonoBehaviour {
    private Earth earth;
    public string speciesName;
    public string speciesDisplayName;
    public Color32 speciesColor;
    [SerializeField] internal int startingPopulation;
    public List<SpeciesOrgan> organs = new List<SpeciesOrgan>();

    public int speciesIndex;
    internal List<int> populationOverTime = new List<int>();
    public int populationCount { internal set; get; }

    public struct Organism {
        [Tooltip("The index of the species")]
        public int speciesIndex;
        [Tooltip("The index of the organism in the activeSpeciesArray")]
        public int activeOrganismIndex;
        [Tooltip("The age of the organism in days")]
        public float age;
        [Tooltip("The zone that the organism is in")]
        public int zone;
        [Tooltip("The rotation from the center of the world")]
        public float3 position;
        [Tooltip("The rotation tangent to the world")]
        public float rotation;
        [Tooltip("Is the organism spawned or not")]
        public bool spawned;

        public Organism(Organism organismData, int speciesActiveOrganismIndex, bool spawned) {
            speciesIndex = organismData.speciesIndex;
            this.activeOrganismIndex = speciesActiveOrganismIndex;
            age = organismData.age;
            zone = organismData.zone;
            position = organismData.position;
            rotation = organismData.rotation;
            this.spawned = spawned;
        }

        public Organism(Organism organismData, float age, int zone, float3 position, float rotation, int speciesActiveOrganismIndex, bool spawned) {
            speciesIndex = organismData.speciesIndex;
            this.activeOrganismIndex = speciesActiveOrganismIndex;
            this.age = age;
            this.zone = zone;
            this.position = position;
            this.rotation = rotation;
            this.spawned = spawned;
        }

        public Organism(Organism organismData, float age, int zone, float3 position, float rotation) {
            speciesIndex = organismData.speciesIndex;
            activeOrganismIndex = organismData.activeOrganismIndex;
            this.age = age;
            this.zone = zone;
            this.position = position;
            this.rotation = rotation;
            spawned = organismData.spawned;
        }

        public Organism(Organism organismData, float age) {
            speciesIndex = organismData.speciesIndex;
            activeOrganismIndex = organismData.activeOrganismIndex;
            this.age = age;
            this.zone = organismData.zone;
            this.position = organismData.position;
            this.rotation = organismData.rotation;
            spawned = organismData.spawned;
        }

        public Organism(Organism organismData, int zone) {
            speciesIndex = organismData.speciesIndex;
            activeOrganismIndex = organismData.activeOrganismIndex;
            this.age = organismData.age;
            this.zone = zone;
            this.position = organismData.position;
            this.rotation = organismData.rotation;
            spawned = organismData.spawned;
        }
    }

    public NativeArray<Organism> organisms;
    public NativeArray<int> activeOrganisms;
    public int activeOrganismsCount;
    public NativeArray<int> inactiveOrganisms;
    public int inactiveOrganismsCount;
    public NativeQueue<int> deadOrganismQueue;
    public NativeQueue<int>.ParallelWriter deadOrganismQueueParallel;

    SpeciesUpdateJob speciesUpdateJob;

    #region SimulationStart
    public virtual void SetupSimulation(Earth earth) {
        this.earth = earth;
        gameObject.name = speciesName;
        for (int i = 0; i < organs.Count; i++) {
            organs[i].SetSpeciesScript(this);
        }
        SetupArrays(startingPopulation * 2);
    }

    public virtual void SetupArrays(int arrayLength) {
        organisms = new NativeArray<Organism>(arrayLength, Allocator.Persistent);
        activeOrganisms = new NativeArray<int>(arrayLength, Allocator.Persistent);
        inactiveOrganisms = new NativeArray<int>(arrayLength, Allocator.Persistent);
        deadOrganismQueue = new NativeQueue<int>(Allocator.Persistent);
        deadOrganismQueueParallel = deadOrganismQueue.AsParallelWriter();
        speciesUpdateJob = new SpeciesUpdateJob(this);
        for (int i = 0; i < organisms.Length; i++) {
            inactiveOrganisms[i] = i;
        }
        inactiveOrganismsCount = organisms.Length;
        activeOrganismsCount = 0;
        foreach (var organ in GetComponents<SpeciesOrgan>()) {
            organ.SetSpeciesScript(this);
            organ.SetupSpeciesOrganArrays(arrayLength);
        }
    }

    public abstract void SetupSpeciesFoodType();

    public abstract List<string> GetOrganismFoodTypes();

    public abstract void StartSimulation();

    public abstract void Populate();
    #endregion

    #region SpawnOrganisms
    /// <summary>
    /// Spawns a new Organism.
    /// </summary>
    /// <returns>The index of the new organism</returns>
    public virtual int SpawnOrganism() {
        int organismIndex = ActivateInactiveOrganism();
        organisms[organismIndex] = new Organism(organisms[organismIndex], activeOrganismsCount - 1, true);
        return organismIndex;
    }

    /// <summary>
    /// Spawns a new organism within distance degrees of position.
    /// </summary>
    /// <param name="position">The position to be randomised around</param>
    /// <param name="distance">The distance in degrees from the position</param>
    /// <returns>The index of the new organism</returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual int SpawnOrganism(float3 position, float distance) {
        int organism = ActivateInactiveOrganism();
        organisms[organism] = new Organism(organisms[organism],0,0, position, 0, activeOrganismsCount - 1, true);
        throw new NotImplementedException("Need to add position and rotation here.");
        return organism;
    }

    /// <summary>
    /// Gets an inactive organism, activates it and returns it.
    /// May change the size of the organism arrays
    /// </summary>
    /// <returns>A new active organism</returns>
    private int ActivateInactiveOrganism() {
        //Make sure that there are free inactive organisms to get
        if (inactiveOrganismsCount == 0) {
            IncreaseOrganismSize(organisms.Length * 2);
        }
        //Get the first inactive organism and remove it from the inactiveOrganisms list
        int newOrganism = inactiveOrganisms[inactiveOrganismsCount - 1];
        inactiveOrganismsCount--;
        //Add the organism to the activeOrganismsList
        activeOrganisms[activeOrganismsCount] = newOrganism;
        activeOrganismsCount++;
        return newOrganism;
    }

    /// <summary>
    /// Removes the organism from the active list and adds it to the inactive list.
    /// Does not acualy change the organism's data.
    /// If the organism's index does not match the value of activeOrganisms at the organim's activeOrganismIndex
    /// then the deactivation has already occured and nothing will be done.
    /// </summary>
    /// <param name="organismIndex">The index of the organism</param>
    public void DeactivateActiveOrganism(int organismIndex) {
        //Check if the organism is still active
        if (organisms[organismIndex].activeOrganismIndex >= activeOrganismsCount || activeOrganisms[organisms[organismIndex].activeOrganismIndex] != organismIndex)
            return;
        //Remove the organism from the active list
        for (int i = organisms[organismIndex].activeOrganismIndex; i < activeOrganismsCount - 1; i++) {
            activeOrganisms[i] = activeOrganisms[i + 1];
        }
        activeOrganismsCount--;
        //Add the organism to the inactive list
        inactiveOrganisms[inactiveOrganismsCount] = organismIndex;
        inactiveOrganismsCount++;
    }

    /// <summary>
    /// Increases the size of the organism arrays, active and inactive arrays.
    /// Also increases the size for all of the organs, plants and animals liked with it.
    /// </summary>
    /// <param name="newSize"></param>
    protected virtual void IncreaseOrganismSize(int newSize) {
        throw new NotImplementedException("IncreaseOrganismSize has not been implamented yet.");
    }
    #endregion

    public struct SpeciesUpdateJob : IJobParallelFor {
        private Species species;

        public SpeciesUpdateJob(Species species) {
            this.species = species;
        }

        public JobHandle BeginJob() {
            return IJobParallelForExtensions.Schedule(this, species.activeOrganismsCount, species.activeOrganismsCount);
        }

        public void Execute(int index) {
            species.UpdateOrganism(index);
        }
    }

    protected virtual void UpdateOrganism(int organism) {
        organisms[organism] = new Organism(organisms[organism], organisms[organism].age + earth.simulationDeltaTime / 24);
    }

    public virtual void UpdateDeadOrganisms() {
        while (!deadOrganismQueue.IsEmpty()) {
            //No need to worry about deactivating an already inactive organism, it is handled in DeactivateActiveOrganism()
            DeactivateActiveOrganism(deadOrganismQueue.Dequeue());
        }
    }

    public void AddToFindZone(int organism, int zone = -1, float range = 0) {
        //GetEarth().GetZoneController().FindZoneController.AddFindZoneData(new FindZoneController.FindZoneData(new ZoneController.DataLocation(plant), zone, plant.position, range));
        throw new NotImplementedException();
    }

    #region PopulationCountGraph
    public List<int> ReturnPopulationList() {
        return populationOverTime;
    }

    public void RefreshPopulationList() {
        populationOverTime.Add(GetCurrentPopulation());
    }

    public int GetCurrentPopulation() {
        return populationCount;
    }
    #endregion

    #region OrganismControls
    public abstract void OnSettingsChanged(bool renderOrganisms);

    /// <summary>
    /// Called right when an organism detects it should be dead.
    /// </summary>
    public void OrganismDeath() {
        populationCount--;
        if (populationCount == 0)
            User.Instance.PrintState("Species has died out after " + (int)(earth.worldTime / 24) + " days.", speciesDisplayName, 3);
    }
    #endregion

    public Earth GetEarth() {
        return earth;
    }

    public virtual void OnDestroy() {
        if (organisms.IsCreated)
            organisms.Dispose();
        if (activeOrganisms.IsCreated)
            activeOrganisms.Dispose();
        if (inactiveOrganisms.IsCreated)
            inactiveOrganisms.Dispose();
        if (deadOrganismQueue.IsCreated)
            deadOrganismQueue.Dispose();
    }
}
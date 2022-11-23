using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
        [Tooltip("The max index of the organism in the activeSpeciesArray")]
        public int maxActiveOrganismIndex;
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

        public Organism(Organism organismData, int maxSpeciesActiveOrganismIndex, bool spawned) {
            speciesIndex = organismData.speciesIndex;
            this.maxActiveOrganismIndex = maxSpeciesActiveOrganismIndex;
            age = organismData.age;
            zone = organismData.zone;
            position = organismData.position;
            rotation = organismData.rotation;
            this.spawned = spawned;
        }

        public Organism(Organism organismData, float age, int zone, float3 position, float rotation, int maxSpeciesActiveOrganismIndex, bool spawned) {
            speciesIndex = organismData.speciesIndex;
            this.maxActiveOrganismIndex = maxSpeciesActiveOrganismIndex;
            this.age = age;
            this.zone = zone;
            this.position = position;
            this.rotation = rotation;
            this.spawned = spawned;
        }

        public Organism(Organism organismData, float age, int zone, float3 position, float rotation) {
            speciesIndex = organismData.speciesIndex;
            maxActiveOrganismIndex = organismData.maxActiveOrganismIndex;
            this.age = age;
            this.zone = zone;
            this.position = position;
            this.rotation = rotation;
            spawned = organismData.spawned;
        }

        public Organism(Organism organismData, float age) {
            speciesIndex = organismData.speciesIndex;
            maxActiveOrganismIndex = organismData.maxActiveOrganismIndex;
            this.age = age;
            this.zone = organismData.zone;
            this.position = organismData.position;
            this.rotation = organismData.rotation;
            spawned = organismData.spawned;
        }

        public Organism(Organism organismData, int zone) {
            speciesIndex = organismData.speciesIndex;
            maxActiveOrganismIndex = organismData.maxActiveOrganismIndex;
            this.age = organismData.age;
            this.zone = zone;
            this.position = organismData.position;
            this.rotation = organismData.rotation;
            spawned = organismData.spawned;
        }
    }

    public struct OrganismAction {
        public enum Action {
            Starve,
            Die,
            Bite,
            Eat,
            Reproduce,
        }
        public Action action;
        public int organism;
        public int2 target;
        public float3 position;
        public int zone;
        public int amount;
        [Tooltip("Either dispertion range or bite size")]
        public float floatValue;

        public OrganismAction(Action action, int organism) {
            this.action = action;
            this.organism = organism;
            this.target = -1;
            this.position = float3.zero;
            this.zone = -1;
            this.amount = -1;
            this.floatValue = -1;
        }

        public OrganismAction(Action action, int organism, int2 target, float biteSize) {
            this.action = action;
            this.organism = organism;
            this.target = target;
            this.position = float3.zero;
            this.zone = -1;
            this.amount = -1;
            this.floatValue = biteSize;
        }

        public OrganismAction(Action action, int organism, Species species, int amount, float dispertionRange) {
            this.action = action;
            this.organism = organism;
            this.target = int2.zero;
            this.position = species.organisms[organism].position;
            this.zone = species.organisms[organism].zone;
            this.amount = amount;
            this.floatValue = dispertionRange;
        }
    }

    [NativeDisableContainerSafetyRestriction] public NativeArray<Organism> organisms;
    public NativeArray<int> activeOrganisms;
    public int activeOrganismsCount;
    public NativeArray<int> inactiveOrganisms;
    public int inactiveOrganismsCount;
    public NativeArray<OrganismAction> organismActions;
    public int organismActionsCount;

    SpeciesUpdateJob speciesUpdateJob;

    #region SimulationStart
    public virtual void SetupSimulation(Earth earth) {
        this.earth = earth;
        gameObject.name = speciesName;
        for (int i = 0; i < organs.Count; i++) {
            organs[i].SetSpeciesScript(this);
        }
        SetupArrays(math.max(startingPopulation * 2, 100));
    }

    public virtual void SetupArrays(int arrayLength) {
        organisms = new NativeArray<Organism>(arrayLength, Allocator.Persistent);
        activeOrganisms = new NativeArray<int>(arrayLength, Allocator.Persistent);
        inactiveOrganisms = new NativeArray<int>(arrayLength, Allocator.Persistent);
        organismActions = new NativeArray<OrganismAction>(arrayLength, Allocator.Persistent);
        speciesUpdateJob = new SpeciesUpdateJob(speciesIndex);
        for (int i = 0; i < organisms.Length; i++) {
            inactiveOrganisms[i] = i;
        }
        inactiveOrganismsCount = organisms.Length;
        activeOrganismsCount = 0;
        organismActionsCount = 0;
        for (int i = 0; i < organs.Count; i++) {
            organs[i].SetupSpeciesOrganArrays(arrayLength);
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
        //Debug.LogWarning("Need to add position and rotation here.");
        return organismIndex;
    }

    /// <summary>
    /// Spawns a new organism within distance degrees of position.
    /// </summary>
    /// <param name="position">The position to be randomised around</param>
    /// <param name="zone">The zone that the position is in</param>
    /// <param name="distance">The distance in degrees from the position</param>
    /// <returns>The index of the new organism</returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual int SpawnOrganism(float3 position, int zone, float distance) {
        int organism = ActivateInactiveOrganism();
        organisms[organism] = new Organism(organisms[organism], 0, zone, position, 0, activeOrganismsCount - 1, true);
        //Debug.LogWarning("Need to add position and rotation here.");
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
    /// Resets the organism's data and sets it to not spawned.
    /// If the organism's index does not match the value of activeOrganisms at the organim's activeOrganismIndex
    /// then the deactivation has already occured and nothing will be done.
    /// </summary>
    /// <param name="organismIndex">The index of the organism</param>
    public void DeactivateActiveOrganism(int organismIndex) {
        //Check if the organism is still active
        if (!organisms[organismIndex].spawned)
            return;
        //Finds the activeOrganismIndex starting at maxActiveOrganismIndex and works it's way to the begining.
        //Because of the way activeOrganisms are removed the index must be equal to or less than maxActiveOrganismIndex.
        int activeOrganismIndex = organisms[organismIndex].maxActiveOrganismIndex;
        for (; activeOrganismIndex >= -1; activeOrganismIndex--) {
            if (activeOrganisms[activeOrganismIndex] == organismIndex)
                break;
        }
        //Remove the organism from the active list
        for (int i = activeOrganismIndex; i < activeOrganismsCount - 1; i++) {
            activeOrganisms[i] = activeOrganisms[i + 1];
        }
        activeOrganismsCount--;
        //Add the organism to the inactive list
        inactiveOrganisms[inactiveOrganismsCount] = organismIndex;
        inactiveOrganismsCount++;
        organisms[organismIndex] = new Organism(organisms[organismIndex], -2, false);
    }

    /// <summary>
    /// Increases the size of the organism arrays, active and inactive arrays.
    /// Also increases the size for all of the organs, plants and animals liked with it.
    /// </summary>
    /// <param name="newSize"></param>
    protected virtual void IncreaseOrganismSize(int newSize) {
        NativeArray<Organism> oldOrganisms = organisms;
        organisms = new NativeArray<Organism>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldOrganisms.Length; i++) {
            organisms[i] = oldOrganisms[i];
        }
        oldOrganisms.Dispose();
        NativeArray<int> oldActiveOrganisms = activeOrganisms;
        activeOrganisms = new NativeArray<int>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldActiveOrganisms.Length; i++) {
            activeOrganisms[i] = oldActiveOrganisms[i];
        }
        oldActiveOrganisms.Dispose();
        NativeArray<int> oldInActiveOrganisms = inactiveOrganisms;
        inactiveOrganisms = new NativeArray<int>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldInActiveOrganisms.Length; i++) {
            inactiveOrganisms[i] = oldInActiveOrganisms[i];
        }
        //Add new inactiveOrganisms to the inactiveOrganismList and increment inactiveOrganismCount
        for (int i = oldInActiveOrganisms.Length; i < inactiveOrganisms.Length; i++) {
            inactiveOrganisms[inactiveOrganismsCount] = i;
            inactiveOrganismsCount++;
        }
        oldInActiveOrganisms.Dispose();
        for (int i = 0; i < organs.Count; i++) {
            organs[i].IncreaseOrganismSize(newSize);

        }
        NativeArray<OrganismAction> oldorganismActions = organismActions;
        organismActions = new NativeArray<OrganismAction>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldorganismActions.Length; i++) {
            organismActions[i] = oldorganismActions[i];
        }
        oldorganismActions.Dispose();
    }
    #endregion

    public struct SpeciesUpdateJob : IJobParallelFor {
        private int species;

        public SpeciesUpdateJob(int species) {
            this.species = species;
        }

        public JobHandle BeginJob() {
            return IJobParallelForExtensions.Schedule(this, SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species].activeOrganismsCount, 10);
        }

        public void Execute(int index) {
            SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species].UpdateOrganism(SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species].activeOrganisms[index]);
        }
    }

    public List<JobHandle> StartJobs() {
        List<JobHandle> jobs = new List<JobHandle>(2);
        jobs.Add(speciesUpdateJob.BeginJob());
        for (int i = 0; i < organs.Count; i++) {
            JobHandle? jobHandle = organs[i].StartJob();
            if (jobHandle.HasValue)
                jobs.Add(jobHandle.Value);
        }
        return jobs;
    }

    protected virtual void UpdateOrganism(int organism) {
        organisms[organism] = new Organism(organisms[organism], organisms[organism].age + earth.simulationDeltaTime / 24);
    }

    public virtual void UpdateOrganismActions() {
        while (organismActionsCount >= 0) {
            //No need to worry about deactivating an already inactive organism, it is handled in DeactivateActiveOrganism()
            switch (organismActions[organismActionsCount].action) {
                case OrganismAction.Action.Starve:
                    DeactivateActiveOrganism(organismActions[organismActionsCount].organism);
                    break;
                case OrganismAction.Action.Die:
                    DeactivateActiveOrganism(organismActions[organismActionsCount].organism);
                    break;
                case OrganismAction.Action.Bite:
                    break;
                case OrganismAction.Action.Eat:
                    break;
                case OrganismAction.Action.Reproduce:
                    ReproduceOrganism(organismActions[organismActionsCount]);
                    break;
            }
            organismActionsCount--;
        }
    }

    public virtual void ReproduceOrganism(OrganismAction action) {
        for (int i = 0; i < action.amount; i++) {
            SpawnOrganism(action.position, action.amount, action.floatValue);
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
        return activeOrganismsCount;
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
        if (organismActions.IsCreated)
            organismActions.Dispose();
        if (organismActions.IsCreated)
            organismActions.Dispose();
    }
}
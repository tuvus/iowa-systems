﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public abstract class Species : MonoBehaviour, IOrganismSpecies, IOrganismListCapacityChange {
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

        [Tooltip("The age of the organism in days")]
        public float age;
        [Tooltip("The zone that the organism is in")]
        public int zone;
        [Tooltip("The rotation from the center of the world")]
        public float3 position;
        [Tooltip("The rotation tangent to the world")]
        public float rotation;

        public Organism(float age, int zone, float3 position, float rotation) {
            this.age = age;
            this.zone = zone;
            this.position = position;
            this.rotation = rotation;
        }

        public Organism(Organism organismData, float age) {
            this.age = age;
            this.zone = organismData.zone;
            this.position = organismData.position;
            this.rotation = organismData.rotation;
        }

        public Organism(Organism organismData, int zone) {
            this.age = organismData.age;
            this.zone = zone;
            this.position = organismData.position;
            this.rotation = organismData.rotation;
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
            this.position = species.organismList.organisms[organism].position;
            this.zone = species.organismList.organisms[organism].zone;
            this.amount = amount;
            this.floatValue = dispertionRange;
        }

        //For readding the reproduce action back to the action queue with the same or less ammount of offspring
        public OrganismAction(OrganismAction oldAction, int newAmount) {
            this.action = oldAction.action;
            this.organism = oldAction.organism;
            this.target = int2.zero;
            this.position = oldAction.position;
            this.zone = oldAction.zone;
            this.amount = newAmount;
            this.floatValue = oldAction.floatValue;
        }
    }

    public OrganismList<Organism> organismList;
    public NativeArray<Organism> organisms;
    public OrganismActionQueue<OrganismAction> organismActions;

    SpeciesUpdateJob speciesUpdateJob;

    #region SimulationStart
    public virtual void SetupSimulation(Earth earth) {
        this.earth = earth;
        gameObject.name = speciesName;
        for (int i = 0; i < organs.Count; i++) {
            organs[i].SetSpeciesScript(this);
        }
        organismList = new OrganismList<Organism>(math.max(startingPopulation * 2, 100), this);
        organisms = organismList.organisms;
        organismActions = new OrganismActionQueue<OrganismAction>(organismList);
        speciesUpdateJob = new SpeciesUpdateJob(speciesIndex);
        for (int i = 0; i < organs.Count; i++) {
            organs[i].SetupSpeciesOrganArrays(organismList);
        }
    }

    public abstract void SetupSpeciesFoodType();

    public abstract List<string> GetOrganismFoodTypes();

    public abstract void StartSimulation();

    public abstract void Populate();
    #endregion

    #region SpawnOrganisms

    public virtual int SpawnOrganism() {
        int organismIndex = organismList.ActivateOrganism();
        //TODO: Need to add position and rotation here
        return organismIndex;
    }

    public virtual int SpawnOrganism(float3 position, int zone, float distance) {
        int? organism = organismList.ActivateOrganismParallel();
        if (!organism.HasValue)
            return -1;
        organisms[organism.Value] = new Organism(0, zone, position, 0);
        //TODO: Need to add position and rotation here
        return organism.Value;
    }

    /// <summary>
    /// Increases the size of the organism arrays, active and inactive arrays.
    /// Also increases the size for all of the organs, plants and animals liked with it.
    /// </summary>
    public virtual void OnListUpdate() {
        organisms = organismList.organisms;
        for (int i = 0; i < organs.Count; i++) {
            organs[i].OnListUpdate();
        }
    }
    #endregion

    public struct SpeciesUpdateJob : IJobParallelFor {
        private int species;

        public SpeciesUpdateJob(int species) {
            this.species = species;
        }

        public JobHandle BeginJob() {
            return IJobParallelForExtensions.Schedule(this, SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species].organismList.activeOrganismCount, 10);
        }

        public void Execute(int index) {
            SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species].UpdateOrganism(SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species].organismList.activeOrganisms[index]);
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
        while (!organismActions.Empty()) {
            //No need to worry about deactivating an already inactive organism, it is handled in DeactivateActiveOrganism()
            switch (organismActions.Peek().action) {
                case OrganismAction.Action.Die:
                case OrganismAction.Action.Starve:
                    organismList.DeactivateActiveOrganism(organismActions.Peek().organism);
                    break;
                case OrganismAction.Action.Bite:
                    break;
                case OrganismAction.Action.Eat:
                    break;
                case OrganismAction.Action.Reproduce:
                    ReproduceOrganismParallel(organismActions.Peek());
                    break;
            }
            organismActions.Dequeue();
        }
    }

    public virtual void ReproduceOrganismParallel(OrganismAction action) {
        int organismsToReproduce = action.amount;
        for (; organismsToReproduce > 0; organismsToReproduce--) {
            if (SpawnOrganism(action.position, action.amount, action.floatValue) == -1) {
                organismsToReproduce--;
                break;
            }
        }
        if (organismsToReproduce > 0) {
            organismActions.Enqueue(new OrganismAction(action, organismsToReproduce));
        }
    }

    public virtual void KillOrganismParallel(OrganismAction action) {
        organismList.DeactivateActiveOrganismParallel(action.organism);
    }

    public void AddToFindZone(int organism, int zone = -1, float range = 0) {
        //GetEarth().GetZoneController().FindZoneController.AddFindZoneData(new FindZoneController.FindZoneData(new ZoneController.DataLocation(plant), zone, plant.position, range));
        throw new NotImplementedException();
        //TODO: Implement Add to find zone
    }

    #region PopulationCountGraph
    public List<int> ReturnPopulationList() {
        return populationOverTime;
    }

    public void RefreshPopulationList() {
        populationOverTime.Add(GetCurrentPopulation());
    }

    public int GetCurrentPopulation() {
        return organismList.activeOrganismCount;
    }
    #endregion

    #region OrganismControls
    public abstract void OnSettingsChanged(bool renderOrganisms);
    #endregion

    public Earth GetEarth() {
        return earth;
    }

    /// <summary>
    /// Called both when species prefabs are destroyed in the intro and the simulation
    /// </summary>
    public virtual void Deallocate() {
        organismList.Deallocate();
        foreach (var organ in organs) {
            organ.Deallocate();
        }
    }

}
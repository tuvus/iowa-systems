﻿using System;
using System.Collections.Generic;
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

    public class Organism {

        [Tooltip("The age of the organism in days")]
        public float age;
        [Tooltip("The zone that the organism is in")]
        public int zone;
        [Tooltip("The rotation from the center of the world")]
        public float3 position;
        [Tooltip("The rotation tangent to the world")]
        public float rotation;

        public Organism() { }

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
        public Organism organism;
        public int2 target;
        public float3 position;
        public int zone;
        public int amount;
        [Tooltip("Either dispertion range or bite size")]
        public float floatValue;
    }

    public HashSet<Organism> organisms;
    public List<OrganismAction> organismActions;

    
    #region SimulationStart
    public virtual void SetupSimulation(Earth earth) {
        this.earth = earth;
        gameObject.name = speciesName;
        foreach (var speciesOrgan in organs) {
            speciesOrgan.SetSpeciesScript(this);
            speciesOrgan.SetupSpeciesOrgan();
        }
        organisms = new HashSet<Organism>();
    }

    public abstract void SetupSpeciesFoodType();

    public abstract List<string> GetOrganismFoodTypes();

    public abstract void StartSimulation();

    public abstract void Populate();
    #endregion

    #region SpawnOrganisms

    public virtual Organism SpawnOrganism() {
        Organism organism = new Organism();
        organisms.Add(organism);
        //TODO: Need to add position and rotation here
        return organism;
    }

    public virtual Organism SpawnOrganism(float3 position, int zone, float distance) {
        Organism organism = new Organism(0, zone, position, 0);
        organisms.Add(organism);
        //TODO: Need to add position and rotation here
        return organism;
    }
    #endregion

    protected virtual void UpdateOrganism(Organism organism) {
        organism.age += earth.simulationDeltaTime / 24;
    }

    #region PopulationCountGraph
    public List<int> ReturnPopulationList() {
        return populationOverTime;
    }

    public void RefreshPopulationList() {
        populationOverTime.Add(GetCurrentPopulation());
    }

    public int GetCurrentPopulation() {
        return organisms.Count;
    }
    #endregion

    #region OrganismControls
    public abstract void OnSettingsChanged(bool renderOrganisms);
    #endregion

    public Earth GetEarth() {
        return earth;
    }
}
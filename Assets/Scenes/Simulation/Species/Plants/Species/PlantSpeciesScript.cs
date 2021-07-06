using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesScript : BasicSpeciesScript {
	public GameObject plantPrefab;

	public float maxGrowth;

	internal List<PlantScript> plants = new List<PlantScript>();


    #region StartSimulation
    internal override void SetupSpecificSimulation() {
    }

    internal override void StartSimulation() {
	}

	internal override void StartSpecificSimulation() {
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();

		Populate();
	}
    #endregion

    #region SpawnOrganisms
    public override void Populate() {
		int organismsToSpawn = startingPopulation;
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnSpecificRandomOrganism();
		}
		if (GetComponent<PlantSpeciesSeeds>() != null) {
			GetComponent<PlantSpeciesSeeds>().Populate(earth);
		}
	}

    public override void SpawnSpecificRandomOrganism() {
		PlantScript plantScript = SpawnOrganism(plantPrefab).GetComponent<PlantScript>();
		SetupRandomOrganism(plantScript);
		plantScript.plantSpecies = this;
		plantScript.SetUpOrganism(this,null);
		plantScript.Grow(Random.Range(plantScript.maxGrowth / 2, plantScript.maxGrowth * 2),1);
		AddOrganism(plantScript);
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(plantScript);
		}
	}

	public Seed SpawnRandomSeed(GameObject seed) {
		Seed seedScript = SpawnOrganism(seed).GetComponent<Seed>();
		SetupRandomOrganism(seedScript);
		seedScript.SetUpOrganism(this, null);
		GetSpeciesSeeds().AddSeed(seedScript);
		return seedScript;
	}


	public Seed SpawnSeed(PlantScript parent, GameObject seed, float range) {
		Seed seedScript = SpawnOrganism(seed).GetComponent<Seed>();
		SetupChildOrganism(seedScript, parent, range);
		seedScript.SetUpOrganism(this, parent);
		GetEarthScript().OnEndFrame += seedScript.OnAddSeed;
		return seedScript;
	}

	public PlantScript SpawnOrganismFromSeed (Seed seed) {
		PlantScript plantScript = SpawnOrganism(plantPrefab).GetComponent<PlantScript>();
		SetupChildOrganism(plantScript, seed);
		plantScript.plantSpecies = this;
		plantScript.SetUpOrganism(this,seed.plantParent);
		SetUpOrgans(plantScript);
		GetEarthScript().OnEndFrame += plantScript.OnAddOrganism;
		return plantScript;
	}

	public override BasicOrganismScript SpawnSpecificOrganism(BasicOrganismScript _parent) {
		PlantScript plantScript = SpawnOrganism(plantPrefab).GetComponent<PlantScript>();
		plantScript.plantSpecies = this;
		plantScript.SetUpOrganism(this, _parent);
		SetUpOrgans(plantScript);
		GetEarthScript().OnEndFrame += plantScript.OnAddOrganism;
		return plantScript;
	}

	void SetUpOrgans(PlantScript plantScript) {
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(plantScript);
		}
	}
    #endregion

    #region PlantListControls
    internal override void AddSpecificOrganism(BasicOrganismScript newOrganism) {
		if (newOrganism.GetComponent<PlantScript>() != null) {
			plants.Add(newOrganism.GetComponent<PlantScript>());
			return;
        }
		if (GetSpeciesSeeds() != null && newOrganism.GetComponent<Seed>() != null) {
			GetSpeciesSeeds().seeds.Add(newOrganism.GetComponent<Seed>());
			return;
        }

	}
	
	internal override void SpecificOrganismDeath(BasicOrganismScript deadOrganism) {
		if (deadOrganism.GetComponent<PlantScript>() != null) {
			plants.Remove(deadOrganism.GetComponent<PlantScript>());
			return;
		}
		if (GetSpeciesSeeds() != null && deadOrganism.GetComponent<Seed>() != null) {
			GetSpeciesSeeds().seeds.Remove(deadOrganism.GetComponent<Seed>());
			return;
		}
	}
    #endregion

	public PlantSpeciesSeeds GetSpeciesSeeds() {
		return GetComponent<PlantSpeciesSeeds>();
    }
}

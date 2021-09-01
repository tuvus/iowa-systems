using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : BasicOrganismScript {

	public PlantSpeciesScript plantSpecies;
	public PlantSpeciesSeeds speciesSeeds;

	public PlantScript plantParent;

	public float humidityRequirement;
	public float tempetureRequirement;
	public float timeRequirement;
	public float maxTime;

    #region SeedSetup
    public void SetupSeed(float _humidity, float _tempature, float _timeRequirement, float _maxTime, PlantSpeciesSeeds plantSpeciesSeeds) {
		speciesSeeds = plantSpeciesSeeds;
		humidityRequirement = _humidity;
		tempetureRequirement = _tempature;
		timeRequirement = _timeRequirement;
		maxTime = _maxTime;
		position = GetOrganismMotor().GetModelTransform().position;
		GetEddible().postion = position;
	}

    public override void SetUpSpecificOrganism(BasicOrganismScript parent) {
		plantSpecies = species.GetComponent<PlantSpeciesScript>();
		if (parent != null) {
			plantParent = parent.GetComponent<PlantScript>();
		}
	}

	public  void OnAddSeed(object sender, System.EventArgs info) {
		GetEarthScript().OnEndFrame -= OnAddSeed;
		speciesSeeds.AddSeed(this);
    }
	#endregion

	#region SeedUpdate
	public override void RefreshOrganism() {
    }

    public override void UpdateOrganism() {
		if (age > timeRequirement) {
			if (humidityRequirement <= species.GetEarthScript().humidity && tempetureRequirement <= species.GetEarthScript().tempeture) {
				speciesSeeds.MakePlant(this, Random.Range(0.3f, 1.6f));
				KillSeed();
			}
		}
		if (age > maxTime) {
			KillSeed();
		} else {
			age += GetEarthScript().simulationDeltaTime * 0.01f;
		}
	}
    #endregion

    #region RemoveSeed
    public void KillSeed() {
		organismDead = true;
		OrganismDied();
		speciesSeeds.SeedDeath(this);
		GetEarthScript().OnEndFrame += OnDestroySeed;
	}

	internal void OnDestroySeed(object sender, System.EventArgs info) {
		GetEarthScript().OnEndFrame -= OnDestroySeed;
		DestroySeed();
	}

	void DestroySeed() {
		speciesSeeds.RemoveSeed(this);
		Destroy(gameObject);
	}

	internal override void OrganismDied() {
    }

    public override string GetFoodType() {
        return speciesSeeds.foodType;
    }
    #endregion
}
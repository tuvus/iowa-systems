using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesSeeds : BasicPlantSpeciesOrganScript {
	public GameObject seedPrefab;

	public string foodType;
	public int startingSeedCount;
	public float humidityRequirement;
	public float tempetureRequirement;
	public float seedDispertionRange;
	public float timeRequirement;
	public float timeMaximum;

	public float awnMaxGrowth;
	public int awnMaxAmount;
	public int awnMaxSeedAmount;

	internal List<Seed> seeds = new List<Seed>();

	public override void MakeOrganism(BasicOrganismScript _newOrganism) {
		SeedOrgan seeds = _newOrganism.gameObject.AddComponent<SeedOrgan>();
		seeds.speciesSeeds = this;
		seeds.SetupBasicOrgan(this);
	}
	public void MakePlant (Seed _plantToGrow, float growth) {
		PlantScript newPlant = plantSpecies.SpawnOrganismFromSeed(_plantToGrow);
	}

	public void Populate (EarthScript _earth) {
		for (int i = 0; i < startingSeedCount; i++) {
			Seed seedScript = plantSpecies.SpawnRandomSeed(seedPrefab);
			seedScript.SetupSeed(humidityRequirement * Random.Range(.6f, 1.4f), tempetureRequirement * Random.Range(.8f, 1.2f), timeRequirement * Random.Range(.5f, 2f),timeMaximum * Random.Range(.5f,2f),this);
			seedScript.age = Random.Range(0, seedScript.timeRequirement);
		}
	}

	public void SpreadSeed(PlantScript _parent) {
		Seed newSeed = plantSpecies.SpawnSeed(_parent, seedPrefab, seedDispertionRange);
		newSeed.SetupSeed(humidityRequirement * Random.Range(.6f, 1.4f), tempetureRequirement * Random.Range(.8f, 1.2f), timeRequirement * Random.Range(.5f,2f), timeMaximum * Random.Range(.5f, 2f),this);
	}

	public int GetSeedCount() {
		return seeds.Count;
    }

	public void AddSeed(Seed newSeed) {
		seeds.Add(newSeed);
		speciesScript.GetEarthScript().AddObject(newSeed, speciesScript);

	}

	public void SeedDeath(Seed deadSeed) {
		seeds.Remove(deadSeed);
    }

	public void RemoveSeed(Seed removeSeed) {
		speciesScript.GetEarthScript().RemoveObject(removeSeed, speciesScript);
	}
}
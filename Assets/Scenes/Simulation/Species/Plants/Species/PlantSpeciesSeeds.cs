using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesSeeds : BasicPlantSpeciesOrganScript {
	public GameObject seed;

	public int seedCount;
	public float humidityRequirement;
	public float tempetureRequirement;
	public float seedDispertionRange;
	public float timeRequirement;
	public float timeMaximum;

	public float awnMaxGrowth;
	public int awnMaxAmount;
	public float awnMaxSeedAmount;

	public override void MakeOrganism(GameObject _newOrganism) {
		SeedOrgan seeds = _newOrganism.AddComponent<SeedOrgan>();
		seeds.speciesSeeds = this;
		seeds.SetupBasicOrgan(this);
	}
	public void MakePlant (GameObject _plantToGrow, float growth) {
		PlantScript newPlant = plantSpecies.SpawnOrganismFromSeed(_plantToGrow);
	}

	public void Populate (EarthScript _earth) {
		for (int i = 0; i < seedCount; i++) {
			Seed seedScript = plantSpecies.SpawnRandomSeed(seed);
			seedScript.SetupSeed(humidityRequirement * Random.Range(.8f, 1.2f), tempetureRequirement * Random.Range(.8f, 1.2f), timeRequirement * Random.Range(.8f, 10),timeMaximum * Random.Range(.4f,1.2f),this);
		}
	}

	public void SpreadSeed(PlantScript _parent) {
		Seed newSeed = plantSpecies.SpawnSeed(_parent.gameObject, seed, seedDispertionRange);
		newSeed.SetupSeed(humidityRequirement * Random.Range(.8f, 1.2f), tempetureRequirement * Random.Range(.8f, 1.2f), timeRequirement * Random.Range(.8f,10), timeMaximum * Random.Range(.4f, 1.2f), this);
		newSeed.speciesSeeds = this;
	}
}
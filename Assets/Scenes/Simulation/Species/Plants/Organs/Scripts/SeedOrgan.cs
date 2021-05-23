using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedOrgan : BasicPlantOrganScript {

	public PlantSpeciesSeeds speciesSeeds;

	public List<float> awnsGrowth = new List<float>();

	private PlantScript basicPlant;

	internal override void SetUpSpecificOrgan() {
		basicPlant = GetComponent<PlantScript>();

	}

	public float Grow(float _growth, float _time) {
		float growth = _growth;
		growth = GrowAwns(growth, _time);
		return growth;
	}

	public float GrowAwns(float _growth, float _time) {
		float growth = _growth;
		float newAwnGrothCost = 0;
		if (awnsGrowth.Count < speciesSeeds.awnMaxAmount && growth > newAwnGrothCost) {
			awnsGrowth.Add(0);
			growth -= newAwnGrothCost;
		}
		float awnGrowthToAdd = _growth / awnsGrowth.Count;
		growth = 0;
		for (int i = 0; i < awnsGrowth.Count; i++) {
			awnsGrowth[i] += awnGrowthToAdd;
			if (awnsGrowth[i] > speciesSeeds.awnMaxGrowth) {
				growth += awnsGrowth[i] - speciesSeeds.awnMaxGrowth;
				awnsGrowth.RemoveAt(i);
				i--;
				SpreadNewSeed();
			}
		}
		return growth;
	}

	public void SpreadNewSeed() {
		speciesSeeds.SpreadSeed(plantScript);
	}
}

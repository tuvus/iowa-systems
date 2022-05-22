using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetativePropagationOrgan : BasicPlantOrganScript {

	public PlantSpeciesVegetativePropagation plantSpeciesVegetativePropagation;


	public float growth;
	public float growthMax;
	public float newPlantGrowthCost;
	public int newPlantGrowChance;

	internal override void SetUpSpecificOrgan() {
		plantScript = basicOrganismScript.GetComponent<PlantScript>();
		newPlantGrowthCost = newPlantGrowthCost * Random.Range(0.8f, 1.2f);
	}

	public override void SpawnOrganismAdult() {
	}

	//void FixedUpdate() {
	//	if (growth >= newPlantGrowthCost) {
	//		if (Random.Range(0, growth / growthMax) >= growth / growthMax) {
	//			growth -= newPlantGrowthCost;
	//			Reproduce();
	//		}
	//	}
	//}

	public override void ResetOrgan() {
	}

    public void Grow(float growth) {
		if (growth > growthMax) {
			growth = growthMax;
		}
		//if (plantScript.organismCount >= 2) {
		//	if (Random.Range(0, 100 / plantScript.organismCount) <= 4) {
		//		plantScript.organismCount--;
		//		plantScript.plantSpecies.SpawnSpecificOrganism(basicOrganismScript);
		//	}
		//}
	}

	public void Reproduce() {
		if (growth >= newPlantGrowthCost) {
			if (Random.Range(0, 100) <= newPlantGrowChance) {

			} else {
				//plantScript.GetPlantSpecies().organismCount++;
				//plantScript.organismCount++;
				growth -= newPlantGrowthCost;
			}
		}
	}

    public override void GrowOrgan(float growth) {
		Grow(growth);
    }
}
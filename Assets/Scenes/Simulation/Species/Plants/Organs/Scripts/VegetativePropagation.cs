using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetativePropagation : MonoBehaviour {

	private BasicPlantScript plantScript;

	public float ageRequirement;
	public float growth;
	public float growthRate;
	public float growthMax;
	public float newPlantGrowthCost;
	public int timeAfterReproduction;
	public int maxTimeAfterReproduction;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
		ageRequirement = ageRequirement * Random.Range(0.8f, 1.2f);
		growthRate = growthRate * Random.Range(0.8f, 1.2f);
		newPlantGrowthCost = newPlantGrowthCost * Random.Range(0.8f, 1.2f);
	}
	void FixedUpdate() {
		if (timeAfterReproduction > 0) {
			timeAfterReproduction--;
		}
		if (plantScript.refreshed == true) {
			if (growth >= newPlantGrowthCost) {
				for (int i = 0; i < plantScript.organismCount; i++) {
					Reproduce();
				}
			}
			if (plantScript.storedGrowth >= newPlantGrowthCost && plantScript.age >= ageRequirement && growth < growthMax && timeAfterReproduction == 0) {
				plantScript.storedGrowth -= growthRate;
				growth += growthRate;
				if (growth > growthMax) {
					growth = growthMax;
				}
			}
		}
		if (plantScript.organismCount >= 2) {
			if (Random.Range (0, 100 / plantScript.organismCount) <= 4) {
				plantScript.organismCount--;
				plantScript.plantSpecies.GetComponent<PlantSpeciesScript>().SpawnVegOrganism(gameObject);
			}
		}
	}


	public void Reproduce () {
		int plantsReproduced = 0;
		for (int i = 0; i < plantScript.organismCount; i++) {
			if (Random.Range(0,10) == 0) {
				plantsReproduced++;
				plantScript.organismCount++;
				growth -= newPlantGrowthCost;
			}
		}
		if (plantsReproduced >= 1) {
			timeAfterReproduction = maxTimeAfterReproduction * plantsReproduced;
		}
		/*if (stack) {
			if (Random.Range(0, 10) == 0) {
				plantScript.plantSpecies.GetComponent<PlantSpeciesScript>().SpawnVegOrganism(gameObject);
			} else {
				growth -= newPlantGrowthCost;
				plantScript.organismCount += 1;
				plantScript.plantSpecies.GetComponent<PlantSpeciesScript>().plantCount++;
			}
		} else {
			plantScript.plantSpecies.GetComponent<PlantSpeciesScript>().SpawnVegOrganism(gameObject);
		}*/
	}
}

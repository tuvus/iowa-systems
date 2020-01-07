using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetativePropagation : MonoBehaviour {

	private BasicPlantScript plantScript;

	public float reproductionAge;
	public float growth;
	public float growthMax;
	public float newPlantGrowthCost;
	public int timeAfterReproduction;
	public int maxTimeAfterReproduction;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
		newPlantGrowthCost = newPlantGrowthCost * Random.Range(0.8f, 1.4f);
		reproductionAge = reproductionAge * Random.Range(0.8f, 1.2f);
	}
	void FixedUpdate() {
		if (GetComponent<BasicPlantScript>().age >= reproductionAge) {
			if (timeAfterReproduction > 0) {
				timeAfterReproduction--;
			}
			if (plantScript.refreshed == true) {
				if (growth >= newPlantGrowthCost) {
					for (int i = 0; i < plantScript.organismCount; i++) {
						Reproduce();
					}
				}
				if (growth > growthMax) {
					growth = growthMax;
				}
			}
			if (plantScript.organismCount >= 2) {
				if (Random.Range(0, 100 / plantScript.organismCount) <= 2) {
					plantScript.organismCount--;
					plantScript.plantSpecies.GetComponent<PlantSpeciesScript>().SpawnVegOrganism(gameObject);
				}
			}
		}
	}

	public void Reproduce () {
		int plantsReproduced = 0;
		for (int i = 0; i < plantScript.organismCount; i++) {
			if (growth >= newPlantGrowthCost) {
				if (Random.Range(0, 40) == 0) {
					plantsReproduced++;
					plantScript.organismCount++;
					growth -= newPlantGrowthCost;
				}
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

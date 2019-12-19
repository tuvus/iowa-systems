using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flowers : MonoBehaviour {

	private BasicPlantScript plantScript;
	private PlantFoodScript plantFood;
	private Seeds seeds;

	public float flowerGrowth;
	public float flowerGrowthMax;
	public float flowerGrowthRate;
	public float flowerAgeRequiremnt;
	public float flowerGrowthRequirement;
	public float flowerGrowRequirement;
	public int flowerCount;

	public float flowerFertilityBonus;

	public float seedGrowthRequirement;
	public int flowerSeedProduction;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
		seeds = gameObject.GetComponentInParent<Seeds>();
		plantFood = GetComponent<PlantFoodScript>();
	}
	void FixedUpdate() {
		if (plantFood.intFoodCount < flowerCount) {
			flowerCount = plantFood.intFoodCount;
		}
		if ((plantScript.inSun) && (plantScript.refreshed == true)) {
			if (flowerCount != 0) {
				plantScript.fertility += flowerFertilityBonus * flowerCount;
				if (plantScript.storedGrowth >= seedGrowthRequirement) {
					for (int i = 0; i < flowerCount; i++) {
						if (Random.Range(0, 10) == 0) {
							plantScript.storedGrowth -= seedGrowthRequirement / 2;
							MakeNewSeed();
							flowerCount--;
							i--;
						}
					}
				
				}
			}
			if ((plantScript.age >= flowerAgeRequiremnt) && (plantScript.storedGrowth >= flowerGrowthRequirement) && (Random.Range (0,2) == 0)) {
				if (flowerGrowth < flowerGrowthMax) {
					plantScript.storedGrowth -= flowerGrowthRate;
					flowerGrowth += flowerGrowthRate;
					if (flowerGrowth > flowerGrowthMax) {
						flowerGrowth = flowerGrowthMax;
					}
				}
				if (flowerGrowth >= flowerGrowRequirement) {
					if (Random.Range(0, 3) == 0) {
						plantScript.storedGrowth -= flowerGrowRequirement;
						flowerCount += 1;
						plantFood.intFoodCount = flowerCount;
					}
				}
			}
		}
	}

	void MakeNewSeed () {
		if (Random.Range(0, 100) < 6) {
			//Make new seed plant
			seeds.MakeSeed(transform.parent.gameObject);
		} else {
			plantScript.organismCount += Random.Range(0, flowerSeedProduction);
		}
	}
}

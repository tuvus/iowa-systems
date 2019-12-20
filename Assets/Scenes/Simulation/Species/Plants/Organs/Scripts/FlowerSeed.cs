using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSeed : MonoBehaviour {

	private BasicPlantScript plantScript;
	private PlantFoodScript plantFood;
	private Seeds seeds;

	public float flowerGrowth;
	public float flowerGrowthMax;
	public float flowerGrowthRate;

	public float flowerAgeRequirement;

	public float flowerGrowthRequirement;
	public float seedGrowthRequirement;

	public int flowerGrowStage;
	public int stage;
	public int distributionStage;
	public int seedGrowStage;

	public int flowerCount;
	public int flowerCountMax;
	public int seedcount;

	public int flowerSeedProduction;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
		seeds = gameObject.GetComponentInParent<Seeds>();
		plantFood = GetComponent<PlantFoodScript>();
	}
	private void FixedUpdate() {
		if (plantScript.age >= flowerAgeRequirement) {
			flowerGrowth += flowerGrowthRate * plantScript.organismCount;
			flowerGrowthRate -= flowerGrowthRate / 100;
			if (flowerGrowth >= flowerGrowthMax) {
				flowerGrowth = flowerGrowthMax;
			}
			if (stage > 0) {
				//this is the flower stage
				if (flowerGrowth >= flowerAgeRequirement && flowerCount != flowerCountMax * plantScript.organismCount) {
					flowerGrowth -= flowerGrowthRequirement;
					flowerCount++;
				}
				stage--;
			} else if (stage < distributionStage) {
				//this is the seed stage
				if (flowerGrowth >= seedGrowthRequirement && seedcount != flowerCount * flowerSeedProduction) {
					seedcount += flowerSeedProduction;
				}
				stage--;
			} else if (stage < 0) {
				//this is the distributionStage
				for (int i = 0; i < seedcount; i++) {
					if (Random.Range(0,30) == 0) {
						seedcount--;
						seeds.MakeSeed(transform.parent.gameObject);
					}
				}
				stage--;
			}
			if (stage == 0) {
				stage = flowerGrowStage;
			}
			if (stage == 1) {
				stage = seedGrowStage;
			}

		}
	}

	/*void FixedUpdate() {
		if (plantFood.intFoodCount < flowerCount) {
			flowerCount = plantFood.intFoodCount;
		}
		if ((plantScript.inSun) && (plantScript.refreshed == true)) {
			if (flowerCount != 0) {
				for (int i = 0; i < flowerCount; i++) {
					if (Random.Range(0, 10) == 0) {
						MakeNewSeed();
						flowerCount--;
						i--;
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
	}*/
}

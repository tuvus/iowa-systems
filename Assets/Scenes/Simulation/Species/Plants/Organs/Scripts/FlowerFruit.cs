using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerFruit : MonoBehaviour {
	
	private BasicPlantScript plantScript;
	private PlantFoodScript plantFood;
	public PlantSpeciesFruit fruit;

	public float flowerGrowth;
	public float flowerGrowthMax;
	public float flowerGrowthRate;

	public float flowerAgeRequirement;

	public float flowerGrowthRequirement;
	public float fruitGrowthRequirement;

	public int flowerGrowStage;
	public int stage;
	public int distributionStage;
	public int fruitGrowStage;

	public int flowerCount;
	public int flowerCountMax;
	public int fruitCount;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
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
				if (flowerGrowth >= fruitGrowthRequirement && fruitCount != flowerCount) {
					fruitCount++;
				}
				stage--;
			} else if (stage < 0) {
				//this is the distributionStage
				for (int i = 0; i < fruitCount; i++) {
					if (Random.Range(0, 30) == 0) {
						fruitCount--;
						fruit.MakeFruit (transform.parent.gameObject);
					}
				}
				stage--;
			}
			if (stage == 0) {
				stage = flowerGrowStage;
			}
			if (stage == 1) {
				stage = fruitGrowStage;
			}

		}
	}
	/*void FixedUpdate() {
		if (plantFood.intFoodCount < flowerCount) {
			flowerCount = plantFood.intFoodCount;
		}
		if ((plantScript.refreshed == true) && (plantScript.inSun)) {
			plantScript.fertility += flowerFertilityBonus * flowerCount;

			if ((plantScript.storedGrowth >= fruitGrowthRequirement) && (flowerCount != 0)) {
				if (Random.Range(0, 100 / flowerCount) == 0) {
					plantScript.storedGrowth -= fruitGrowthRequirement;
					MakeNewFruit();
				}
			}
			if (plantScript.storedGrowth >= flowerGrowthRequirement) {
				if (Random.Range(0, 10) == 0) {
					plantScript.storedGrowth -= flowerGrowthRequirement;
					flowerCount++;
				}
			}
		}
	}

	void MakeNewFruit() {
		flowerCount--;
		//Make new seed plant
		fruit = gameObject.GetComponentInParent<Fruit>();
		fruit.growTimeMax = fruitGrowTimeMax;
		fruit.parent = transform.parent.gameObject;
		fruit.MakeNewPlant();

	}*/

}

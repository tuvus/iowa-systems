using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerFruit : MonoBehaviour {
	
	private BasicPlantScript plantScript;
	private PlantFoodScript plantFood;
	private Fruit fruit;

	public float flowerGrowthRequirement;
	public float flowerGrowthRate;
	public int flowerCount;
	public int flowerCountMax;

	public float flowerFertilityBonus;

	public float fruitGrowthRequirement;
	public float fruitGrowCost;
	public int fruitGrowTimeMax;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
	}
	void FixedUpdate() {
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

	}

}

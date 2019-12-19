using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesFruit : MonoBehaviour {

	public GameObject flower;
	public GameObject fruit;

	public float flowerGrowthRequirement;
	public float flowerGrowthRate;
	public int flowerCountMax;

	public float flowerFertilityBonus;

	public float fruitGrowthRequirement;
	public float fruitGrowCost;
	public int fruitGrowTimeMax;

	public string foodType;
	public float foodCount;
	public float foodGain;
	public float eatNoiseRange;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newFlower = Instantiate(flower, _newOrganism.transform);
		FlowerFruit flowerFruit = newFlower.GetComponent<FlowerFruit>();
		flowerFruit.flowerGrowthRequirement = flowerGrowthRequirement;
		flowerFruit.flowerGrowthRate = flowerGrowthRate;
		flowerFruit.flowerCountMax = flowerCountMax;
		flowerFruit.flowerFertilityBonus = flowerFertilityBonus;
		flowerFruit.fruitGrowthRequirement = fruitGrowthRequirement;
		flowerFruit.fruitGrowCost = fruitGrowCost;
		flowerFruit.fruitGrowTimeMax = fruitGrowTimeMax;

		PlantFoodScript plantFood = newFlower.AddComponent<PlantFoodScript>();
		plantFood.foodType = foodType;
		plantFood.floatFoodCount = foodCount;
		plantFood.foodGain = foodGain;
		plantFood.eatNoiseRange = eatNoiseRange;
	}
	public void makeFlower () {
		
	}
}

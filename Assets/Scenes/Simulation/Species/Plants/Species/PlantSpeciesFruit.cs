using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesFruit : MonoBehaviour {

	public GameObject flower;
	public GameObject fruit;

	public float flowerGrowthMax;
	public float flowerGrowthRate;

	public float flowerAgeRequirement;

	public float flowerGrowthRequirement;
	public float fruitGrowthRequirement;

	public int flowerGrowStage;
	public int distributionStage;
	public int fruitGrowStage;

	public int flowerCountMax;

	public string foodType;
	public float foodCount;
	public float foodGain;
	public float eatNoiseRange;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newFlower = Instantiate(flower, _newOrganism.transform);
/*		FlowerFruit flowerFruit = newFlower.GetComponent<FlowerFruit>();
		flowerFruit.fruit = this;
		flowerFruit.flowerCountMax = flowerCountMax;
		flowerFruit.flowerGrowthRate = flowerGrowthRate;

		flowerFruit.flowerAgeRequirement = flowerAgeRequirement;

		flowerFruit.flowerGrowthRequirement = flowerGrowthRequirement;
		flowerFruit.fruitGrowthRequirement = fruitGrowthRequirement;

		flowerFruit.flowerGrowStage = flowerGrowStage;
		flowerFruit.distributionStage = distributionStage;
		flowerFruit.fruitGrowStage = fruitGrowStage;

		flowerFruit.flowerCountMax = flowerCountMax;*/

		PlantFoodScript plantFood = newFlower.AddComponent<PlantFoodScript>();
		plantFood.foodType = foodType;
		plantFood.foodGain = foodGain;
		plantFood.eatNoiseRange = eatNoiseRange;
	}
	public void MakeNewGrownOrganism(GameObject _newOrganism) {

	}

	public void MakeFruit (GameObject _parent) {
		
	}
}

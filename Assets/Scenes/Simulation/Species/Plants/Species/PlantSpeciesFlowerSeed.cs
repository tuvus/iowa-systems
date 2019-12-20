using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesFlowerSeed : MonoBehaviour {

	public GameObject flower;

	public float flowerGrowthMax;
	public float flowerGrowthRate;

	public float flowerAgeRequirement;

	public float flowerGrowthRequirement;
	public float seedGrowthRequirement;

	public int flowerGrowStage;
	public int distributionStage;
	public int seedGrowStage;

	public int flowerCountMax;

	public int flowerSeedProduction;

	public string foodType;
	public float foodGain;
	public float eatNoiseRange;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newFlower = Instantiate(flower, _newOrganism.transform);
		FlowerSeed flowers = newFlower.GetComponent<FlowerSeed>();
		flowers.flowerGrowthMax = flowerGrowthMax;
		flowers.flowerGrowthRate = flowerGrowthRate;

		flowers.flowerAgeRequirement = flowerAgeRequirement;

		flowers.flowerGrowthRequirement = flowerGrowthRequirement;
		flowers.seedGrowthRequirement = seedGrowthRequirement;

		flowers.flowerGrowStage = flowerGrowStage;
		flowers.distributionStage = distributionStage;
		flowers.seedGrowStage = seedGrowStage;
		flowers.flowerCountMax = flowerCountMax;

		flowers.flowerSeedProduction = flowerSeedProduction;

		PlantFoodScript plantFood = newFlower.AddComponent<PlantFoodScript>();
		plantFood.foodType = foodType;
		plantFood.foodGain = foodGain;
		plantFood.eatNoiseRange = eatNoiseRange;
	}
	public void MakeNewGrownOrganism(GameObject _newOrganism) {

	}

}

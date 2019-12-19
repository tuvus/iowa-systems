using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesFlowers : MonoBehaviour {

	public GameObject flower;

	public float flowerAgeRequiremnt;
	public float flowerGrowthRequirement;
	public float flowerGrowthRate;
	public float flowerGrowthMax;
	public float flowerGrowRequirement;

	public float flowerFertilityBonus;

	public float seedGrowthRequirement;
	public int flowerSeedProduction;

	public string foodType;
	public float foodGain;
	public float eatNoiseRange;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newFlower = Instantiate(flower, _newOrganism.transform);
		Flowers flowers = newFlower.GetComponent<Flowers>();
		flowers.flowerGrowthMax = flowerGrowthMax;
		flowers.flowerGrowthRate = flowerGrowthRate;
		flowers.flowerAgeRequiremnt = flowerAgeRequiremnt;
		flowers.flowerGrowthRequirement = flowerGrowthRequirement;
		flowers.flowerGrowRequirement = flowerGrowRequirement;

		flowers.flowerFertilityBonus = flowerFertilityBonus;

		flowers.seedGrowthRequirement = seedGrowthRequirement;
		flowers.flowerSeedProduction = flowerSeedProduction;

		PlantFoodScript plantFood = newFlower.AddComponent<PlantFoodScript>();
		plantFood.foodType = foodType;
		plantFood.foodGain = foodGain;
		plantFood.eatNoiseRange = eatNoiseRange;
	}
}

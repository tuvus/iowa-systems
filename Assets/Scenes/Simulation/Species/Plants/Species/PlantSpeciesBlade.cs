using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesBlade : MonoBehaviour {

	public GameObject blade;

	public float bladeAgeRequirement;
	public float bladeGrowthRate;
	public float bladeGrowthMax;

	public float bladeHealthBonus;

	public string foodType;
	public float foodGain;
	public float eatNoiseRange;

	public void makeOrganism (GameObject _newOrganism) {
		GameObject newBlade = Instantiate(blade, _newOrganism.transform);
		Blade bladeScript = newBlade.GetComponent<Blade>();
		bladeScript.bladeAgeRequirement = bladeAgeRequirement;
		bladeScript.bladeGrowthRate = bladeGrowthRate;
		bladeScript.bladeGrowthMax = bladeGrowthMax;

		bladeScript.bladeHealthBonus = bladeHealthBonus;

		PlantFoodScript plantFood = newBlade.AddComponent<PlantFoodScript>();
		plantFood.foodType = foodType;
		plantFood.foodGain = foodGain;
		plantFood.eatNoiseRange = eatNoiseRange;
	}
	public void MakeNewGrownOrganism (GameObject _newOrganism) {
		GameObject newBlade = Instantiate(blade, _newOrganism.transform);
		Blade bladeScript = newBlade.GetComponent<Blade>();
		bladeScript.bladeAgeRequirement = bladeAgeRequirement;
		bladeScript.bladeGrowthRate = bladeGrowthRate;
		bladeScript.bladeGrowthMax = bladeGrowthMax;

		bladeScript.bladeHealthBonus = bladeHealthBonus;

		PlantFoodScript plantFood = newBlade.AddComponent<PlantFoodScript>();
		plantFood.foodType = foodType;
		plantFood.foodGain = foodGain;
		plantFood.eatNoiseRange = eatNoiseRange;
	}

}

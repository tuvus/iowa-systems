using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour {

	private BasicPlantScript plantScript;
	private PlantFoodScript plantFood;

	public float bladeAgeRequirement;
	public float bladeGrowthRate;
	public float bladeGrowthMax;
	public float bladeGrowth;

	public float bladeFertilityBonus;
	public float bladeFertilityCost;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
		plantFood = GetComponent<PlantFoodScript>();
		bladeGrowth = 0;
		bladeAgeRequirement = bladeAgeRequirement * Random.Range(0.8f, 1.2f);
		bladeGrowthRate = bladeGrowthRate * Random.Range(0.8f, 1.2f);
		bladeFertilityBonus = bladeFertilityBonus * Random.Range(0.8f, 1.2f);
		bladeFertilityCost = bladeFertilityCost * Random.Range(0.8f, 1.2f);
	}
	void FixedUpdate() {
		//bladeGrowth = plantFood.floatFoodCount;
		if (plantScript.refreshed == true) {
			if (plantScript.age >= bladeAgeRequirement && plantScript.storedGrowth >= bladeGrowthRate && bladeGrowth < bladeGrowthMax) {
				Grow();
			}
			if (bladeGrowth > 0) {
				if (plantScript.inSun) {
					plantScript.fertility += ((bladeFertilityBonus - bladeFertilityCost) * (bladeGrowth / bladeGrowthMax)) * plantScript.organismCount;
				} else {
					plantScript.fertility += -bladeFertilityCost * bladeGrowth / bladeGrowthMax * plantScript.organismCount * (bladeGrowth / bladeGrowthMax);
				}
			}
		}
	}
	void Grow() {
		plantScript.storedGrowth -= bladeGrowthRate;
		bladeGrowth += bladeGrowthRate;
		bladeGrowthRate -= bladeGrowthRate / 1000;
		if (bladeGrowth > bladeGrowthMax) {
			bladeGrowth = bladeGrowthMax;
		}
		plantFood.floatFoodCount = bladeGrowth;
	}

}

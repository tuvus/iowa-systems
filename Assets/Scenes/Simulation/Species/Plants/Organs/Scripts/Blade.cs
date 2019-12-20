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

	public float bladeHealthBonus;

	void Start() {
		plantScript = GetComponentInParent<BasicPlantScript>();
		plantFood = GetComponent<PlantFoodScript>();
		bladeGrowth = 0;
		bladeAgeRequirement = bladeAgeRequirement * Random.Range(0.8f, 1.2f);
		bladeGrowthRate = bladeGrowthRate * Random.Range(0.8f, 1.2f);
		bladeHealthBonus = bladeHealthBonus * Random.Range(0.8f, 1.2f);
	}
	void FixedUpdate() {
		//bladeGrowth = plantFood.floatFoodCount;
		if (plantScript.refreshed == true) {
			if (plantScript.age >= bladeAgeRequirement && bladeGrowth < bladeGrowthMax) {
				Grow();
			}
			if (bladeGrowth > 0) {
				if (plantScript.inSun) {
					if (plantScript.health <= plantScript.maxHealth) {
						plantScript.health += bladeHealthBonus * (bladeGrowth / bladeGrowthMax);
					}
				}
			}
		}
		if (bladeGrowth >= bladeGrowthMax) {
			if (transform.parent.GetComponentInChildren<FlowerSeed>() != null && transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowth < transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowthMax) {
				transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowth += bladeGrowthRate;
				bladeGrowthRate -= bladeGrowthRate / 1000;
			} else if (transform.parent.GetComponentInChildren<FlowerFruit>() != null && transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowth < transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowthMax) {
				transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowth += bladeGrowthRate;
				bladeGrowthRate -= bladeGrowthRate / 1000;
			} else if (GetComponentInParent<VegetativePropagation>() != null&& transform.parent.GetComponentInChildren<VegetativePropagation>().growth < transform.parent.GetComponentInChildren<VegetativePropagation>().growthMax) {
				GetComponentInParent<VegetativePropagation>().growth += bladeGrowthRate;
				bladeGrowthRate -= bladeGrowthRate / 1000;
			} 
		}
	}
	void Grow() {
		bladeGrowth += bladeGrowthRate;
		bladeGrowthRate -= bladeGrowthRate / 1000;
		if (bladeGrowth > bladeGrowthMax) {
			bladeGrowth = bladeGrowthMax;
		}
		plantFood.floatFoodCount = bladeGrowth;
	}

}

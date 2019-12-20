using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaves : MonoBehaviour {

	private BasicPlantScript plantScript;
	private PlantFoodScript plantFood;

	public float leafGrowth;
	public float leafAgeRequiremnt;
	public float leafGrowthRequirement;
	public float leafGrowthRate;
	public float leafGrowthMax;

	public float leafGrowCost;
	public int leafCount;
	public int leafCountMax;

	public float leafHealthBonus;


	void Start () {
		plantScript = GetComponentInParent<BasicPlantScript>();
		plantFood = GetComponent<PlantFoodScript>();

		leafAgeRequiremnt = leafAgeRequiremnt * Random.Range(0.8f, 1.2f);
		leafGrowthRate = leafGrowthRate * Random.Range(0.8f, 1.2f);
		leafHealthBonus = leafHealthBonus * Random.Range(0.8f, 1.2f);
		leafCountMax = Mathf.RoundToInt (leafCountMax * Random.Range(0.8f, 1.2f));
	}
	void FixedUpdate() {
		if (plantFood.intFoodCount < leafCount) {
			leafCount = plantFood.intFoodCount;
		}
		if (leafGrowth < 0) {
			StabalizeGrowth();
		}
		if (plantScript.refreshed) {
			if (plantScript.inSun) {

			}
			if ((plantScript.age >= leafAgeRequiremnt) && (leafCount != leafCountMax)) {
				leafGrowth += leafGrowthRate / 2;
				leafGrowthRate -= leafGrowthRate / 1000;
				if (leafGrowth > leafGrowthMax) {
					leafGrowth = leafGrowthMax;
				}
					
			}
			if ((leafGrowth >= leafGrowCost) && (leafCount != leafCountMax)) {
				if (Random.Range(0,3) == 0) {
					leafCount += 1;
					leafGrowth -= leafGrowCost;
					plantFood.intFoodCount = leafCount;
				}
			}
			if (leafGrowth >= leafGrowthMax) {
				if (transform.parent.GetComponentInChildren<FlowerSeed>() != null && transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowth < transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowthMax) {
					transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowth += leafGrowthRate;
					leafGrowthRate -= leafGrowthRate / 1000;
				} else if (transform.parent.GetComponentInChildren<FlowerFruit>() != null && transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowth < transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowthMax) {
					transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowth += leafGrowthRate;
					leafGrowthRate -= leafGrowthRate / 1000;
				} else if (GetComponentInParent<VegetativePropagation>() != null && transform.parent.GetComponentInChildren<VegetativePropagation>().growth < transform.parent.GetComponentInChildren<VegetativePropagation>().growthMax) {
					GetComponentInParent<VegetativePropagation>().growth += leafGrowthRate;
					leafGrowthRate -= leafGrowthRate / 1000;
				}
			}
			if (plantScript.health < plantScript.maxHealth) {
				plantScript.health += leafHealthBonus * leafCount;
			}
		}
	}
	private void StabalizeGrowth() {
		leafGrowth += leafGrowCost;
		leafCount -= 1;
		if (leafGrowth < 0) {
			StabalizeGrowth();
		}
	}

}
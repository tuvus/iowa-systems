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

	public float leafFertilityBonus;


	void Start () {
		plantScript = GetComponentInParent<BasicPlantScript>();
		plantFood = GetComponent<PlantFoodScript>();

		leafAgeRequiremnt = leafAgeRequiremnt * Random.Range(0.8f, 1.2f);
		leafGrowthRate = leafGrowthRate * Random.Range(0.8f, 1.2f);
		leafFertilityBonus = leafFertilityBonus * Random.Range(0.8f, 1.2f);
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
				if (leafCount != 0) {
					plantScript.fertility += leafFertilityBonus * leafCount;
				}
			}
			if ((plantScript.age >= leafAgeRequiremnt) && (plantScript.storedGrowth >= leafGrowthRequirement) && (leafCount != leafCountMax)) {
				plantScript.storedGrowth -= leafGrowthRate;
				leafGrowth += leafGrowthRate / 2;
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
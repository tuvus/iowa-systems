using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesLeaves : MonoBehaviour {

	public GameObject leaves;
	public float leafAgeRequiremnt;
	public float leafGrowthRequirement;
	public float leafGrowthRate;
	public float leafGrowthMax;

	public float leafGrowCost;
	public int leafCountMax;

	public float leafFertilityBonus;

	public string foodType;
	public float foodGain;
	public float eatNoiseRange;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newLeaf = Instantiate(leaves, _newOrganism.transform);
		Leaves leaf = newLeaf.GetComponent<Leaves>();

		leaf.leafGrowthMax = leafGrowthMax;
		leaf.leafAgeRequiremnt = leafAgeRequiremnt;
		leaf.leafGrowthRequirement = leafGrowthRequirement;
		leaf.leafGrowthRate = leafGrowthRate;

		leaf.leafGrowCost = leafGrowCost;
		leaf.leafCountMax = leafCountMax;

		leaf.leafFertilityBonus = leafFertilityBonus;

		PlantFoodScript plantFood = newLeaf.AddComponent<PlantFoodScript>();
		plantFood.foodType = foodType;
		plantFood.foodGain = foodGain;
		plantFood.eatNoiseRange = eatNoiseRange;
	}
}

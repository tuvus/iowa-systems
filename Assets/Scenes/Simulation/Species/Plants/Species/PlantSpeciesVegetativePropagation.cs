using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesVegetativePropagation : MonoBehaviour {

	public float ageRequirement;
	public float growthRate;
	public float growthMax;
	public float newPlantGrowthCost;
	public int maxTimeAfterReproduction;

	public void makeOrganism(GameObject _newOrganism) {
		VegetativePropagation propagation = _newOrganism.AddComponent<VegetativePropagation>();
		propagation.ageRequirement = ageRequirement;
		propagation.growthRate = growthRate;
		propagation.growthMax = growthMax;
		propagation.newPlantGrowthCost = newPlantGrowthCost;
		propagation.maxTimeAfterReproduction = maxTimeAfterReproduction;
	}
}

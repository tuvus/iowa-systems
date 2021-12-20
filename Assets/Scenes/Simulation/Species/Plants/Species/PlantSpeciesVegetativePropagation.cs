using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesVegetativePropagation : BasicPlantSpeciesOrganScript {

	public float growthMax;
	public float newPlantGrowthCost;
	public int newPlantGrowChance;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		return;
		//VegetativePropagationOrgan propagation = newOrganism.AddComponent<VegetativePropagationOrgan>();
		//propagation.SetupBasicOrgan(this);
		//propagation.plantSpeciesVegetativePropagation = this;
		//propagation.growthMax = growthMax;
		//propagation.newPlantGrowthCost = newPlantGrowthCost;
		//propagation.newPlantGrowChance = newPlantGrowChance;
	}
}

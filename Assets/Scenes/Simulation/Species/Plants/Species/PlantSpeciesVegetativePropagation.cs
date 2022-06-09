using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesVegetativePropagation : PlantSpeciesOrgan {

	public float growthMax;
	public float newPlantGrowthCost;
	public int newPlantGrowChance;

    public override float GetGrowthRequirementForStage(Plant.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        throw new System.NotImplementedException();
    }

    public override void MakeOrganism(Plant plant) {
		VegetativePropagationOrgan propagation = plant.gameObject.AddComponent<VegetativePropagationOrgan>();
		propagation.SetupOrgan(this, plant);
	}
}

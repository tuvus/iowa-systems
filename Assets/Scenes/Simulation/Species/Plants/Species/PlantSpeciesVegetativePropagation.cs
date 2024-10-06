using System;
using Plant = PlantSpecies.Plant;

public class PlantSpeciesVegetativePropagation : PlantSpeciesOrgan {

	public float growthMax;
	public float newPlantGrowthCost;
	public int newPlantGrowChance;

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        throw new NotImplementedException();
    }

    public override void GrowOrgan(Species.Organism organismR, Plant plantR, Plant plantW, float growth) {    }
}

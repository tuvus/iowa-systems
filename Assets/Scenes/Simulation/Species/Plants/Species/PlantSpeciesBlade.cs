using Plant = PlantSpecies.Plant;

public class PlantSpeciesBlade : EddiblePlantSpeciesOrgan {

    public override void GrowOrgan(Species.Organism organismR, Plant plantR, Plant plantW, float growth) {
        plantW.bladeArea = plantR.bladeArea + growth * growthModifier;
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantSpecies.GrowthStage.Adult)
            return 0;
        return (thisStageValues.bladeArea - previousStageValues.bladeArea) / growthModifier;
    }
}
using Plant = PlantSpecies.Plant;
public class PlantSpeciesStem : EddiblePlantSpeciesOrgan {

    public override void GrowOrgan(Species.Organism organismR, Plant plantR, Plant plantW, float growth) {
        plantW.stemHeight = plantR.stemHeight + growth * growthModifier;
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantSpecies.GrowthStage.Adult)
            return 0;
        return (thisStageValues.stemHeight - previousStageValues.stemHeight) / growthModifier;
    }
}
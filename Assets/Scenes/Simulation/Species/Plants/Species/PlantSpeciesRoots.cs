using Plant = PlantSpecies.Plant;

public class PlantSpeciesRoots : PlantSpeciesOrgan {
    public float rootDensity;

    public override string GetOrganType() {
        return organType;
    }

    public override void GrowOrgan(Species.Organism organismR, Plant plantR, Plant plantW, float growth) {
        plantW.rootGrowth.y = plantR.rootGrowth.y + growth * growthModifier;
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantSpecies.GrowthStage.Adult)
            return 0;
        return (thisStageValues.rootGrowth.y - previousStageValues.rootGrowth.y) / growthModifier;
    }
}

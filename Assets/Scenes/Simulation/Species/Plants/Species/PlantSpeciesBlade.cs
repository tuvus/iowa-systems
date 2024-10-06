using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantSpeciesBlade : EddiblePlantSpeciesOrgan {

    public override void GrowOrgan(PlantSpecies.Plant plant, float growth) {
        plant.bladeArea += (growth * growthModifier);
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantSpecies.GrowthStage.Adult)
            return 0;
        return (thisStageValues.bladeArea - previousStageValues.bladeArea) / growthModifier;
    }
}
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantSpeciesStem : EddiblePlantSpeciesOrgan {

    public override void GrowOrgan(int organism, float growth, ref float bladeArea, ref float stemHeight, ref float2 rootGrowth) {
        stemHeight = stemHeight + (growth * growthModifier);
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantSpecies.GrowthStage.Adult)
            return 0;
        return (thisStageValues.stemHeight - previousStageValues.stemHeight) / growthModifier;
    }
}
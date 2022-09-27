using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantSpeciesRoots : PlantSpeciesOrgan {
    public float rootDensity;

    public override string GetOrganType() {
        return organType;
    }

    public override void GrowOrgan(int organism, float growth, ref float bladeArea, ref float stemHeight, ref float2 rootGrowth) {
        rootGrowth = new float2(rootGrowth.x, rootGrowth.y + (growth * growthModifier));
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantSpecies.GrowthStage.Adult)
            return 0;
        return (thisStageValues.rootGrowth.y - previousStageValues.rootGrowth.y) / growthModifier;
    }
}

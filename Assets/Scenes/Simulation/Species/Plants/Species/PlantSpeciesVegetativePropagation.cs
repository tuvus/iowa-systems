using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantSpeciesVegetativePropagation : PlantSpeciesOrgan {

	public float growthMax;
	public float newPlantGrowthCost;
	public int newPlantGrowChance;

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        throw new NotImplementedException();
    }

    public override void GrowOrgan(int organism, float growth, ref float bladeArea, ref float stemHeight, ref float2 rootGrowth) {
        throw new NotImplementedException();
    }
}

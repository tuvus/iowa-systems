﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesVegetativePropagation : PlantSpeciesOrgan {

	public float growthMax;
	public float newPlantGrowthCost;
	public int newPlantGrowChance;

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        throw new NotImplementedException();
    }
}

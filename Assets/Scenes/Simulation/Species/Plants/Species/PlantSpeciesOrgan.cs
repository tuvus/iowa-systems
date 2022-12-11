using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public abstract class PlantSpeciesOrgan : SpeciesOrgan {
    public string organType;
    public int organFoodIndex;
    [Tooltip("The relationship between how much growth input is require to grow this organ")]
    public float growthModifier;

    public NativeArray<float> growthPriorities;

    public abstract float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues);

    public void SetupSpeciesOrganFoodType() {
        organFoodIndex = GetPlantSpecies().GetEarth().GetIndexOfFoodType(GetOrganType());
    }

    public virtual string GetOrganType() {
        return null;
    }

    public abstract void GrowOrgan(int organism, float growth, ref float bladeArea, ref float stemHeight, ref float2 rootGrowth);

    public int GetOrganFoodIndex() {
        return organFoodIndex;
    }

    public override void Deallocate() {
            growthPriorities.Dispose();
    }

    public PlantSpecies GetPlantSpecies() {
        return (PlantSpecies)GetSpecies();
    }
}

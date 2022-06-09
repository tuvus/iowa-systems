using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class PlantSpeciesOrgan : SpeciesOrgan {
    public string organType;
    public int organFoodIndex;
    [Tooltip("The relationship between how much growth input is require to grow this organ")]
    public float growthModifier;

    public NativeArray<float> growthPriorities;

    public abstract void MakeOrganism(Plant plant);

    public abstract float GetGrowthRequirementForStage(Plant.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues);

    public void SetupSpeciesOrganFoodType() {
        organFoodIndex = GetPlantSpecies().GetEarth().GetIndexOfFoodType(GetOrganType());
    }

    public virtual string GetOrganType() {
        return null;
    }

    public int GetOrganFoodIndex() {
        return organFoodIndex;
    }

    public void OnDestroy() {
        if(growthPriorities.IsCreated)
            growthPriorities.Dispose();
    }

    public PlantSpecies GetPlantSpecies() {
        return (PlantSpecies)GetSpecies();
    }
}

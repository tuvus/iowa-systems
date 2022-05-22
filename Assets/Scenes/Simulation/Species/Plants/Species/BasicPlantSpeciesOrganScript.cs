using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class BasicPlantSpeciesOrganScript : BasicSpeciesOrganScript {
    internal PlantSpecies plantSpecies;
    public string organType;
    public int organFoodIndex;
    [Tooltip("The relationship between how much growth input is require to grow this organ")]
    public float growthModifier;

    public NativeArray<float> growthPriorities;

    public override void SetSpeciesScript(BasicSpeciesScript _species) {
        plantSpecies = (PlantSpecies)_species;
    }

    public abstract float GetGrowthRequirementForStage(PlantScript.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues);

    public void SetupSpeciesOrganFoodType() {
        organFoodIndex = plantSpecies.GetEarthScript().GetIndexOfFoodType(GetOrganType());
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
}

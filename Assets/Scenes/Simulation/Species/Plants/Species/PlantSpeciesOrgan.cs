using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public abstract class PlantSpeciesOrgan : SpeciesOrgan {
    public string organType;
    [Tooltip("The relationship between how much growth input is require to grow this organ")]
    public float growthModifier;

    public float[] growthPriorities;

    public abstract float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues);

    public virtual string GetOrganType() {
        return null;
    }

    public abstract void GrowOrgan(PlantSpecies.Plant plant, float growth);
    

    public PlantSpecies GetPlantSpecies() {
        return (PlantSpecies)GetSpecies();
    }
}

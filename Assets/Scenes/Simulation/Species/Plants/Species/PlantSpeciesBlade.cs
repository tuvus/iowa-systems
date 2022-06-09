using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesBlade : EddiblePlantSpeciesOrganScript {
    public override void MakeOrganism(Plant plant) {
        BladeOrgan bladeOrgan = plant.gameObject.AddComponent<BladeOrgan>();
        bladeOrgan.SetupOrgan(this, plant);
        MakeEddibleOrganism(bladeOrgan, plant);
    }

    public override float GetGrowthRequirementForStage(Plant.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == Plant.GrowthStage.Adult)
            return 0;
        return (thisStageValues.bladeArea - previousStageValues.bladeArea) / growthModifier;
    }
}
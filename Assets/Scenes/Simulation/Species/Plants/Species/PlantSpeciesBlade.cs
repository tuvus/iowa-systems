using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesBlade : EddiblePlantSpeciesOrganScript {
    public override void MakeOrganism(BasicOrganismScript newOrganism) {
        BladeOrgan bladeOrgan = newOrganism.gameObject.AddComponent<BladeOrgan>();
        bladeOrgan.SetupBasicOrgan(this, newOrganism);
        bladeOrgan.speciesBlade = this;
        MakeEddibleOrganism(bladeOrgan,newOrganism);
    }

    public override float GetGrowthRequirementForStage(PlantScript.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantScript.GrowthStage.Adult)
            return 0;
        return (thisStageValues.bladeArea - previousStageValues.bladeArea) / growthModifier;
    }
}
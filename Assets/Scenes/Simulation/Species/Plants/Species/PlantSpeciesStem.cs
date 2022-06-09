using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesStem : EddiblePlantSpeciesOrganScript {

    public override void MakeOrganism(Plant plant) {
        StemOrgan stemOrgan = plant.gameObject.AddComponent<StemOrgan>();
        stemOrgan.SetupOrgan(this, plant);
        MakeEddibleOrganism(stemOrgan, plant);
    }

    public override float GetGrowthRequirementForStage(Plant.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == Plant.GrowthStage.Adult)
            return 0;
        return (thisStageValues.stemHeight - previousStageValues.stemHeight) / growthModifier;
    }
}

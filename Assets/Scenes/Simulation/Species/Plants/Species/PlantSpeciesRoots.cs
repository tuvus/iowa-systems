using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantSpeciesRoots : BasicPlantSpeciesOrganScript {
	public float rootDensity;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		RootOrgan rootOrgan = newOrganism.gameObject.AddComponent<RootOrgan>();
		rootOrgan.SetupBasicOrgan(this, newOrganism);
		rootOrgan.speciesRoots = this;
	}

	public override string GetOrganType() {
		return organType;
	}

    public override float GetGrowthRequirementForStage(PlantScript.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
		if (stage == PlantScript.GrowthStage.Adult)
			return 0;
		return (thisStageValues.rootGrowth.y - previousStageValues.rootGrowth.y) / growthModifier;
    }
}

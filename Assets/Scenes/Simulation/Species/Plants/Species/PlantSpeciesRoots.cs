using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantSpeciesRoots : PlantSpeciesOrgan {
	public float rootDensity;

	public override void MakeOrganism(Plant plant) {
		RootOrgan rootOrgan = plant.gameObject.AddComponent<RootOrgan>();
		rootOrgan.SetupOrgan(this, plant);
	}

	public override string GetOrganType() {
		return organType;
	}

    public override float GetGrowthRequirementForStage(Plant.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
		if (stage == Plant.GrowthStage.Adult)
			return 0;
		return (thisStageValues.rootGrowth.y - previousStageValues.rootGrowth.y) / growthModifier;
    }
}

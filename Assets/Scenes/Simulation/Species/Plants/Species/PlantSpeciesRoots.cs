using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlantSpeciesRoots : BasicPlantSpeciesOrganScript {

	public float2 rootGrowthMax;
	public float rootDensity;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		RootOrgan rootOrgan = newOrganism.gameObject.AddComponent<RootOrgan>();
		rootOrgan.SetupBasicOrgan(this, newOrganism);
		rootOrgan.speciesRoots = this;
		rootOrgan.rootGrowth.x = rootGrowthMax.x;
	}

	public override string GetOrganType() {
		return organType;
	}
}

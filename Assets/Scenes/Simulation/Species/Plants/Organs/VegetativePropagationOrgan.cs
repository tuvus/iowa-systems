using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetativePropagationOrgan : PlantOrgan {
	public float growth;
	public int newPlantGrowChance;

	public void SetupOrgan(PlantSpeciesOrgan plantSpeciesOrgan, Plant plant) {
		base.SetupOrgan(plantSpeciesOrgan, plant);

	}

	public override void SpawnOrganismAdult() {
	}

	public override void ResetOrgan() {
	}

    public void Grow(float growth) {
	}

	public void Reproduce() {
	}

    public override void GrowOrgan(float growth) {
		Grow(growth);
    }

	public PlantSpeciesVegetativePropagation GetPlantSpeciesVegetativePropagation() {
		return (PlantSpeciesVegetativePropagation)GetSpeciesOrgan();
    }
}
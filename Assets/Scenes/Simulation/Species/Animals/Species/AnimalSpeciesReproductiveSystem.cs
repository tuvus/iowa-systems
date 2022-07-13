using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesReproductiveSystem : AnimalSpeciesOrgan {
	public GameObject reproductiveSystemPrefab;

	[Tooltip("The time to birth after conception in hours")]
	public float birthTime;
	public float reproductionDelay;
	public float reproductionAge;
	public int reproducionAmount;
	public int birthSuccessPercent;

	public override void MakeOrganism(Animal animal) {
		ReproductiveSystemOrgan reproductiveSystemScript = GetAnimalSpecies().InstantiateNewOrgan(reproductiveSystemPrefab, animal).GetComponent<ReproductiveSystemOrgan>();
		reproductiveSystemScript.SetupOrgan(this, animal);
	}

	public void MakeChildOrganism (int amount, Animal parent) {
		for (int i = 0; i < amount; i++) {
			GetAnimalSpecies().SpawnOrganism(parent);
		}
	}
}
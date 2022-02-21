using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesReproductiveSystem : BasicAnimalSpeciesOrganScript {

	public GameObject reproductiveSystemPrefab;

	public float birthTime;
	public float reproductionDelay;
	public float reproductionAge;
	public int reproducionAmount;
	public float birthSuccessPercent;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		GameObject newReproductiveSystem = speciesScript.InstantiateNewOrgan(reproductiveSystemPrefab, newOrganism);
		ReproductiveSystem reproductiveSystemScript = newReproductiveSystem.GetComponent<ReproductiveSystem>();
		reproductiveSystemScript.animalSpeciesReproductive = this;
		reproductiveSystemScript.SetupBasicOrgan(this,newOrganism);
	}

	public void MakeChildOrganism (int amount, AnimalScript parent) {
		for (int i = 0; i < amount; i++) {
			animalSpecies.SpawnOrganism(parent);
		}
	}
}
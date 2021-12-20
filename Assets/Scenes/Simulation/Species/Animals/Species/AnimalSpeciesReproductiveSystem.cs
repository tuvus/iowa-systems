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

	public override void MakeOrganism(BasicOrganismScript _newOrganism) {
		GameObject newReproductiveSystem = speciesScript.InstantiateNewOrgan(reproductiveSystemPrefab, _newOrganism);
		ReproductiveSystem reproductiveSystemScript = newReproductiveSystem.GetComponent<ReproductiveSystem>();
		reproductiveSystemScript.animalSpeciesReproductive = this;
		reproductiveSystemScript.SetupBasicOrgan(this);
		if (Random.Range(0,2) == 0) {
			FemaleReproductiveSystem femaleReproductive = newReproductiveSystem.AddComponent<FemaleReproductiveSystem>();
			femaleReproductive.reproductive = reproductiveSystemScript;
			reproductiveSystemScript.femaleReproductiveSystem = femaleReproductive;
			femaleReproductive.SetupBasicOrgan(this);
        } else {
			MaleReproductiveSystem maleReproductive = newReproductiveSystem.AddComponent<MaleReproductiveSystem>();
			maleReproductive.reproductive = reproductiveSystemScript;
			reproductiveSystemScript.maleReproductiveSystem = maleReproductive;
			maleReproductive.SetupBasicOrgan(this);
		}
	}

	public void MakeChildOrganism (int _amount, BasicOrganismScript _parent) {
		for (int i = 0; i < _amount; i++) {
			animalSpecies.SpawnOrganism(_parent);
		}
	}
}
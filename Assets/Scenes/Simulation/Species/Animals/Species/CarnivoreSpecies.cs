using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivoreSpecies : BasicAnimalSpecies {

	internal override void StartSpecificSimulation() {
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();
		Populate();
	}

	public override void AddBehaviorToNewOrganism(BasicOrganismScript organism, BasicAnimalSpecies animalSpecies) {
		CarnivoreScript carnivoreScript = organism.gameObject.AddComponent<CarnivoreScript>();
		carnivoreScript.carnivoreSpecies = this;
		carnivoreScript.animalSpecies = animalSpecies;
		carnivoreScript.SetUpBehaviorScript();
	}
}

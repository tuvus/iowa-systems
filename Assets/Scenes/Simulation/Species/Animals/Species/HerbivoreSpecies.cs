using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivoreSpecies : BasicAnimalSpecies {

	internal override void StartSpecificSimulation() {
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();
		Populate();
	}
	
	public override void AddBehaviorToNewOrganism(BasicOrganismScript organism, BasicAnimalSpecies animalSpecies) {
		HerbivoreScript herbivoreScript = organism.gameObject.AddComponent<HerbivoreScript>();
		herbivoreScript.herbivoreSpecies = this;
		herbivoreScript.animalSpecies = animalSpecies;
		herbivoreScript.SetUpBehaviorScript();
	}
}

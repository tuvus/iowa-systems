using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmnivoreSpecies : BasicAnimalSpecies {

	internal override void StartSpecificSimulation() {
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();
		Populate();
	}
	
    public override void AddBehaviorToNewOrganism(BasicOrganismScript organism, BasicAnimalSpecies animalSpecies) {
		CarnivoreScript carnivore = organism.gameObject.AddComponent<CarnivoreScript>();
		carnivore.animalSpecies = animalSpecies;
		carnivore.SetUpBehaviorScript();
		carnivore.SetUpBehaviorScript();
    }
}

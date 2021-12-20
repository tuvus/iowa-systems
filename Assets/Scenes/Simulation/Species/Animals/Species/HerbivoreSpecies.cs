using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivoreSpecies : BasicAnimalSpecies {

	public override void AddBehaviorToNewOrganism(BasicOrganismScript organism, BasicAnimalSpecies animalSpecies) {
		HerbivoreScript herbivoreScript = organism.gameObject.AddComponent<HerbivoreScript>();
		herbivoreScript.herbivoreSpecies = this;
		herbivoreScript.animalSpecies = animalSpecies;
		herbivoreScript.SetUpBehaviorScript();
	}
}

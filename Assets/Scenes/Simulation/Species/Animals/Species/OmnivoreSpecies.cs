using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmnivoreSpecies : BasicAnimalSpecies {
    public override void AddBehaviorToNewOrganism(BasicOrganismScript organism, BasicAnimalSpecies animalSpecies) {
		CarnivoreScript carnivore = organism.gameObject.AddComponent<CarnivoreScript>();
		carnivore.animalSpecies = animalSpecies;
		carnivore.SetUpBehaviorScript();
		carnivore.SetUpBehaviorScript();
    }

    public override List<string> GetOrganismFoodTypes() {
        throw new System.NotImplementedException();
    }
}

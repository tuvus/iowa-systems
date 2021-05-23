using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAnimalSpeciesOrganScript : BasicSpeciesOrganScript {
    internal BasicAnimalSpecies animalSpecies;

    public override void SetSpeciesScript(BasicSpeciesScript _species) {
        animalSpecies = (BasicAnimalSpecies)_species;
    }
}

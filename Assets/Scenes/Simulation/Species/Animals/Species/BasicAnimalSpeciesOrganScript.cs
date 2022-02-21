using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAnimalSpeciesOrganScript : BasicSpeciesOrganScript {
    internal AnimalSpecies animalSpecies;

    public override void SetSpeciesScript(BasicSpeciesScript _species) {
        animalSpecies = (AnimalSpecies)_species;
    }
}

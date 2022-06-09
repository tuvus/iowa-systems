using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalSpeciesOrgan : SpeciesOrgan {
    public abstract void MakeOrganism(Animal animal);

    public AnimalSpecies GetAnimalSpecies() {
        return (AnimalSpecies)GetSpecies();
    }
}

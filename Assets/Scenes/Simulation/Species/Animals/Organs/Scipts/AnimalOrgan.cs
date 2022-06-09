using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalOrgan : Organ {

    public void SetupOrgan(AnimalSpeciesOrgan animalSpeciesOrganScript, Animal animal) {
        base.SetupOrgan(animalSpeciesOrganScript, animal);
        GetAnimal().AddOrgan(this);
    }

    public abstract void UpdateOrgan();

    internal override void RemoveFoodTypeFromZone(int zoneIndex) {
    }

    public Animal GetAnimal() {
        return (Animal)GetOrganism();
    }

    public AnimalSpeciesOrgan GetAnimalSpeciesOrgan() {
        return (AnimalSpeciesOrgan)GetSpeciesOrgan();
    }
}

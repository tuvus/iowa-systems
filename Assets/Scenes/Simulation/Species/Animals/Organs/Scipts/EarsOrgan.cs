using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarsOrgan : AnimalOrgan {
	List<GameObject> heardGameobjects = new List<GameObject>();

    public void SetupOrgan(AnimalSpeciesEars animalSpeciesEars, Animal animal) {
        base.SetupOrgan(animalSpeciesEars, animal);
    }

    public override void UpdateOrgan() {
    }

	List<GameObject> GetAllHearableGameobjects(float _range) {
		List<GameObject> hearableGameObjects = new List<GameObject>();
		return hearableGameObjects;
	}

    public override void ResetOrgan() {
    }

    public AnimalSpeciesEars GetAnimalSpeciesEars() {
        return (AnimalSpeciesEars)GetAnimalSpeciesOrgan();
    }
}

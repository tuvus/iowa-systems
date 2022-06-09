using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesNose : AnimalSpeciesOrgan {
	public GameObject nose;

	public float smellRange;

	public override void MakeOrganism(Animal animal) {
		NoseOrgan noseScipt = GetAnimalSpecies().InstantiateNewOrgan(nose, animal).GetComponent<NoseOrgan>();
		noseScipt.SetupOrgan(this, animal);
	}
}

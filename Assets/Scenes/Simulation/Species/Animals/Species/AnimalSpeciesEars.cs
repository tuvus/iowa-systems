using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEars : AnimalSpeciesOrgan {
	public GameObject ears;
	public float hearRange;

	public override void MakeOrganism(Animal animal) {
		EarsOrgan earsScript = GetAnimalSpecies().InstantiateNewOrgan(ears, animal).GetComponent<EarsOrgan>();
		earsScript.SetupOrgan(this, animal);
	}
}

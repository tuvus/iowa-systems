using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEars : BasicAnimalSpeciesOrganScript {

	public GameObject ears;

	public float hearRange;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		EarsScript earsScript = speciesScript.InstantiateNewOrgan(ears, newOrganism).GetComponent<EarsScript>();
		earsScript.hearRange = hearRange;
		earsScript.SetupBasicOrgan(this, newOrganism);
	}
}

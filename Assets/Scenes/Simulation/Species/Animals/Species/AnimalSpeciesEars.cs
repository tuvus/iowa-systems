using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEars : BasicAnimalSpeciesOrganScript {

	public GameObject ears;

	public float hearRange;

	public override void MakeOrganism(BasicOrganismScript _newOrganism) {
		EarsScript earsScript = speciesScript.InstantiateNewOrgan(ears,_newOrganism).GetComponent<EarsScript>();
		earsScript.hearRange = hearRange;
		earsScript.SetupBasicOrgan(this);
	}
}

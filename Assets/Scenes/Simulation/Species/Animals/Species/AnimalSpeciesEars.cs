using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEars : BasicAnimalSpeciesOrganScript {

	public GameObject ears;

	public float hearRange;

	public override void MakeOrganism(GameObject _newOrganism) {
		GameObject newEars = speciesScript.InstantiateNewOrgan(ears,_newOrganism);
		EarsScript earsScript = newEars.GetComponent<EarsScript>();
		earsScript.hearRange = hearRange;
		earsScript.SetupBasicOrgan(this);
	}
}

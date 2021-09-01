using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesNose : BasicAnimalSpeciesOrganScript {
	public GameObject nose;

	public float smellRange;

	public override void MakeOrganism(BasicOrganismScript _newOrganism) {
		GameObject newNose = speciesScript.InstantiateNewOrgan(nose, _newOrganism);
		NoseScript noseScipt = newNose.GetComponent<NoseScript>();
		noseScipt.SetupBasicOrgan(this);
	}
}

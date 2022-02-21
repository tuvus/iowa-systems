using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesNose : BasicAnimalSpeciesOrganScript {
	public GameObject nose;

	public float smellRange;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		GameObject newNose = speciesScript.InstantiateNewOrgan(nose, newOrganism);
		NoseScript noseScipt = newNose.GetComponent<NoseScript>();
		noseScipt.speciesNose = this;
		noseScipt.SetupBasicOrgan(this, newOrganism);
	}
}

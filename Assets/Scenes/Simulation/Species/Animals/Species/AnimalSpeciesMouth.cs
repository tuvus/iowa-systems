using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesMouth : BasicAnimalSpeciesOrganScript {

	public GameObject mouth;

	public float biteSize;
	public float eatRange;
	public float eatTime;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		MouthScript mouthScript = speciesScript.InstantiateNewOrgan(mouth, newOrganism).GetComponent<MouthScript>();
		mouthScript.biteSize = biteSize;
		mouthScript.eatTime = eatTime;
		mouthScript.eatRange = eatRange;
		mouthScript.SetupBasicOrgan(this, newOrganism);
	}
}

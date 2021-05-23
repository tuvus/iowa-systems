using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesMouth : BasicAnimalSpeciesOrganScript {

	public GameObject mouth;

	public float biteSize;
	public float eatTime;

	public override void MakeOrganism(GameObject _newOrganism) {
		GameObject newMouth = speciesScript.InstantiateNewOrgan(mouth,_newOrganism);
		MouthScript plantMouth = newMouth.GetComponent<MouthScript>();
		plantMouth.biteSize = biteSize;
		plantMouth.eatTime = eatTime;
		plantMouth.SetupBasicOrgan(this);
	}
}

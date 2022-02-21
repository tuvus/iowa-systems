using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEyes : BasicAnimalSpeciesOrganScript {

	public GameObject eyes;

	public float sightRange;
	public EyesScript.EyeTypes eyeType;

	public override void MakeOrganism(BasicOrganismScript newOrganism) {
		GameObject newEyes = speciesScript.InstantiateNewOrgan(eyes,newOrganism);
		EyesScript eyesScript = newEyes.GetComponent<EyesScript>();
		eyesScript.speciesEyes = this;
		eyesScript.SetupBasicOrgan(this, newOrganism);
	}
}

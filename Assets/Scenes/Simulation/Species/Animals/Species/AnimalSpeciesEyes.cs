using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEyes : BasicAnimalSpeciesOrganScript {

	public GameObject eyes;

	public float sightRange;
	public EyesScript.EyeTypes eyeType;

	public override void MakeOrganism(BasicOrganismScript _newOrganism) {
		GameObject newEyes = speciesScript.InstantiateNewOrgan(eyes,_newOrganism);
		EyesScript eyesScript = newEyes.GetComponent<EyesScript>();
		eyesScript.sightRange = sightRange * Random.Range(.6f,1.4f);
		eyesScript.speciesEyes = this;
		eyesScript.SetupBasicOrgan(this);
	}
}

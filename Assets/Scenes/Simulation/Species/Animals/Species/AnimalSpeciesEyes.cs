using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEyes : MonoBehaviour {

	public GameObject eyes;

	public float sightRange;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newEyes = Instantiate(eyes, _newOrganism.transform);
		EyesScript eyesScript = newEyes.GetComponent<EyesScript>();

		eyesScript.sightRange = sightRange;
	}
}

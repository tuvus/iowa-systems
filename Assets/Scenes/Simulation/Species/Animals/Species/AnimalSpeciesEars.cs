using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEars : MonoBehaviour {

	public GameObject ears;

	public float hearRange;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newEars = Instantiate(ears, _newOrganism.transform);
		EarsScript earsScript = newEars.GetComponent<EarsScript>();
		earsScript.hearRange = hearRange;
	}
}

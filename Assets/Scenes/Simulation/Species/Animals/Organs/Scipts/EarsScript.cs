using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarsScript : BasicAnimalOrganScript {

	private BasicAnimalScript basicAnimal;

	public float hearRange;

	internal override void SetUpSpecificOrgan() {
		basicAnimal = gameObject.transform.parent.GetComponent<BasicAnimalScript>();
		GetComponent<SphereCollider>().radius = hearRange;
	}

	void OnTriggerEnter(Collider trigg) {
		if (trigg.gameObject.layer == 12) {
			if (basicAnimal.nearbyObjects != null) {
				basicAnimal.nearbyObjects.Add(trigg.transform.parent.gameObject);
			}
		}
	}
	void OnTriggerExit(Collider trigg) {
		if (trigg.gameObject.layer == 12) {
			if (basicAnimal.nearbyObjects != null) {
				basicAnimal.nearbyObjects.Remove(trigg.transform.parent.gameObject);
			}
		}
	}
}

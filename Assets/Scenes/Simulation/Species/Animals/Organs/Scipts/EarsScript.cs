using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarsScript : MonoBehaviour {

	private BasicAnimalScript basicAnimal;

	public float hearRange;

	void Start() {
		basicAnimal = gameObject.transform.parent.GetComponent<BasicAnimalScript>();
		GetComponent<SphereCollider>().radius = hearRange;
	}

	void FixedUpdate() {
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

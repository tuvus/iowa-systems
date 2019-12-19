using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesScript : MonoBehaviour {

	private BasicAnimalScript basicAnimal;

	public float sightRange;

	void Start () {
		basicAnimal = transform.parent.GetComponent<BasicAnimalScript>();
	}
	
	void FixedUpdate () {
		GetComponent<SphereCollider>().radius = sightRange / 2;
		transform.localPosition = new Vector3(sightRange, 0, 0);
	}
	void OnTriggerEnter(Collider trigg) {
		if (trigg.gameObject.layer != 8) {
			if (trigg.gameObject.layer != 10) {
				if (basicAnimal != null) {
					basicAnimal.nearbyObjects.Add(trigg.gameObject);
				}
			}
		}
	}
	void OnTriggerExit(Collider trigg) {
		if (trigg.gameObject.layer != 8) {
			if (trigg.gameObject.layer != 10) {
				if (basicAnimal != null) {
					basicAnimal.nearbyObjects.Remove(trigg.gameObject);
				}
			}
		}
	}
}

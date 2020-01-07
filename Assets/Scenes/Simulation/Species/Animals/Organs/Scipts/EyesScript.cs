using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesScript : MonoBehaviour {

	private BasicAnimalScript basicAnimal;

	public float sightRange;

	void Start () {
		basicAnimal = transform.parent.GetComponent<BasicAnimalScript>();
		GetComponent<SphereCollider>().radius = sightRange * Random.Range(0.8f, 1.2f);
		GetComponent<SphereCollider>().center = new Vector3(GetComponent<SphereCollider>().radius, 0, 0);
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

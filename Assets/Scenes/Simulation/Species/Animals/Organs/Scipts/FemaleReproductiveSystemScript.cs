using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FemaleReproductiveSystemScript : MonoBehaviour {

	ReproductiveSystemScript reproductive;

	public int timeUntilBirth;

	void Start() {
		timeUntilBirth = -1;
		reproductive = GetComponent<ReproductiveSystemScript>();
	}
	void FixedUpdate() {
		if (timeUntilBirth != -1) {
			if (timeUntilBirth > 0) {
				timeUntilBirth --;
				if (timeUntilBirth == 0) {
					timeUntilBirth = reproductive.timeAfterReproduction;
					reproductive.createChildren();
				}
			} else if (timeUntilBirth < 0) {
				timeUntilBirth++;
			}
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthScript : BasicAnimalOrganScript {

	public bool eating;
	public float biteSize;
	public float eatTime;

    internal override void SetUpSpecificOrgan() {
	}

	void OnTriggerEnter(Collider coll) {
		if (basicAnimalScript.behavior.GetBasicAnimal().GetAnimalSpecies().GetDiet().IsEddible(coll.gameObject)) {
			basicAnimalScript.behavior.foodsInRange.Add(coll.gameObject.gameObject);
		}
	}
	void OnTriggerExit(Collider coll) {
		if (basicAnimalScript.behavior.GetBasicAnimal().GetAnimalSpecies().GetDiet().IsEddible(coll.gameObject)) {
			basicAnimalScript.behavior.foodsInRange.Remove(coll.gameObject.gameObject);
		}
	}

	public void Eat() {

	}
}

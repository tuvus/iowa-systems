using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivoreSpecies : BasicAnimalSpecies {

	internal override void StartSpecificSimulation() {
		populationOverTime.Add(organismCount);
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();

		for (int i = 0; i < transform.parent.childCount; i++) {
			if (transform.parent.transform.GetChild(i).gameObject.GetComponent<CarnivoreSpecies>()) {
				//if (transform.parent.transform.GetChild(i).GetComponent<CarnivoreSpecies>().diet.Contains(speciesName)) {
				//	predators.Add(transform.parent.transform.GetChild(i).name);
				//}
			}
		}
		Populate();
	}

	public override BasicBehaviorScript AddBehaviorToNewOrganism(GameObject organism) {
		CarnivoreScript carnivoreScript = organism.AddComponent<CarnivoreScript>();
		carnivoreScript.species = this;
		return carnivoreScript;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivoreSpecies : BasicAnimalSpecies {

	internal override void StartSpecificSimulation () {
		populationOverTime.Add(organismCount);
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();

		for (int i = 0; i < transform.parent.childCount; i++) {
			if (transform.parent.transform.GetChild(i).gameObject.GetComponent<HerbivoreSpecies>()) {
				//if (transform.parent.transform.GetChild(i).GetComponent<HerbivoreSpecies>().diet.Contains(speciesName)) {
				//	predators.Add(transform.parent.transform.GetChild(i).name);
				//}
			}
		}
		Populate();

	}
	
	public override BasicBehaviorScript AddBehaviorToNewOrganism(GameObject organism) {
		HerbivoreScript herbivoreScript = organism.AddComponent<HerbivoreScript>();
		herbivoreScript.species = this;
		return herbivoreScript;
	}

}

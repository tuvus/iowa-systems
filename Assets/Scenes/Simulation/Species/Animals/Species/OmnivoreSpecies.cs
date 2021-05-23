using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmnivoreSpecies : BasicAnimalSpecies {

	internal override void StartSpecificSimulation() {
		populationOverTime.Add(organismCount);
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();

		for (int i = 0; i < transform.parent.childCount; i++) {
			if (transform.parent.transform.GetChild(i).gameObject.GetComponent<OmnivoreSpecies>()) {
				//if (transform.parent.transform.GetChild(i).GetComponent<OmnivoreSpecies>().diet.Contains(speciesName)) {
				//	predators.Add(transform.parent.transform.GetChild(i).name);
				//}
			}
		}
		Populate();
	}
	
	public override void Populate() {
		for (int i = 0; i < organismCount; i++) {
			SpawnSpecificRandomOrganism();
		}
	}

    public override BasicBehaviorScript AddBehaviorToNewOrganism(GameObject organism) {
		return organism.AddComponent<CarnivoreScript>();
    }
}

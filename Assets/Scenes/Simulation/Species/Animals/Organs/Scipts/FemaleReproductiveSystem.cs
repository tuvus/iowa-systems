using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FemaleReproductiveSystem : BasicAnimalOrganScript {

	public ReproductiveSystem reproductive;

	public float timeUntilBirth;
	public float timeAfterReproduction;

	internal override void SetUpSpecificOrgan() {
		timeAfterReproduction = reproductive.animalSpeciesReproductive.reproductionDelay * Random.Range(0.0f, .2f);
	}

	public override void UpdateOrgan() {
		if (timeUntilBirth > 0) {
			timeUntilBirth -= basicAnimalScript.GetEarthScript().simulationDeltaTime * .2f;
			if (timeUntilBirth <= 0) {
				timeUntilBirth = 0;
				Reproduce();
			}
			return;
		}
		if (timeAfterReproduction > 0) {
			timeAfterReproduction -= basicAnimalScript.GetEarthScript().simulationDeltaTime * .2f;
			if (timeAfterReproduction <= 0)
				timeAfterReproduction = 0;
		}
	}

	public bool Concieve() {
		if (ReadyToConcieve()) {
			timeUntilBirth = reproductive.animalSpeciesReproductive.birthTime * Random.Range(0.8f, 1.2f);
			return true;
		}
		return false;
	}

	public bool ReadyToConcieve() {
		if (timeAfterReproduction <= 0 && timeUntilBirth <= 0 && basicAnimalScript.mate != null) {
			return true;
		}
		return false;
	}

	void Reproduce() {
		timeAfterReproduction = reproductive.animalSpeciesReproductive.reproductionDelay * Random.Range(0.8f, 1.2f);
		reproductive.CreateChildren();
	}
}
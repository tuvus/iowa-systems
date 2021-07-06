using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductiveSystem : BasicAnimalOrganScript {

	public AnimalSpeciesReproductiveSystem animalSpeciesReproductive;
	public FemaleReproductiveSystem femaleReproductiveSystem;
	public MaleReproductiveSystem maleReproductiveSystem;

	public float reproductionAge;

	internal override void SetUpSpecificOrgan() {
		reproductionAge = animalSpeciesReproductive.reproductionAge * Random.Range(0.8f, 1.2f);
		behaviorScript.reproductive = this;
	}

    public override void UpdateOrgan() {
    }

	public bool AttemptReproduction() {
		if (maleReproductiveSystem != null && maleReproductiveSystem.Concieve()) {
			return true;
		}
		return false;
	}

	public bool ReadyToAttemptReproduction() {
		if ((maleReproductiveSystem != null && maleReproductiveSystem.ReadyToConcieve()) || (femaleReproductiveSystem != null && femaleReproductiveSystem.ReadyToConcieve())) {
			return true;
        }
		return false;
    }

	public bool CheckMate(BasicAnimalScript _basicAnimal) {
		if (_basicAnimal.GetAnimalSpecies() == basicAnimalScript.GetAnimalSpecies() && _basicAnimal.behavior.reproductive.GetSex() != GetSex() && _basicAnimal.mate == null && PastReproductiveAge() && _basicAnimal.behavior.reproductive.PastReproductiveAge()) {
			return true;
        }
		return false;
    }

	public void CreateChildren () {
		int birthAmmount = animalSpeciesReproductive.reproducionAmount;
		for (int i = 0; i < animalSpeciesReproductive.reproducionAmount; i++) {
			if (Random.Range(0f,100f) > animalSpeciesReproductive.birthSuccessPercent) {
				birthAmmount--;
			}
		}
		basicAnimalScript.behavior.PrintState("Birth:" + birthAmmount, 3);
		animalSpeciesReproductive.MakeChildOrganism(birthAmmount, basicOrganismScript);
	}

	/// <summary>
	/// Returns false for Female and true for Male
	/// </summary>
	public bool GetSex() {
		if (maleReproductiveSystem)
			return true;
		return false;
    }

	public bool PastReproductiveAge() {
		if (basicAnimalScript.age >= reproductionAge)
			return true;
		return false;
    }
}
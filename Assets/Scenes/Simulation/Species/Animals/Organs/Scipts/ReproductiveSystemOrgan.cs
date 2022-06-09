using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductiveSystemOrgan : AnimalOrgan {
	public bool sex;
	public float reproductionAge;
	public float timeUntilBirth;
	public float timeAfterReproduction;

	public void SetupOrgan(AnimalSpeciesReproductiveSystem speciesReproductive, Animal animal) {
		base.SetupOrgan(speciesReproductive, animal);
		animal.reproductive = this;
	}

	public void SpawnReproductive() {
		reproductionAge = GetAnimalSpeciesReproductiveSystem().reproductionAge * Random.Range(0.8f, 1.2f);
		if (Random.Range(0, 2) == 0) {
			sex = false;
		} else {
			sex = true;
		}
		if (IsMature()) {
			GetAnimal().stage = Animal.GrowthStage.Adult;
		}
	}

	public override void UpdateOrgan() {
		if (sex) {
			UpdateMaleOrgan();
		} else {
			UpdateFemaleOrgan();
        }
    }

	public void UpdateMaleOrgan() {
		if (timeAfterReproduction > 0) {
			timeAfterReproduction -= GetAnimal().GetEarthScript().simulationDeltaTime * .2f;
			if (timeAfterReproduction <= 0)
				timeAfterReproduction = 0;
		}
	}

	public void UpdateFemaleOrgan() {
		if (timeUntilBirth > 0) {
			timeUntilBirth -= GetAnimal().GetEarthScript().simulationDeltaTime * .2f;
			if (timeUntilBirth <= 0) {
				timeUntilBirth = 0;
				Reproduce();
			}
			return;
		}
		if (timeAfterReproduction > 0) {
			timeAfterReproduction -= GetAnimal().GetEarthScript().simulationDeltaTime * .2f;
			if (timeAfterReproduction <= 0)
				timeAfterReproduction = 0;
		}
	}

	void Reproduce() {
		timeAfterReproduction = GetAnimalSpeciesReproductiveSystem().reproductionDelay * Random.Range(0.8f, 1.2f);
		CreateChildren();
	}

	public bool AttemptReproduction() {
		if (GetAnimal().mate != null && ReadyToAttemptReproduction() && GetAnimal().mate.GetReproductive().ReadyToAttemptReproduction()) {
			Concieve();
			GetAnimal().mate.GetReproductive().Concieve();
			return true;
		}
		return false;
	}

	public void Concieve() {
		if (sex) {
			timeAfterReproduction = GetAnimalSpeciesReproductiveSystem().reproductionDelay * Random.Range(0.0f, .3f);
		} else {
			timeUntilBirth = GetAnimalSpeciesReproductiveSystem().birthTime * Random.Range(0.8f, 1.2f);
		}
	}

	public bool ReadyToAttemptReproduction() {
		if (timeAfterReproduction <= 0 && GetAnimal().mate != null) {
			if (!sex && timeUntilBirth > 0)
				return false;
			return true;
		}
		return false;
	}

	public bool CheckMate(Animal targetMate) {
		if (targetMate.reproductive.GetSex() == GetSex())
			return false;
		if (!(IsMature() && targetMate.reproductive.IsMature()))
			return false;
		return true;
	}

	public void CreateChildren () {
		int birthAmmount = GetAnimalSpeciesReproductiveSystem().reproducionAmount;
		for (int i = 0; i < GetAnimalSpeciesReproductiveSystem().reproducionAmount; i++) {
			if (Random.Range(0f,100f) > GetAnimalSpeciesReproductiveSystem().birthSuccessPercent) {
				birthAmmount--;
			}
		}
		User.Instance.PrintState("Birth:" + birthAmmount, GetAnimalSpeciesReproductiveSystem().GetAnimalSpecies().speciesDisplayName, 2);
		GetAnimalSpeciesReproductiveSystem().MakeChildOrganism(birthAmmount, GetAnimal());
	}

	/// <summary>
	/// Returns false for Female and true for Male
	/// </summary>
	public bool GetSex() {
		return sex;
    }

	public bool IsMature() {
		if (GetAnimal().age >= reproductionAge)
			return true;
		return false;
    }

    public override void ResetOrgan() {
		sex = false;
		reproductionAge = 0;
		timeUntilBirth = 0;
		timeAfterReproduction = 0;
    }

	public AnimalSpeciesReproductiveSystem GetAnimalSpeciesReproductiveSystem() {
		return (AnimalSpeciesReproductiveSystem)GetAnimalSpeciesOrgan();
    }
}
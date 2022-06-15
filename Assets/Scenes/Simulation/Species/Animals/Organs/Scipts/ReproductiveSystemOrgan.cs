using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductiveSystemOrgan : AnimalOrgan {
    [Tooltip("False = female, True = male")]
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
        sex = Random.Range(0, 2) == 0;
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

    public bool AttemptReproduction(Animal mate) {
        if (ReadyToAttemptReproduction() && mate.GetReproductive().ReadyToAttemptReproduction()) {
            Concieve();
            mate.GetReproductive().Concieve();
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
        return GetAnimal().stage == Animal.GrowthStage.Adult && timeAfterReproduction <= 0 && timeUntilBirth <= 0;
    }

    public void CreateChildren() {
        int birthAmmount = GetAnimalSpeciesReproductiveSystem().reproducionAmount;
        for (int i = 0; i < GetAnimalSpeciesReproductiveSystem().reproducionAmount; i++) {
            if (Random.Range(0f, 100f) > GetAnimalSpeciesReproductiveSystem().birthSuccessPercent) {
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
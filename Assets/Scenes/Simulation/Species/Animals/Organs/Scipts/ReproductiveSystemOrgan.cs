using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductiveSystemOrgan : AnimalOrgan {
    [Tooltip("False = female, True = male")]
    public bool sex;
    [Tooltip("The age in which the animal is ready to reproduce in days")]
    public float reproductionAge;
    [Tooltip("Time until the animal is ready give birth in days")]
    public float timeUntilBirth;
    [Tooltip("Time until the animal is ready to reproduce again in hours")]
    public float timeAfterReproduction;

    public void SetupOrgan(AnimalSpeciesReproductiveSystem speciesReproductive, Animal animal) {
        base.SetupOrgan(speciesReproductive, animal);
        animal.reproductive = this;
    }

    public void SpawnReproductive() {
        reproductionAge = GetAnimalSpeciesReproductiveSystem().reproductionAge * Simulation.randomGenerator.NextFloat(0.8f, 1.2f);
        sex = Simulation.randomGenerator.NextBool();
        if (IsMature()) {
            GetAnimal().stage = Animal.GrowthStage.Adult;
            timeAfterReproduction = GetAnimalSpeciesReproductiveSystem().reproductionDelay * Simulation.randomGenerator.NextFloat(0f, 1.2f);
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
        timeAfterReproduction = Mathf.Max(0, timeAfterReproduction - GetAnimal().GetEarthScript().simulationDeltaTime);
    }

    public void UpdateFemaleOrgan() {
        if (timeUntilBirth > 0) {
            timeUntilBirth = Mathf.Max(0, timeUntilBirth - GetAnimal().GetEarthScript().simulationDeltaTime / 24);
            if (timeUntilBirth <= 0) {
                Reproduce();
            }
            return;
        }
        timeAfterReproduction = Mathf.Max(0, timeAfterReproduction - GetAnimal().GetEarthScript().simulationDeltaTime);
    }

    void Reproduce() {
        timeAfterReproduction = GetAnimalSpeciesReproductiveSystem().reproductionDelay * Simulation.randomGenerator.NextFloat(0.8f, 1.2f);
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
            timeAfterReproduction = GetAnimalSpeciesReproductiveSystem().reproductionDelay * Simulation.randomGenerator.NextFloat(0.0f, .3f);
        } else {
            timeUntilBirth = GetAnimalSpeciesReproductiveSystem().birthTime * Simulation.randomGenerator.NextFloat(0.8f, 1.2f);
        }
    }

    public bool ReadyToAttemptReproduction() {
        return GetAnimal().stage == Animal.GrowthStage.Adult && timeAfterReproduction <= 0 && timeUntilBirth <= 0;
    }

    public void CreateChildren() {
        int birthAmmount = GetAnimalSpeciesReproductiveSystem().reproducionAmount;
        for (int i = 0; i < GetAnimalSpeciesReproductiveSystem().reproducionAmount; i++) {
            if (Simulation.randomGenerator.NextInt(0, 100) < GetAnimalSpeciesReproductiveSystem().birthSuccessPercent) {
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
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAnimalSpecies : BasicSpeciesScript {
	public GameObject deadAnimal;
	public GameObject noise;

	public GameObject basicOrganism;

	//AnimalStats
	public float bodySize;
	public float maxHealth;
	public float speed;
	//Organs
	public float maxFood;
	//Reproduction and age
	public int maxAge;

	[SerializeField] internal List<BasicAnimalScript> animals = new List<BasicAnimalScript>();
	[SerializeField] internal List<BasicAnimalScript> availableMaleMates = new List<BasicAnimalScript>();
	[SerializeField] internal List<BasicAnimalScript> availableFemaleMates = new List<BasicAnimalScript>();
	[SerializeField] internal List<Eddible> eddibleFood = new List<Eddible>();
	[SerializeField] internal List<BasicAnimalScript> predators = new List<BasicAnimalScript>();

    internal override void SetupSpecificSimulation() {
        earth.OnCreateNewOrganism += AddOtherOrganism;
        earth.OnDestroyNewOrganism += RemoveOtherOrganism;
		earth.OnCreateNewFood += AddOtherFood;
		earth.OnDestroyNewFood += RemoveOtherFood;
    }

    internal override void StartSimulation() {
	}

    #region SpawnOrganisms
	public override void Populate() {
		int organismsToSpawn = startingPopulation;
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnSpecificRandomOrganism();
		}
	}

    public override void SpawnSpecificRandomOrganism() {
		BasicAnimalScript basicAnimal = SpawnOrganism(basicOrganism).GetComponent<BasicAnimalScript>();
		SetupRandomOrganism(basicAnimal);
		AddBehaviorToNewOrganism(basicAnimal, this);
		basicAnimal.animalSpecies = this;
		basicAnimal.SetUpOrganism(this,null);
		AddOrganism(basicAnimal);
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(basicAnimal);
		}
		if (basicAnimal.GetReproductive().PastReproductiveAge()) {
			earth.OnEndFrame += basicAnimal.AddAvailableMate;
        }
	}

	public override BasicOrganismScript SpawnSpecificOrganism(BasicOrganismScript parent) {
		BasicAnimalScript basicAnimal = SpawnOrganism(basicOrganism).GetComponent<BasicAnimalScript>();
		SetupChildOrganism(basicAnimal, parent);
		AddBehaviorToNewOrganism(basicAnimal, this);
		basicAnimal.animalSpecies = this;
		basicAnimal.SetUpOrganism(this,parent);
		GetEarthScript().OnEndFrame += basicAnimal.OnAddOrganism;
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(basicAnimal);
		}
		return basicAnimal;
	}

	public abstract void AddBehaviorToNewOrganism(BasicOrganismScript organism, BasicAnimalSpecies animalSpecies);

    #endregion

    #region AnimalControls
    public MeatFoodScript SpawnDeadAnimal(GameObject _animal) {
		MeatFoodScript newDeadBody = Instantiate(deadAnimal, _animal.transform.position, _animal.transform.rotation, null).GetComponent<MeatFoodScript>();
		newDeadBody.transform.SetParent(earth.GetOrganismsTransform());
		return newDeadBody;
	}

	public float GetFoodConsumption() {
		return .2f + .05f * bodySize;
	}
    #endregion

    #region AnimalListControls
    internal override void AddSpecificOrganism(BasicOrganismScript newOrganism) {
		animals.Add((BasicAnimalScript)newOrganism);
    }

    internal override void SpecificOrganismDeath(BasicOrganismScript deadOrganism) {
		animals.Remove((BasicAnimalScript)deadOrganism);
	}

	public void AddAvalibleMate(BasicAnimalScript animal, bool maleOrFemale) {
		if (maleOrFemale) {
			availableMaleMates.Add(animal);
        } else {
			availableFemaleMates.Add(animal);
        }
    }

	public void RemoveAvalibleMate(BasicAnimalScript animal, bool maleOrFemale) {
		if (maleOrFemale) {
			availableMaleMates.Remove(animal);
		} else {
			availableFemaleMates.Remove(animal);
		}
	}

	public bool IsMateAvalible(BasicAnimalScript animal, bool maleOrFemale) {
		if (maleOrFemale && availableMaleMates.Contains(animal)) {
			return true;
		} else if (!maleOrFemale && availableFemaleMates.Contains(animal)) {
			return true;
        }
		return false;
    }

	public List<BasicAnimalScript> GetPotentialMates(bool maleOrFemale) {
		if (maleOrFemale) {
			return availableFemaleMates;
		} else {
			return availableMaleMates;
		}
	}

	public void AddOtherOrganism(object sender, EarthScript.OnOrganismArgs information) {
		if (information.species == this)
			return;
		if (GetDiet().IsEddible(information.organism)) {
            eddibleFood.Add(information.organism.GetEddible());
        }
		if (information.species.GetComponent<BasicAnimalSpecies>() != null && IsPredator(this,information.species.GetComponent<BasicAnimalSpecies>())) {
			predators.Add(information.organism.GetComponent<BasicAnimalScript>());
        }
    }

	public void RemoveOtherOrganism(object sender, EarthScript.OnOrganismArgs information) {
		if (information.species == this)
			return;
		if (GetDiet().IsEddible(information.organism)) {
			eddibleFood.Remove(information.organism.GetEddible());
		}
		if (information.species.GetComponent<BasicAnimalSpecies>() != null && IsPredator(this, information.species.GetComponent<BasicAnimalSpecies>())) {
			predators.Remove(information.organism.GetComponent<BasicAnimalScript>());
		}
	}

	public void AddOtherFood(object sender, EarthScript.OnFoodArgs information) {
		if (GetDiet().IsEddible(information.foodScript)) {
			eddibleFood.Add(information.foodScript.GetEddible());
        }
    }

	public void RemoveOtherFood(object sender, EarthScript.OnFoodArgs information) {
		if (GetDiet().IsEddible(information.foodScript)) {
			eddibleFood.Remove(information.foodScript.GetEddible());
        }
    }
    #endregion

    #region GetMethods
    public bool IsPredator(BasicSpeciesScript _preySpecies, BasicAnimalSpecies _predatorSpecies) {
		if (_predatorSpecies.GetDiet().IsEddible(_preySpecies))
			return true;
		return false;
	}

    public DietScript GetDiet() {
		return GetComponent<DietScript>();
	}
    #endregion
}

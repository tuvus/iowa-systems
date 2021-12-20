using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
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
	public float fullFood;
	public float maxFood;
	//Reproduction and age
	public int maxAge;

	[SerializeField] internal List<BasicAnimalScript> animals = new List<BasicAnimalScript>();
	[SerializeField] internal List<BasicAnimalScript> availableMaleMates = new List<BasicAnimalScript>();
	[SerializeField] internal List<BasicAnimalScript> availableFemaleMates = new List<BasicAnimalScript>();

	AnimalJobController animalJobController;

    #region StartSimulation
    internal override void SetupSpecificSimulation() {
		animalJobController = gameObject.AddComponent<AnimalJobController>();
		animalJobController.SetUpJobController(this,earth);
		fullFood = maxFood * .7f;
    }

    internal override void StartSimulation() {
		Populate();
	}
    #endregion

    #region SpawnOrganisms
    public override void Populate() {
		int organismsToSpawn = startingPopulation;
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnRandomOrganism();
		}
	}

    public override void SpawnRandomOrganism() {
		BasicAnimalScript basicAnimal = SpawnOrganism(basicOrganism).GetComponent<BasicAnimalScript>();
		RandomiseOrganismPosition(basicAnimal);
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

	public override BasicOrganismScript SpawnOrganism(BasicOrganismScript parent) {
		BasicAnimalScript basicAnimal = SpawnOrganism(basicOrganism).GetComponent<BasicAnimalScript>();
		RandomiseOrganismChildPosition(basicAnimal, parent);
		AddBehaviorToNewOrganism(basicAnimal, this);
		basicAnimal.animalSpecies = this;
		basicAnimal.SetUpOrganism(this,parent);
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(basicAnimal);
		}
		return basicAnimal;
	}

	public abstract void AddBehaviorToNewOrganism(BasicOrganismScript organism, BasicAnimalSpecies animalSpecies);

    #endregion

    #region AnimalControls
    public override void UpdateOrganismData() {
		Debug.LogError("not implamented, please fix");
	}

	public override void UpdateOrganismsBehavior() {
        for (int i = 0; i < animals.Count; i++) {
			AnimalActions animalAction = GetAnimalJobController().animalActions[i];
			BasicAnimalScript animalTarget = null;
			Debug.LogError("not implamented, please fix");
     //       switch (animalAction.actionType) {
     //           case AnimalActions.ActionType.Idle:
     //               break;
     //           case AnimalActions.ActionType.RunFromPredator:
					//if (animalAction.index == -1)
					//	Debug.LogError("animalAction.index was not set");
					//animalTarget = predators[animalAction.index];
					//break;
     //           case AnimalActions.ActionType.EatFood:
					//eddibleTarget = eddibleFood[animalAction.index];
					//break;
     //           case AnimalActions.ActionType.GoToFood:
					//eddibleTarget = eddibleFood[animalAction.index];
					//break;
     //           case AnimalActions.ActionType.AttemptReproduction:
     //               break;
     //           case AnimalActions.ActionType.AttemptToMate:
					//if (animals[i].behavior.reproductive.GetSex()) {
					//	animalTarget = availableFemaleMates[animalAction.index];
					//} else {
					//	animalTarget = availableMaleMates[animalAction.index];
					//}
					//break;
     //           case AnimalActions.ActionType.Explore:
     //               break;
     //       }
            //animals[i].UpdateAnimalBehavior(animalAction.actionType,animalTarget,eddibleTarget);
        }
    }

	public override void UpdateOrganisms() {
		for (int i = 0; i < animals.Count; i++) {
			animals[i].UpdateOrganism();
		}
	}

	public float GetFoodConsumption() {
		return .1f + (.05f * bodySize);
	}
    #endregion

    #region AnimalListControls
    internal override void AddSpecificOrganism(BasicOrganismScript newOrganism) {
		animals.Add((BasicAnimalScript)newOrganism);
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
    #endregion

    #region GetMethods
	public List<BasicAnimalScript> GetAnimals() {
		return animals;
    }

	public List<BasicAnimalScript> GetAvailableMaleMates() {
		return availableMaleMates;
	}

	public List<BasicAnimalScript> GetAvailableFemaleMates() {
		return availableFemaleMates;
	}

	public float GetSightRange() {
		return GetComponent<AnimalSpeciesEyes>().sightRange;
    }

	public float GetEatRange() {
		return GetComponent<AnimalSpeciesMouth>().eatRange;
    }

	public float GetSmellRange() {
		return GetComponent<AnimalSpeciesNose>().smellRange;
    }

	public float GetReproductiveAge() {
		return GetComponent<AnimalSpeciesReproductiveSystem>().reproductionAge;
    }

	public AnimalJobController GetAnimalJobController() {
		return animalJobController;
    }

    public override List<string> GetOrganismFoodTypes() {
		List<string> foodTypes = new List<string>();
		foodTypes.Add(speciesName);
		return foodTypes;
    }
    #endregion
}
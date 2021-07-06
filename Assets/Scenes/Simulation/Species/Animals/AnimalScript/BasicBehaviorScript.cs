using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicBehaviorScript : MonoBehaviour {
	public BasicAnimalSpecies animalSpecies;
	
	internal BasicAnimalScript basicAnimal;
	public ReproductiveSystem reproductive;

	public void SetUpBehaviorScript () {
		basicAnimal = GetComponent<BasicAnimalScript>();
		basicAnimal.behavior = this;
    }

	public abstract void UpdateBehavior();


	public BasicAnimalScript GetBasicAnimal() {
		return basicAnimal;
	}

	public bool FindMate() {
		if (basicAnimal.age >= reproductive.reproductionAge && basicAnimal.mate == null) {
            foreach (var potentialMate in basicAnimal.GetEyes().GetAnimalsInRange(animalSpecies.GetPotentialMates(reproductive.GetSex()))) {
				if (reproductive.CheckMate(potentialMate)) {
					basicAnimal.mate = potentialMate;
					potentialMate.mate = basicAnimal;
					return true;
                }
            }
		}
		return false;
	}

	public BasicAnimalScript FindPredator() {
        foreach (var predator in basicAnimal.GetEyes().GetAnimalsInRange(animalSpecies.predators)) {
			return predator;
        }
		return null;
	}

	public Eddible FindFoodInMouthRange() {
		Eddible foundFood = null;
		int foundFoodRank = 0;
        foreach (var eddible in basicAnimal.GetFoodsInRange()) {
			int checkOrganismFoodRank = RankFood(eddible.gameObject);
			if (checkOrganismFoodRank > foundFoodRank) {
				foundFood = eddible;
				foundFoodRank = checkOrganismFoodRank;
            }
        }
		return foundFood;
	}

	public Eddible FindFoodInSightRange() {
		Eddible foundFood = null;
		int foundFoodRank = 0;
		float foodDistance = -1;
		foreach (var eddible in basicAnimal.GetEyes().GetEddiblesInRange(animalSpecies.eddibleFood)) {
			int checkOrganismFoodRank = RankFood(eddible.gameObject);
			if (checkOrganismFoodRank > foundFoodRank) {
				float eddibleDistance = Vector3.Distance(basicAnimal.position, eddible.GetPosition());
				if (foodDistance < 0 || foodDistance > eddibleDistance) {
					foundFood = eddible;
					foundFoodRank = checkOrganismFoodRank;
					foodDistance = eddibleDistance;
				}
			}
		}
		return foundFood;
	}

	public int RankFood(GameObject _food) {
		if (!animalSpecies.GetDiet().IsEddible(_food))
			return -1;
		if (_food.GetComponent<MeatFoodScript>() != null && _food.GetComponent<MeatFoodScript>().HasFood()) {
			return 3;
        } else if (_food.GetComponent<PlantFoodScript>() != null && _food.GetComponent<PlantFoodScript>().HasFood()) {
			return 2;
        } else  if (_food.GetComponent<BasicOrganismScript>() != null) {
			return 1;
        }
		return 0;

    }

	internal bool RunFromPredator() {
		BasicAnimalScript predator = FindPredator();
		if (predator != null) {
			basicAnimal.RunFromOrganism(predator);
			return true;
		}
		return false;
	}

	internal bool GoToFood() {
		if (!basicAnimal.Hungry())
			return false;

		Eddible foodInRange = FindFoodInSightRange();
		if (foodInRange != null) {
			basicAnimal.GoToPoint(foodInRange.transform.position);
			return true;
		}
		return false;
	}

	/// <summary>
	/// For Debuging purposes
	/// </summary>
	internal void PrintState(string text, int printLevel) {
		int printlevelRequirement = 2;
		if (printLevel > printlevelRequirement)
			Debug.Log(animalSpecies.speciesDisplayName + ":" + text);
    }
}
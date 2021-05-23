using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicBehaviorScript : MonoBehaviour {
	public BasicAnimalSpecies animalSpecies;
	
	public BasicAnimalScript basicAnimal;
	public ReproductiveSystem reproductive;
	public List<GameObject> foodsInRange = new List<GameObject>();

	public void SetUpBehaviorScript () {
		basicAnimal = GetComponent<BasicAnimalScript>();
    }

	public BasicAnimalScript GetBasicAnimal() {
		return basicAnimal;
	}

	public bool FindMate() {
		if (basicAnimal.age >= reproductive.reproductionAge && basicAnimal.mate == null) {
			for (int i = 0; i < basicAnimal.nearbyObjects.Count; i++) {
				if (basicAnimal.nearbyObjects[i].gameObject != null && basicAnimal.nearbyObjects[i].GetComponent<BasicAnimalScript>() != null) {
					BasicAnimalScript mateToCheck = basicAnimal.nearbyObjects[i].GetComponent<BasicAnimalScript>();
					if (reproductive.CheckMate(mateToCheck)) {
						basicAnimal.mate = mateToCheck;
						mateToCheck.mate = basicAnimal;
						return true;
					}
				}
			}
		}
		return false;
	}


	public GameObject FindPredator() {
		for (int i = 0; i < basicAnimal.nearbyObjects.Count; i++) {
			if (basicAnimal.nearbyObjects[i] != null && basicAnimal.nearbyObjects[i].GetComponent<BasicAnimalScript>() != null) {
				if (animalSpecies.IsPredator(basicAnimal,basicAnimal.nearbyObjects[i].GetComponent<BasicAnimalScript>())) {
					return basicAnimal.nearbyObjects[i].gameObject;
				}
			}
		}
		return null;
	}

	public GameObject FindFood() {
		GameObject foundFood = null;
		for (int i = 0; i < basicAnimal.nearbyObjects.Count; i++) {
			if (basicAnimal.nearbyObjects[i] == null) {
				continue;
			}
			if (FoodDesire(basicAnimal.nearbyObjects[i]) <= 0)
				continue;
			if (foundFood == null || (FoodDesire(foundFood) < FoodDesire(basicAnimal.nearbyObjects[i]))) {
				foundFood = basicAnimal.nearbyObjects[i];
			}
		}
		return foundFood;
	}

	public int FoodDesire(GameObject _food) {
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


	public void Bite(GameObject _organismToBite) {
		Debug.Log("Bite: " + _organismToBite);
		_organismToBite.GetComponent<BasicAnimalScript>().Eaten(GetComponentInChildren<MouthScript>().biteSize, basicAnimal);
	}

	public bool Eat() {
		if (basicAnimal.waitTime == 0f && !basicAnimal.Full()) {
			for (int i = 0; i < foodsInRange.Count; i++) {
				if (foodsInRange[i] == null) {
					foodsInRange.RemoveAt(i);
				} else if (foodsInRange[i].GetComponent<PlantScript>() != null && foodsInRange[i].GetComponent<PlantScript>().CheckFood() > 0) {
					PlantScript plantScript = foodsInRange[i].GetComponent<PlantScript>();
					basicAnimal.food += plantScript.Eat(GetComponentInChildren<MouthScript>().biteSize);
					GameObject newNoise = Instantiate(animalSpecies.noise, gameObject.transform);
					newNoise.GetComponent<NoiseScript>().time = Random.Range(1.2f, 0.3f);
					newNoise.GetComponent<NoiseScript>().range = Random.Range(GetComponentInChildren<MouthScript>().biteSize, GetComponentInChildren<MouthScript>().biteSize * 3);
					newNoise.GetComponent<NoiseScript>().type = "eatNoise";
					newNoise.transform.localPosition = new Vector3(0, 0, 0);
					basicAnimal.waitTime = 0.3f;
					return true;
				} else if (foodsInRange[i].GetComponent<MeatFoodScript>() != null && foodsInRange[i].GetComponent<MeatFoodScript>().HasFood()) {
					if (animalSpecies.GetDiet().IsEddible(foodsInRange[i].GetComponent<MeatFoodScript>())) {
						MeatFoodScript meatScript = foodsInRange[i].GetComponent<MeatFoodScript>();
						basicAnimal.food += meatScript.Eaten(GetComponentInChildren<MouthScript>().biteSize);
						GameObject newNoise = Instantiate(animalSpecies.noise, gameObject.transform);
						newNoise.GetComponent<NoiseScript>().time = Random.Range(1.2f, 0.3f);
						newNoise.GetComponent<NoiseScript>().range = Random.Range(GetComponentInChildren<MouthScript>().biteSize, GetComponentInChildren<MouthScript>().biteSize * 3);
						newNoise.GetComponent<NoiseScript>().type = "eatNoise";
						newNoise.transform.localPosition = new Vector3(0, 0, 0);
						basicAnimal.waitTime = 0.3f;
						return true;
					}
				} else if (foodsInRange[i].GetComponent<BasicAnimalScript>() != null) {
					if (animalSpecies.GetDiet().IsEddible(foodsInRange[i].GetComponent<BasicAnimalScript>())) {
						Bite(foodsInRange[i]);
						basicAnimal.waitTime = 0.3f;
					}
				}
			}
		}
		return false;
	}

	public void Explore() {
		float random = Random.Range(0,10f);
		transform.Rotate(new Vector3(Random.Range(-random, random), Random.Range(-random, random), Random.Range(-random, random)));
		basicAnimal.moving = true;
	}

	public void FollowMate() {
		transform.LookAt(basicAnimal.mate.transform.position);
		transform.Rotate(transform.up * -90);
		basicAnimal.moving = true;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivoreScript : MonoBehaviour {

	private BasicAnimalScript basicAnimal;
	private ReproductiveSystemScript reproductive;
	public List<GameObject> foodsInRange = new List<GameObject>();

	public GameObject noise;

	private PlantFood target;
	public CarnivoreSpecies species;

	void Start() {
		basicAnimal = GetComponent<BasicAnimalScript>();
		reproductive = GetComponentInChildren<ReproductiveSystemScript>();
	}

	void FixedUpdate() {
		basicAnimal.nearbyObjects.Remove(null);
		foodsInRange.Remove(null);
		basicAnimal.move = false;
		if (basicAnimal.waitTime == 0f) {
			if (FindPredator() != null) {
				//FindPredator
				Debug.Log("PredatorFound");
				transform.LookAt(-FindPredator().transform.position);
				transform.Rotate(transform.up * -90);
				basicAnimal.move = false;
			} else if (Eat()) {
				//CheckIfCanEat
				Debug.Log("HaveEaten");
				basicAnimal.move = false;
				basicAnimal.waitTime = GetComponentInChildren<MouthScript>().eatTime;
			} else if ((basicAnimal.food < basicAnimal.fullFood) && (FindFood() != null)) {
				//CheckForNearbyFood
				Debug.Log("FoundFood(Carnivore)");
				transform.LookAt(FindFood().transform.position);
				transform.Rotate(transform.up * -90);
				basicAnimal.move = true;
			} else if (reproductive.reproduce()) {
				//CheckIfAbleToReproduce
				Debug.Log("AtemptingReproduction");
			} else if (findMate()) {
				//CheckIfAbleToFindMate
				Debug.Log("FoundMate");
			} else {
				if (basicAnimal.touchingEarth) {
                    //Explore
                    transform.Rotate(new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)));
                    basicAnimal.move = true;
				}
			}
		}
	}
	public bool findMate() {
		if (basicAnimal.age >= reproductive.reproductionAge && basicAnimal.mate == null) {
			for (int i = 0; i < basicAnimal.nearbyObjects.Count; i++) {
				GameObject mateToCheck = basicAnimal.nearbyObjects[i].gameObject;
				if (mateToCheck != null) {
					if (mateToCheck.GetComponent<BasicAnimalScript>() != null) {
						if (mateToCheck.GetComponent<BasicAnimalScript>().species == basicAnimal.species) {
							BasicAnimalScript mateCheckScript = mateToCheck.GetComponent<BasicAnimalScript>();
							if (mateCheckScript.GetComponentInChildren<ReproductiveSystemScript>().sex != reproductive.sex && mateCheckScript.mate == null) {
								mateCheckScript.mate = gameObject;
								basicAnimal.mate = mateToCheck.gameObject;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}


	public GameObject FindPredator() {
		for (int i = 0; i < basicAnimal.nearbyObjects.Count; i++) {
			//Checking if the target is a predetor.
			if (basicAnimal.nearbyObjects[i] != null) {
				if (basicAnimal.nearbyObjects[i].GetComponent<BasicAnimalScript>() != null) {
					if (species.predators.Contains(basicAnimal.nearbyObjects[i].gameObject.GetComponent<BasicAnimalScript>().species) == true) {
						Debug.Log("Runn!");
						//Run away
						return basicAnimal.nearbyObjects[i].gameObject;
					}
				}
			}
		}
		return null;
	}
	public GameObject FindFood() {
		for (int i = 0; i < basicAnimal.nearbyObjects.Count; i++) {
			if (basicAnimal.nearbyObjects[i] == null) {
				basicAnimal.nearbyObjects.Remove(null);
			} else if (basicAnimal.nearbyObjects[i].GetComponent<PlantFoodScript>() != null) {
				if (basicAnimal.diet.Contains(basicAnimal.nearbyObjects[i].GetComponent<PlantFoodScript>().foodType)) {
					return basicAnimal.nearbyObjects[i].GetComponent<PlantFoodScript>().gameObject;
				}
			} else if (basicAnimal.nearbyObjects[i].GetComponent<BasicAnimalScript>() != null) {
				if (basicAnimal.diet.Contains(basicAnimal.nearbyObjects[i].GetComponent<BasicAnimalScript>().species)) {
					return basicAnimal.nearbyObjects[i].gameObject;
				}
			}
		}
		return null;
	}

	public void Bite(GameObject _organismToBite) {
		Debug.Log("Bite: " + _organismToBite);
		_organismToBite.GetComponent<BasicAnimalScript>().Eaten(GetComponentInChildren<MouthScript>().biteSize, gameObject);
	}

	public bool Eat() {
		if (basicAnimal.waitTime == 0f && basicAnimal.food < basicAnimal.fullFood) {
			for (int i = 0; i < foodsInRange.Count; i++) {
				if (foodsInRange[i] == null) {
					foodsInRange.RemoveAt(i);
				} else if (foodsInRange[i].GetComponent<PlantFoodScript>() != null) {
					if (foodsInRange[i].GetComponent<PlantFoodScript>().checkFood()) {
						Debug.Log("2");
						basicAnimal.food += foodsInRange[i].GetComponent<PlantFoodScript>().eaten(GetComponentInChildren<MouthScript>().biteSize);
						GameObject newNoise = Instantiate(noise, gameObject.transform);
						newNoise.GetComponent<NoiseScript>().time = Random.Range(1.2f, 0.3f);
						newNoise.GetComponent<NoiseScript>().range = Random.Range(foodsInRange[i].GetComponent<PlantFoodScript>().eatNoiseRange / 2, foodsInRange[i].GetComponent<PlantFoodScript>().eatNoiseRange + 2);
						newNoise.GetComponent<NoiseScript>().type = "eatNoise";
						newNoise.transform.localPosition = new Vector3(0, 0, 0);

						return true;
					}
				} else if (foodsInRange[i].GetComponent<BasicAnimalScript>() != null) {
					if (basicAnimal.diet.Contains(foodsInRange[i].GetComponent<BasicAnimalScript>().species)) {  
						Bite(foodsInRange[i]);
						basicAnimal.waitTime = 0.3f;
					}
				}
			}
		}
		return false;
	}
	public GameObject GetSpecies() {
		return species.gameObject;
	}
}

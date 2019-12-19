using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproductiveSystemScript : MonoBehaviour {

	private BasicAnimalScript basicAnimal;
	public AnimalSpeciesReproductiveSystem animalSpeciesReproductive;

	public bool sex;

	public int maxBirthTime;
	public int timeAfterReproduction;
	public float shortTimeAfterReproduction;
	public float reproductionChance;
	public float reproductionAge;
	public int reproducionAmount;
	public float reproductionSuccessAmount;

	void Start() {
		basicAnimal = GetComponentInParent<BasicAnimalScript>();
		if (Random.Range(0, 2) == 0) {
			sex = true;
			gameObject.AddComponent<FemaleReproductiveSystemScript>();
		} else {
			sex = false;
		}
		reproductionAge = Random.Range(reproductionAge * 0.8f, reproductionAge * 1.2f);
		reproductionChance = Random.Range(reproductionChance * 0.8f, reproductionChance * 1.2f);
	}
	public bool reproduce() {
		if (basicAnimal.age >= reproductionAge && basicAnimal.mate != null && !sex && basicAnimal.mate.GetComponentInChildren<FemaleReproductiveSystemScript>().timeUntilBirth == -1 && basicAnimal.nearbyObjects.Contains(basicAnimal.mate) && basicAnimal.food >= basicAnimal.fullFood) {
			if (Random.Range(0, 100) < reproductionChance && basicAnimal.mate.GetComponent<BasicAnimalScript>().age >= basicAnimal.mate.GetComponentInChildren<ReproductiveSystemScript>().reproductionAge) {
				basicAnimal.mate.GetComponentInChildren<FemaleReproductiveSystemScript>().timeUntilBirth = Mathf.RoundToInt(Random.Range(maxBirthTime * .8f, maxBirthTime * 1.2f));
			}
			basicAnimal.waitTime = shortTimeAfterReproduction;
			basicAnimal.mate.GetComponent<BasicAnimalScript>().waitTime = shortTimeAfterReproduction;
			return true;
		}
		return false;
	}
	public void createChildren () {
		int reproduction = reproducionAmount;
		for (int i = 0; i < reproduction; i++) {
			if (Random.Range(0,100) > reproductionSuccessAmount) {
				reproduction--;
			}
		}
		animalSpeciesReproductive.makeChildOrganism(reproduction, transform.parent.gameObject);
	}
}
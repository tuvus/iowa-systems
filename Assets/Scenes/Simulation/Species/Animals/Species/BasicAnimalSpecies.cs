using System.Collections;
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

	internal override void StartSimulation() {
	}

	public DietScript GetDiet() {
		return GetComponent<DietScript>();
	}

	public override void SpawnSpecificRandomOrganism() {
		GameObject newOrganism = SpawnRandomOrganism(basicOrganism).gameObject;
		BasicBehaviorScript behaviorScript = AddBehaviorToNewOrganism(newOrganism);
		behaviorScript.animalSpecies = this;
		BasicAnimalScript basicAnimal = newOrganism.GetComponent<BasicAnimalScript>();
		basicAnimal.animalSpecies = this;
		basicAnimal.behavior = behaviorScript;
		basicAnimal.SetUpOrganism(this);
		behaviorScript.SetUpBehaviorScript();
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(newOrganism);
		}

		basicAnimal.maxFood = maxFood;
		basicAnimal.bodyFoodConsumption = GetFoodConsumption();
		basicAnimal.maxAge = maxAge;
	}

	public override GameObject SpawnSpecificOrganism(GameObject _parent) {
		GameObject newOrganism = SpawnOrganism(_parent, basicOrganism).gameObject;
		BasicBehaviorScript behaviorScript = AddBehaviorToNewOrganism(newOrganism);
		behaviorScript.animalSpecies = this;
		BasicAnimalScript basicAnimal = newOrganism.GetComponent<BasicAnimalScript>();
		basicAnimal.animalSpecies = this;
		basicAnimal.behavior = behaviorScript;
		basicAnimal.SetUpOrganism(this);
		behaviorScript.SetUpBehaviorScript();
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(newOrganism);
		}

		basicAnimal.maxFood = maxFood;
		basicAnimal.bodyFoodConsumption = GetFoodConsumption();
		basicAnimal.maxAge = maxAge;

		organismCount++;
		return newOrganism;
	}

	public abstract BasicBehaviorScript AddBehaviorToNewOrganism(GameObject organism);

	public override void Populate() {
		int organismsToSpawn = organismCount;
		organismCount = 0;
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnSpecificRandomOrganism();
		}
	}

	public MeatFoodScript SpawnDeadAnimal(GameObject _animal) {
		MeatFoodScript newDeadBody = Instantiate(deadAnimal, _animal.transform.position, _animal.transform.rotation, null).GetComponent<MeatFoodScript>();
		newDeadBody.transform.SetParent(earth.GetOrganismsTransform());
		return newDeadBody;
	}

	public bool IsPredator(BasicAnimalScript _prey,BasicAnimalScript _predator) {
		if (_predator.animalSpecies.GetDiet().IsEddible(_prey))
			return true;
		return false;
	}

	public float GetFoodConsumption() {
		return .2f + .05f * bodySize;
	}
}

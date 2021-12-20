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

	public abstract void UpdateBehavior(AnimalActions.ActionType actionType, BasicAnimalScript animalTarget);
}
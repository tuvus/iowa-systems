using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesReproductiveSystem : MonoBehaviour {

	public GameObject reproductiveSystem;

	public int maxBirthTime;
	public int timeAfterReproduction;
	public float shortTimeAfterReproduction;
	public float reproductionChance;
	public float reproductionAge;
	public int reproducionAmount;
	public float reproductionSuccessAmount;

	void Start() {
		if (timeAfterReproduction > -1) {
			Debug.LogError("timeAfterReproduction Must be less than or equal to -1", this);
		}
	}

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newReproductiveSystem = Instantiate(reproductiveSystem, _newOrganism.transform);
		ReproductiveSystemScript reproductiveSystemScript = newReproductiveSystem.GetComponent<ReproductiveSystemScript>();
		reproductiveSystemScript.animalSpeciesReproductive = this;
		reproductiveSystemScript.reproductionAge = reproductionAge;
		reproductiveSystemScript.reproductionChance = reproductionChance;
		reproductiveSystemScript.reproducionAmount = reproducionAmount;
		reproductiveSystemScript.maxBirthTime = maxBirthTime;
		reproductiveSystemScript.timeAfterReproduction = timeAfterReproduction;
		reproductiveSystemScript.reproductionSuccessAmount = reproductionSuccessAmount;
		reproductiveSystemScript.shortTimeAfterReproduction = shortTimeAfterReproduction;
	}

	public void makeChildOrganism (int _amount, GameObject _parent) {
		for (int i = 0; i < _amount; i++) {
			if (GetComponent<HerbivoreSpecies>()) {
				GetComponent<HerbivoreSpecies>().spawnOrganism(_parent);
			}
			if (GetComponent<CarnivoreSpecies>()) {
				GetComponent<CarnivoreSpecies>().spawnOrganism(_parent);
			}
			if (GetComponent<OmnivoreSpecies>()) {
				
			}
		}
	}
}
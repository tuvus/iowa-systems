using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivoreSpecies : MonoBehaviour {
	public GameObject deadAnimal;
	private GameObject earth;
	private SpeciesMotor history;
	[SerializeField]
	private GameObject noise;

	public GameObject basicOrganism;
	private BasicAnimalScript basicAnimal;
	public string speciesName;
	public string namedSpecies;
	public Color speciesColor;
	//Population start stats
	public int organismCount;
	//SpeciesStats
	public List<string> diet = new List<string>();
	public List<string> predators = new List<string>();
	private List<int> populationOverTime = new List<int>();
	//AnimalStats
	public int mass;
	public float maxHealth;
	public float speed;
	//Organs
	public float maxFood;
	public float bodyFoodConsumption;
	//Reproduction and age
	public int maxAge;
	public float ageMultiplier; //Max age will be determend by a value 0-1 and then multiplied by this

	public void StartSimulation () {
		populationOverTime.Add(organismCount);
		gameObject.name = speciesName;
		earth = GameObject.Find("Earth");
		history = GetComponentInParent<SpeciesMotor>();

		for (int i = 0; i < transform.parent.childCount; i++) {
			if (transform.parent.transform.GetChild(i).gameObject.GetComponent<HerbivoreSpecies>()) {
				if (transform.parent.transform.GetChild(i).GetComponent<HerbivoreSpecies>().diet.Contains(speciesName)) {
					predators.Add(transform.parent.transform.GetChild(i).name);
				}
			}
			if (transform.parent.transform.GetChild(i).gameObject.GetComponent<CarnivoreSpecies>()) {
				if (transform.parent.transform.GetChild(i).GetComponent<CarnivoreSpecies>().diet.Contains(speciesName)) {
					predators.Add(transform.parent.transform.GetChild(i).name);
				}
			}
			if (transform.parent.transform.GetChild(i).gameObject.GetComponent<OmnivoreSpecies>()) {
				if (transform.parent.transform.GetChild(i).GetComponent<OmnivoreSpecies>().diet.Contains(speciesName)) {
					predators.Add(transform.parent.transform.GetChild(i).name);
				}
			}
		}
		Populate();

	}
	private void FixedUpdate() {
		if (history != null) {
			if (history.refreshTime == 0) {
				populationOverTime.Add(organismCount);
			}
		}
	}
	public List<int> ReturnPopulationList() {
		return populationOverTime;
	}

	public void Populate () {
		for (int i = 0; i < organismCount; i++) {
			GameObject newOrganism = Instantiate(basicOrganism, new Vector3(0,0,0), new Quaternion (0,0,0,1), null);
			newOrganism.GetComponent<Renderer>().material.color = speciesColor;
			basicAnimal = newOrganism.GetComponent<BasicAnimalScript>();
			basicAnimal.age = Random.Range(0f, maxAge);
			basicAnimal.gameObject.AddComponent<SpawnRandomizer>();
			newOrganism.AddComponent<HerbivoreScript>().noise = noise;
			basicAnimal.species = speciesName;
			basicAnimal.GetComponent<Rigidbody>().mass = mass;
            newOrganism.GetComponent<HerbivoreScript>().species = this;
            basicAnimal.health = maxHealth;
			basicAnimal.speed = speed;
			//Add if statments and add organs
			if (GetComponent<AnimalSpeciesEars>() != null) {
				GetComponent<AnimalSpeciesEars>().makeOrganism(newOrganism);
			}
			if (GetComponent<AnimalSpeciesEyes>() != null) {
				GetComponent<AnimalSpeciesEyes>().makeOrganism(newOrganism);
			}
			if (GetComponent<AnimalSpeciesMouth>() != null) {
				GetComponent<AnimalSpeciesMouth>().makeOrganism(newOrganism);
			}
			if (GetComponent<AnimalSpeciesReproductiveSystem>() != null) {
				GetComponent<AnimalSpeciesReproductiveSystem>().makeOrganism(newOrganism);
			}

			basicAnimal.maxFood = maxFood;
			basicAnimal.bodyFoodConsumption = bodyFoodConsumption;
			basicAnimal.maxAge = maxAge;
			basicAnimal.diet = diet;
		}
	}
	public GameObject spawnOrganism (GameObject _parent) {
		GameObject newOrganism = Instantiate(basicOrganism, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
		newOrganism.GetComponent<Renderer>().material.color = speciesColor;
		basicAnimal = newOrganism.GetComponent<BasicAnimalScript>();
        SpawnRandomizer spawn = basicAnimal.gameObject.AddComponent<SpawnRandomizer>();
		spawn.parent = _parent;
		spawn.range = .8f;
		newOrganism.AddComponent<HerbivoreScript>().noise = noise;
        newOrganism.GetComponent<HerbivoreScript>().species = this;
        basicAnimal.species = speciesName;
		basicAnimal.GetComponent<Rigidbody>().mass = mass;
		basicAnimal.health = maxHealth;
		basicAnimal.speed = speed;
		//Add if statments and add organs
		if (GetComponent<AnimalSpeciesEars>() != null) {
			GetComponent<AnimalSpeciesEars>().makeOrganism(newOrganism);
		}
		if (GetComponent<AnimalSpeciesEyes>() != null) {
			GetComponent<AnimalSpeciesEyes>().makeOrganism(newOrganism);
		}
		if (GetComponent<AnimalSpeciesMouth>() != null) {
			GetComponent<AnimalSpeciesMouth>().makeOrganism(newOrganism);
		}
		if (GetComponent<AnimalSpeciesReproductiveSystem>() != null) {
			GetComponent<AnimalSpeciesReproductiveSystem>().makeOrganism(newOrganism);
		}

		basicAnimal.maxFood = maxFood;
		basicAnimal.bodyFoodConsumption = bodyFoodConsumption;
		basicAnimal.maxAge = maxAge;
		basicAnimal.diet = diet;
		organismCount++;
		return newOrganism;
	}
}

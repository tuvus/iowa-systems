using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesScript : MonoBehaviour {
	
	private GameObject earth;
	private SpeciesMotor history;
	[SerializeField]
	public GameObject plantType;
	public string speciesName;
	public string namedSpecies;
	public Color speciesColor;
	//Population start stats
	public int plantCount;
	public int seedCount;
	private List<int> populationOverTime = new List<int>();

	//Plant Stats
	public float maxHealth;

	private BasicPlantScript basicPlant;

	public void StartSimulation() {
		populationOverTime.Add(plantCount);
		gameObject.name = speciesName;
		earth = GameObject.Find("Earth");
		history = GetComponentInParent<SpeciesMotor>();

		Populate();
	}
	private void FixedUpdate() {
		if (history != null) {
			if (history.refreshTime == 0) {
				populationOverTime.Add(plantCount);
			}
		}
	}
	public List<int> ReturnPopulationList() {
		return populationOverTime;
	}

	public void Populate() {
		for (int i = 0; i < plantCount; i++) {
			GameObject newPlant = Instantiate(plantType, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
			newPlant.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = speciesColor;
			BasicPlantScript basicPlantScipt = newPlant.GetComponent<BasicPlantScript>();
			basicPlantScipt.maxHealth = maxHealth;
			basicPlantScipt.age = Random.Range(0.0f, 6.6f);
			newPlant.AddComponent<SpawnRandomizer>();
			basicPlantScipt.plantSpecies = gameObject;
			basicPlantScipt.species = speciesName;
			//basicPlant.fertilityConsumption = fertilityConsumption;
			if (GetComponent<PlantSpeciesBlade>() != null) {
				GetComponent<PlantSpeciesBlade>().makeOrganism(newPlant);
			}
			if (GetComponent<PlantSpeciesFlowerSeed>() != null) {
				GetComponent<PlantSpeciesFlowerSeed>().makeOrganism(newPlant);
			}
			if (GetComponent<PlantSpeciesFruit>() != null) {
				GetComponent<PlantSpeciesFruit>().makeOrganism(newPlant);
			}
			if (GetComponent<PlantSpeciesLeaves>() != null) {
				GetComponent<PlantSpeciesLeaves>().makeOrganism(newPlant);
			}
			if (GetComponent<PlantSpeciesRoots>() != null) {
				GetComponent<PlantSpeciesRoots>().makeOrganism(newPlant);
			}
			if (GetComponent<PlantSpeciesSeeds>() != null) {
				GetComponent<PlantSpeciesSeeds>().makeOrganism(newPlant);
			}
			if (GetComponent<PlantSpeciesVegetativePropagation>() != null) {
				GetComponent<PlantSpeciesVegetativePropagation>().makeOrganism(newPlant);
			}
		}
		if (GetComponent<PlantSpeciesSeeds>() != null) {
			GetComponent<PlantSpeciesSeeds>().Populate(seedCount, earth);
		}
	}
	public GameObject SpawnSeedOrganism (GameObject _seed) {
		plantCount += 1;
		GameObject newPlant = Instantiate(plantType, _seed.transform.position, new Quaternion(0, 0, 0, 1), null);
		newPlant.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = speciesColor;
		BasicPlantScript basicPlantScipt = newPlant.GetComponent<BasicPlantScript>();
		basicPlantScipt.maxHealth = maxHealth;
		basicPlantScipt.plantSpecies = gameObject;
		basicPlantScipt.species = speciesName;
		if (GetComponent<PlantSpeciesBlade>() != null) {
			GetComponent<PlantSpeciesBlade>().MakeNewGrownOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesFlowerSeed>() != null) {
			GetComponent<PlantSpeciesFlowerSeed>().MakeNewGrownOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesFruit>() != null) {
			GetComponent<PlantSpeciesFruit>().MakeNewGrownOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesLeaves>() != null) {
			GetComponent<PlantSpeciesLeaves>().MakeNewGrownOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesRoots>() != null) {
			GetComponent<PlantSpeciesRoots>().MakeNewGrownOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesSeeds>() != null) {
			GetComponent<PlantSpeciesSeeds>().makeOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesVegetativePropagation>() != null) {
			GetComponent<PlantSpeciesVegetativePropagation>().makeOrganism(_seed);
		}
		return newPlant;
	}
	public GameObject SpawnVegOrganism(GameObject _parent) {
		Debug.Log("SpawnParent" + _parent);
		plantCount += 1;
		GameObject newPlant = Instantiate(plantType, _parent.transform.position, new Quaternion(0, 0, 0, 1), null);
		newPlant.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = speciesColor;
		SpawnRandomizer spawn = newPlant.gameObject.AddComponent<SpawnRandomizer>();
		spawn.parent = _parent;
		spawn.range = 1.5f;
		BasicPlantScript basicPlantScipt = newPlant.GetComponent<BasicPlantScript>();
		basicPlantScipt.maxHealth = maxHealth;
		basicPlantScipt.plantSpecies = gameObject;
		basicPlantScipt.species = speciesName;
		if (GetComponent<PlantSpeciesBlade>() != null) {
			GetComponent<PlantSpeciesBlade>().makeOrganism(newPlant);
		}
		if (GetComponent<PlantSpeciesFlowerSeed>() != null) {
			GetComponent<PlantSpeciesFlowerSeed>().makeOrganism(newPlant);
		}
		if (GetComponent<PlantSpeciesFruit>() != null) {
			GetComponent<PlantSpeciesFruit>().makeOrganism(newPlant);
		}
		if (GetComponent<PlantSpeciesLeaves>() != null) {
			GetComponent<PlantSpeciesLeaves>().makeOrganism(newPlant);
		}
		if (GetComponent<PlantSpeciesRoots>() != null) {
			GetComponent<PlantSpeciesRoots>().makeOrganism(newPlant);
		}
		if (GetComponent<PlantSpeciesSeeds>() != null) {
			GetComponent<PlantSpeciesSeeds>().makeOrganism(newPlant);
		}
		if (GetComponent<PlantSpeciesVegetativePropagation>() != null) {
			GetComponent<PlantSpeciesVegetativePropagation>().makeOrganism(newPlant);
		}
		return newPlant;
	}
}

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
	public float fertilityConsumption;

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
			basicPlant = newPlant.GetComponent<BasicPlantScript>();
			BasicPlantScript basicPlantScipt = basicPlant.GetComponent<BasicPlantScript>();
			basicPlantScipt.storedGrowth = Random.Range(2.4f, 4.2f);
			basicPlantScipt.age = Random.Range(0.0f, 3.6f);
			basicPlantScipt.fertilityConsumption = fertilityConsumption;
			basicPlant.gameObject.AddComponent<SpawnRandomizer>();
			basicPlant.plantSpecies = gameObject;
			basicPlant.species = speciesName;
			//basicPlant.fertilityConsumption = fertilityConsumption;
			if (GetComponent<PlantSpeciesBlade>() != null) {
				GetComponent<PlantSpeciesBlade>().makeOrganism(newPlant);
			}
			if (GetComponent<PlantSpeciesFlowers>() != null) {
				GetComponent<PlantSpeciesFlowers>().makeOrganism(newPlant);
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
		basicPlant = newPlant.GetComponent<BasicPlantScript>();
		basicPlant.fertilityConsumption = fertilityConsumption;
		basicPlant.plantSpecies = gameObject;
		basicPlant.species = speciesName;
		if (GetComponent<PlantSpeciesBlade>() != null) {
			GetComponent<PlantSpeciesBlade>().makeOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesFlowers>() != null) {
			GetComponent<PlantSpeciesFlowers>().makeOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesFruit>() != null) {
			GetComponent<PlantSpeciesFruit>().makeOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesLeaves>() != null) {
			GetComponent<PlantSpeciesLeaves>().makeOrganism(_seed);
		}
		if (GetComponent<PlantSpeciesRoots>() != null) {
			GetComponent<PlantSpeciesRoots>().makeOrganism(_seed);
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
		SpawnRandomizer spawn = basicPlant.gameObject.AddComponent<SpawnRandomizer>();
		spawn.parent = _parent;
		spawn.range = 1.5f;
		basicPlant = newPlant.GetComponent<BasicPlantScript>();
		basicPlant.storedGrowth = (Random.Range(2.8f, 3.6f));
		basicPlant.plantSpecies = gameObject;
		basicPlant.species = speciesName;
		basicPlant.fertilityConsumption = fertilityConsumption;
		if (GetComponent<PlantSpeciesBlade>() != null) {
			GetComponent<PlantSpeciesBlade>().makeOrganism(newPlant);
		}
		if (GetComponent<PlantSpeciesFlowers>() != null) {
			GetComponent<PlantSpeciesFlowers>().makeOrganism(newPlant);
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

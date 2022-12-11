using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesManager : MonoBehaviour {
	public static SpeciesManager Instance { get; private set; }

	GameObject speciesList;
	[SerializeField]
	private GameObject speciesHolderPrefab;
	private SpeciesMakerPanel speciesMaker;

	[SerializeField] List<GameObject> plantSimulation = new List<GameObject>();
	[SerializeField] List<GameObject> defaultSimulation = new List<GameObject>();

	private SpeciesMotor speciesMotor;

	public void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}
		speciesMaker = GameObject.Find("SpeciesMakerPanel").GetComponent<SpeciesMakerPanel>();
		speciesList = GameObject.Find("SpeciesList");
		speciesMotor = GetComponent<SpeciesMotor>();
	}

	public void SetDefaultSimulation() {
		ClearSpecies();
		speciesMaker.AddListOfSpecies(defaultSimulation);
	}

	public void SetDefaultPlantSimulation() {
		ClearSpecies();
		speciesMaker.AddListOfSpecies(plantSimulation);
	}

	public void ClearSpecies() {
        foreach (var speciesHolder in speciesList.GetComponentsInChildren<SpeciesHolderScript>()) {
			speciesHolder.DeleteSpecies();
        }
	}

	public void AddNewSpecies () {
		speciesMaker.DisplayPanel(true);
	}

	public void SelectSpecies(SpeciesHolderScript speciesHolder) {
		speciesMaker.DisplayPanel(true);
		speciesMaker.EditSpecies(speciesHolder);
	}

	public int GetAllStartingPlantsAndSeeds() {
		int count = 0;
        for (int i = 0; i < transform.childCount; i++) {
			
			PlantSpecies plantSpecies = transform.GetChild(i).GetComponent<PlantSpecies>();
			if (plantSpecies != null) {
				count += plantSpecies.startingPopulation;
				PlantSpeciesAwns speciesSeeds = plantSpecies.GetComponent<PlantSpeciesAwns>();
				if (speciesSeeds != null) {
					count += speciesSeeds.startingSeedCount;
                }
            }
        }
		return count;
	}

	public int GetAllStartingAnimals() {
		int count = 0;
        for (int i = 0; i < transform.childCount; i++) {
			AnimalSpecies animalSpecies = transform.GetChild(i).GetComponent<AnimalSpecies>();
			if (animalSpecies != null) {
				count += animalSpecies.startingPopulation;
				AnimalSpecies speciesSeeds = animalSpecies.GetComponent<AnimalSpecies>();
			}
		}
		return count;
    }

	public SpeciesMotor GetSpeciesMotor() {
		return speciesMotor;
    }
}
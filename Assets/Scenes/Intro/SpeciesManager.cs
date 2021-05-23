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

	public void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}
		speciesMaker = GameObject.Find("SpeciesMakerPanel").GetComponent<SpeciesMakerPanel>();
		speciesList = GameObject.Find("SpeciesList");
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

	public SpeciesMotor GetSpeciesMotor() {
		return GetComponent<SpeciesMotor>();
    }
}
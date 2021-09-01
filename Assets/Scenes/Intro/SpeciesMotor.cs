using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesMotor : MonoBehaviour {
	[SerializeField]
	GameObject populaitonCountPrefab;

	EarthScript earth;
	GameObject canvasUI;
	GraphWindow graphWindow;

	float refreshTime;
	public float maxRefreshTime;
	int refreshCount;

	public void SetupSimulation(EarthScript earth, SunScript sun) {
		this.earth = earth;
		canvasUI = GameObject.Find("Canvas");
		graphWindow = canvasUI.transform.GetChild(0).GetComponent<GraphWindow>();
		graphWindow.gameObject.SetActive(false);
		for (int i = 0; i < transform.childCount; i++) {
			GameObject newCountPrefab = Instantiate(populaitonCountPrefab, GetPopulationCountParent());
			newCountPrefab.GetComponent<SpeciesPopulaitonCount>().SetSpecies(transform.GetChild(i).GetComponent<BasicSpeciesScript>(), i);
		}
        foreach (var speciesHolder in GetAllSpeciesHolders()) {
			speciesHolder.Destroy();
        }
        foreach (var species in GetAllSpecies()) {
			species.SetupSimulation(earth, sun);
        }
	}

	public void StartSimulation() {
        foreach (var species in GetAllSpecies()) {
			species.StartBasicSimulation();
        }
		AddNewData();
		refreshTime = maxRefreshTime;
	}

	public void UpdateSpeciesGraphData() {
		if (refreshTime <= 0) {
			refreshTime = maxRefreshTime;
			refreshCount++;
			AddNewData();
		}
		refreshTime -= earth.simulationDeltaTime;
	}

	public void AddNewData() {
		bool newYMaximum = false;
        foreach (var species in GetAllSpecies()) {
			if (graphWindow.SetPopulationMax(species.GetCurrentPopulation())) {
				newYMaximum = true;
            }
			species.RefreshPopulationList();
		}
		if (newYMaximum) {
			graphWindow.RefreshPopulationMax();
        }
        foreach (var species in GetAllSpecies()) {
			graphWindow.AddNewDot(refreshCount, species.GetCurrentPopulation(), species.speciesColor, species);
		}
	}

	public void ToggleGraph() {
		if (graphWindow.gameObject.activeSelf) {
			graphWindow.gameObject.SetActive(false);
		} else {
			graphWindow.gameObject.SetActive(true);
		}
	}

	public void ShowGraph() {
        //for (int i = 0; i < transform.childCount; i++) {
        //	if (transform.GetChild(i).GetComponent<BasicSpeciesScript>() != null) {
        //		GetGraphWindow().AddDisplaySpecies(transform.GetChild(i).GetComponent<BasicSpeciesScript>().ReturnPopulationList(),
        //			transform.GetChild(i).GetComponent<BasicSpeciesScript>().speciesColor,
        //			transform.GetChild(i).GetComponent<BasicSpeciesScript>());
        //	}
        //}
    }

	public Transform GetPopulationCountParent() {
		return canvasUI.transform.GetChild(1);
    }

	public List<SpeciesHolderScript> GetAllSpeciesHolders() {
		List<SpeciesHolderScript> speciesHolders = new List<SpeciesHolderScript>();
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).GetComponent<SpeciesHolderScript>() != null) {
				speciesHolders.Add(transform.GetChild(i).GetComponent<SpeciesHolderScript>());
			}
		}
		return speciesHolders;
	}

	public List<BasicSpeciesScript> GetAllSpecies() {
		List<BasicSpeciesScript> species = new List<BasicSpeciesScript>();
        for (int i = 0; i < transform.childCount; i++) {
			species.Add(transform.GetChild(i).GetComponent<BasicSpeciesScript>());
        }
		return species;
    }

	public List<BasicAnimalSpecies> GetAllAnimalSpecies() {
		List<BasicAnimalSpecies> animalSpecies = new List<BasicAnimalSpecies>();
        foreach (var species in GetAllSpecies()) {
			if (species.GetComponent<BasicAnimalSpecies>() != null)
				animalSpecies.Add(species.GetComponent<BasicAnimalSpecies>());
        }
		return animalSpecies;
	}
}
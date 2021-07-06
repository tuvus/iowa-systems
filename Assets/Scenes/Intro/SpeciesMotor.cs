using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesMotor : MonoBehaviour {

	GameObject canvasUI;
	[SerializeField]
	GameObject populaitonCountPrefab;
	float refreshTime;
	public float maxRefreshTime;
	int refreshCount;

	public void SetupSimulation(EarthScript earth, SunScript sun) {
		canvasUI = GameObject.Find("Canvas");
		GetGraphWindow().gameObject.SetActive(false);
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

	void FixedUpdate() {
		if (refreshTime <= 0) {
			refreshTime = maxRefreshTime;
			refreshCount++;
			AddNewData();
		}
		refreshTime -= Time.fixedDeltaTime;
	}

	public void AddNewData() {
		bool newYMaximum = false;
        foreach (var species in GetAllSpecies()) {
			if (GetGraphWindow().SetPopulationMax(species.GetCurrentPopulation())) {
				newYMaximum = true;
            }
			species.RefreshPopulationList();
		}
		if (newYMaximum) {
			GetGraphWindow().RefreshPopulationMax();
        }
        foreach (var species in GetAllSpecies()) {
			GetGraphWindow().AddNewDot(refreshCount, species.GetCurrentPopulation(), species.speciesColor, species);
		}
	}

	public void ToggleGraph() {
		if (GetGraphWindow().gameObject.activeSelf) {
			GetGraphWindow().gameObject.SetActive(false);
		} else {
			GetGraphWindow().gameObject.SetActive(true);
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

	public GraphWindow GetGraphWindow() {
		return canvasUI.transform.GetChild(0).GetComponent<GraphWindow>();
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
			if ((BasicAnimalSpecies)species != null)
				animalSpecies.Add((BasicAnimalSpecies)species);
        }
		return animalSpecies;
	}
}
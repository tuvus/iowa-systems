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
	public int maxRefreshTime;
	int refreshCount;

	List<BasicSpeciesScript> allSpecies = new List<BasicSpeciesScript>();
	List<AnimalSpecies> animalSpecies = new List<AnimalSpecies>();
	List<PlantSpecies> plantSpecies = new List<PlantSpecies>();

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
		for (int i = 0; i < transform.childCount; i++) {
			allSpecies.Add(transform.GetChild(i).GetComponent<BasicSpeciesScript>());
			transform.GetChild(i).GetComponent<BasicSpeciesScript>().speciesIndex = i;
		}
		for (int i = 0; i < GetAllSpecies().Count; i++) {
			if (GetAllSpecies()[i].GetComponent<AnimalSpecies>() != null) {
				animalSpecies.Add(GetAllSpecies()[i].GetComponent<AnimalSpecies>());
				GetAllSpecies()[i].specificSpeciesIndex = animalSpecies.Count - 1;
			}
		}
		for (int i = 0; i < GetAllSpecies().Count; i++) {
			if (GetAllSpecies()[i].GetComponent<PlantSpecies>() != null) {
				plantSpecies.Add(GetAllSpecies()[i].GetComponent<PlantSpecies>());
				GetAllSpecies()[i].specificSpeciesIndex = plantSpecies.Count - 1;
			}
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

	public bool GetGraphStatus() {
		return graphWindow.gameObject.activeSelf;
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
		return allSpecies;
	}

	public List<AnimalSpecies> GetAllAnimalSpecies() {

		return animalSpecies;
	}

	public List<PlantSpecies> GetAllPlantSpecies() {
		return plantSpecies;
	}
}
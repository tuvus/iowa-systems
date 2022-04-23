using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesMotor : MonoBehaviour {
	[SerializeField]
	GameObject populationCountPrefab;

	EarthScript earth;
	GameObject canvasUI;
	GraphWindow graphWindow;
	GraphFileManager graphFileManager;

	int refreshTime;
	public int maxRefreshTime;
	public long refreshCount;

	List<BasicSpeciesScript> allSpecies = new List<BasicSpeciesScript>();
	List<AnimalSpecies> animalSpecies = new List<AnimalSpecies>();
	List<PlantSpecies> plantSpecies = new List<PlantSpecies>();

	public void SetupSimulation(EarthScript earth, SunScript sun) {
		this.earth = earth;
		canvasUI = GameObject.Find("Canvas");
		graphWindow = canvasUI.transform.GetChild(0).GetComponent<GraphWindow>();
		graphFileManager = graphWindow.GetComponent<GraphFileManager>();
		for (int i = 0; i < transform.childCount; i++) {
			GameObject newCountPrefab = Instantiate(populationCountPrefab, GetPopulationCountParent());
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
		graphWindow.SetupGraph(maxRefreshTime);
		Color32[] speciesColors = new Color32[GetAllSpecies().Count];
        for (int i = 0; i < GetAllSpecies().Count; i++) {
			speciesColors[i] = GetAllSpecies()[i].speciesColor;
        }
		graphFileManager.SetupFileManager(GetAllSpecies().Count,speciesColors);
		graphWindow.gameObject.SetActive(false);
	}

	public void StartSimulation() {
		foreach (var species in GetAllSpecies()) {
			species.StartBasicSimulation();
		}
		AddNewData();
		refreshTime = 0;
	}

	public void UpdateSpeciesGraphData() {
		if (refreshTime == maxRefreshTime) {
			refreshTime = 0;
			refreshCount++;
			AddNewData();
        }
		refreshTime++;
	}

	public void AddNewData() {
		int[] points = new int[GetAllSpecies().Count];
        for (int i = 0; i < GetAllSpecies().Count; i++) {
			points[i] = GetAllSpecies()[i].GetCurrentPopulation();
        }
		graphFileManager.AddPointsToFile(graphFileManager.GetPopulationFile(), points);
	}

	public void ToggleGraph() {
		if (graphWindow.gameObject.activeSelf) {
			graphWindow.gameObject.SetActive(false);
		} else {
			graphWindow.gameObject.SetActive(true);
			graphWindow.DisplayGraph(graphFileManager.GetPopulationFile());
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
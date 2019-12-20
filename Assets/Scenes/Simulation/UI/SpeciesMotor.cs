using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesMotor : MonoBehaviour {

	private GraphWindow graph;
	[SerializeField]
	private GameObject populaitonCountPrefab;
	private GameObject populationCountParent;
	private GameObject graphNode;
	public int refreshTime;
	public int maxRefreshTime;
	private int highestPop;
	private int refreshCount;

	public void StartSimulation() {
		refreshTime = maxRefreshTime;
		if (GameObject.Find("GraphWindow") != null) {
			graph = GameObject.Find("GraphWindow").GetComponent<GraphWindow>();
			graph.gameObject.SetActive(false);
		}
		if (GameObject.Find("PopulaitonUI") != null) {
			populationCountParent = GameObject.Find("PopulaitonUI");
			for (int i = 0; i < transform.childCount; i++) {
				GameObject newCountPrefab = Instantiate(populaitonCountPrefab, populationCountParent.transform);
				if (transform.GetChild(i).GetComponent<HerbivoreSpecies>() != null) {
					newCountPrefab.GetComponent<SpeciesPopulaitonCount>().SetSpecies(transform.GetChild(i).GetComponent<HerbivoreSpecies>(), null, null, i);
				} else if (transform.GetChild(i).GetComponent<CarnivoreSpecies>() != null) {
					newCountPrefab.GetComponent<SpeciesPopulaitonCount>().SetSpecies(null, transform.GetChild(i).GetComponent<CarnivoreSpecies>(), null, i);
				} else if (transform.GetChild(i).GetComponent<PlantSpeciesScript>() != null) {
					newCountPrefab.GetComponent<SpeciesPopulaitonCount>().SetSpecies(null, null, transform.GetChild(i).GetComponent<PlantSpeciesScript>(), i);
				}     
			}
		}

		for (int i = 0; i < transform.childCount; i++) {
			/*if (transform.GetChild(i).GetComponent<PlantSpeciesScript>() != null) {
				transform.GetChild(i).GetComponent<PlantSpeciesScript>().StartSimulation();
			}
			if (transform.GetChild(i).GetComponent<CarnivoreSpecies>() != null) {
				transform.GetChild(i).GetComponent<CarnivoreSpecies>().StartSimulation();
			}
			if (transform.GetChild(i).GetComponent<HerbivoreSpecies>() != null) {
				transform.GetChild(i).GetComponent<HerbivoreSpecies>().StartSimulation();
			}*/
			if (transform.GetChild(i).GetComponent<SpeciesHolderScript>() != null) {
				transform.GetChild(i).GetComponent<SpeciesHolderScript>().StartSimulation();
			}

		}
	}
	void FixedUpdate () {
		if (refreshTime <= 0) {
			refreshTime = maxRefreshTime;
			refreshCount++;
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild(i).GetComponent<PlantSpeciesScript>() != null) {
					if (graph.yMaximum < transform.GetChild(i).GetComponent<PlantSpeciesScript>().plantCount) {
						graph.yMaximum = Mathf.CeilToInt(graph.yMaximum = transform.GetChild(i).GetComponent<PlantSpeciesScript>().plantCount / 10) * 10 + 10;
					}
				}
				if (transform.GetChild(i).GetComponent<CarnivoreSpecies>() != null) {
					if (graph.yMaximum < transform.GetChild(i).GetComponent<CarnivoreSpecies>().organismCount) {
						graph.yMaximum = Mathf.CeilToInt(graph.yMaximum = transform.GetChild(i).GetComponent<CarnivoreSpecies>().organismCount / 10) * 10 + 10;

					}
				}
				if (transform.GetChild(i).GetComponent<HerbivoreSpecies>() != null) {
					if (graph.yMaximum < transform.GetChild(i).GetComponent<HerbivoreSpecies>().organismCount) {
						graph.yMaximum = Mathf.CeilToInt(graph.yMaximum = transform.GetChild(i).GetComponent<HerbivoreSpecies>().organismCount / 10) * 10 + 10;
						//Debug.Log(Mathf.CeilToInt(graph.yMaximum = transform.GetChild(i).GetComponent<HerbivoreSpecies>().organismCount / 10) * 10 + 10);
					}
				}
			}
			if (graph.gameObject.activeSelf) {
				graph.ClearGraph();
				ShowHistory();
			}
		}
		refreshTime--;

	}
	public void ToggleGraph () {
		if (graph.gameObject.activeSelf) {
			graph.ClearGraph();
			graph.gameObject.SetActive(false);
		} else {
			ShowHistory();
		}
	}
	public void ShowHistory () {
		graph.gameObject.SetActive(true);
		//graph.ShowGeneralGraph(refreshCount);
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).GetComponent<PlantSpeciesScript>() != null) {
				graph.ShowGraph(transform.GetChild(i).GetComponent<PlantSpeciesScript>().ReturnPopulationList(),
				                transform.GetChild(i).GetComponent<PlantSpeciesScript>().speciesColor);
			} else if (transform.GetChild(i).GetComponent<CarnivoreSpecies>() != null) {
				graph.ShowGraph(transform.GetChild(i).GetComponent<CarnivoreSpecies>().ReturnPopulationList(), 
				                transform.GetChild(i).GetComponent<CarnivoreSpecies>().speciesColor);
			} else if (transform.GetChild(i).GetComponent<HerbivoreSpecies>() != null) {
				graph.ShowGraph(transform.GetChild(i).GetComponent<HerbivoreSpecies>().ReturnPopulationList(), 
				                transform.GetChild(i).GetComponent<HerbivoreSpecies>().speciesColor);
			}
		}
	}

}

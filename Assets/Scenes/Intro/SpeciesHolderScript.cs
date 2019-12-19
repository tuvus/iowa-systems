using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesHolderScript : MonoBehaviour {


	public void DeleteSpecies() {
		Destroy(gameObject);
	}

	public void StartSimulation() {
		Destroy(GetComponent<Button>());
		Destroy(GetComponent<Image>());

		if (GetComponent<PlantSpeciesScript>() != null) {
			GetComponent<PlantSpeciesScript>().StartSimulation();
		}
		if (GetComponent<CarnivoreSpecies>() != null) {
			GetComponent<CarnivoreSpecies>().StartSimulation();
		}
		if (GetComponent<HerbivoreSpecies>() != null) {
			GetComponent<HerbivoreSpecies>().StartSimulation();
		}
		Destroy(this);
	}
		
}

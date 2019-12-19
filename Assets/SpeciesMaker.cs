using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesMaker : MonoBehaviour {

	private GameObject species;
	private Transform speciesPresetHolder;
	public GameObject selectedSpecies;
	private Color color;

	private void Start() {
		speciesPresetHolder = GameObject.Find("SpeciesPresetHolder").transform;
		species = GameObject.Find("Species");
	}
	public void DisplaySpecies () {
		GetComponent<Image>().enabled = true;
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(true);
		}
	}

	public void SelectSpecies () {
		string species = transform.GetChild(0).GetComponent<Dropdown>().options[transform.GetChild(0).GetComponent<Dropdown>().value].text;
		for (int i = 0; i < speciesPresetHolder.childCount; i++) {
			if (speciesPresetHolder.GetChild(i).name == species) {
				selectedSpecies = speciesPresetHolder.GetChild(i).gameObject;
			}
		}
	}

	public void CreateSpecies () {
		GameObject newSpecies = Instantiate(selectedSpecies, species.transform);
		newSpecies.transform.GetChild(0).GetComponent<Text>().text = transform.GetChild(1).GetComponent<InputField>().text;
		if (newSpecies.GetComponent<HerbivoreSpecies>() != null) {
			HerbivoreSpecies herbivore = newSpecies.GetComponent<HerbivoreSpecies>();
			herbivore.namedSpecies = transform.GetChild(1).GetComponent<InputField>().text;
			herbivore.organismCount = Mathf.RoundToInt(transform.GetChild(2).GetComponent<Slider>().value);
			herbivore.speciesColor = color;
		} else if (newSpecies.GetComponent<CarnivoreSpecies>() != null) {
			CarnivoreSpecies carnivore = newSpecies.GetComponent<CarnivoreSpecies>();
			carnivore.namedSpecies = transform.GetChild(1).GetComponent<InputField>().text;
			carnivore.organismCount = Mathf.RoundToInt(transform.GetChild(2).GetComponent<Slider>().value);
			carnivore.speciesColor = color;
		} else if (newSpecies.GetComponent<PlantSpeciesScript>() != null) {
			PlantSpeciesScript plantSpecies = newSpecies.GetComponent<PlantSpeciesScript>();
			plantSpecies.namedSpecies = transform.GetChild(1).GetComponent<InputField>().text;
			plantSpecies.plantCount = Mathf.RoundToInt(transform.GetChild(2).GetComponent<Slider>().value);
			plantSpecies.speciesColor = color;
		}
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(false);
		}
		GetComponent<Image>().enabled = false;
	}
	public void RefreshOrganismCount() {
		transform.GetChild(3).GetComponent<Text>().text = ("PopulationCount:" + transform.GetChild(2).GetComponent<Slider>().value);
	}
	public void RefreshColor () {
		color = new Color(transform.GetChild(4).GetComponent<Slider>().value, transform.GetChild(5).GetComponent<Slider>().value, transform.GetChild(6).GetComponent<Slider>().value, 1);
		transform.GetChild(7).GetComponent<Image>().color = color;
	}

}

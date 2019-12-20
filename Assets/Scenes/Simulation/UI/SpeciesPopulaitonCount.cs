using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesPopulaitonCount : MonoBehaviour {

	private HerbivoreSpecies herbivore;
	private CarnivoreSpecies carnivore;
	private PlantSpeciesScript plant;
	private int index;

	public void SetSpecies (HerbivoreSpecies _herbivore, CarnivoreSpecies _carnivore, PlantSpeciesScript _plant, int _index) {
		if (_herbivore != null) {
			herbivore = _herbivore;
			transform.GetChild(0).GetComponent<Text>().color = new Color (herbivore.speciesColor.r,herbivore.speciesColor.g,herbivore.speciesColor.b,1);
			transform.GetChild(0).GetComponent<Text>().text = (herbivore.namedSpecies + "Populaiton:" + herbivore.organismCount);
		} else if (_carnivore != null) {
			carnivore = _carnivore;
			transform.GetChild(0).GetComponent<Text>().color = new Color(carnivore.speciesColor.r, carnivore.speciesColor.g, carnivore.speciesColor.b, 1);
			transform.GetChild(0).GetComponent<Text>().text = (carnivore.namedSpecies + "Populaiton:" + carnivore.organismCount);
		} else if (_plant != null) {
			plant = _plant;
			transform.GetChild(0).GetComponent<Text>().color = new Color(plant.speciesColor.r, plant.speciesColor.g, plant.speciesColor.b, 1);
			transform.GetChild(0).GetComponent<Text>().text = (plant.namedSpecies + "Populaiton:" + plant.plantCount);
		}
		index = _index;
		GetComponent<RectTransform>().localPosition = new Vector2(-30, 45 - (32 * _index));
	}

	void LateUpdate () {
		if (herbivore != null) {
			transform.GetChild(0).GetComponent<Text>().text = (herbivore.namedSpecies + "Populaiton:" + herbivore.organismCount);
		} else if (carnivore != null) {
			transform.GetChild(0).GetComponent<Text>().text = (carnivore.namedSpecies + "Populaiton:" + carnivore.organismCount);
		} else if (plant != null) {
			transform.GetChild(0).GetComponent<Text>().text = (plant.namedSpecies + "Populaiton:" + plant.plantCount);
		}
	}
}

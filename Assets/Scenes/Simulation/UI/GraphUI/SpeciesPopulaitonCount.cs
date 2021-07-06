using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesPopulaitonCount : MonoBehaviour {

	private BasicSpeciesScript speciesScript;
	private int index;

	public void SetSpecies(BasicSpeciesScript _speciesScript, int _index) {
		speciesScript = _speciesScript;
		GetComponent<Image>().color = new Color(speciesScript.speciesColor.r, speciesScript.speciesColor.g, speciesScript.speciesColor.b, 1);
		UpdateInitialText();

		index = _index;
		GetComponent<RectTransform>().localPosition = new Vector2(-30, 45 - (32 * _index));
	}

	void LateUpdate() {
		UpdateText();
	}

	void UpdateInitialText() {
		if (speciesScript.GetComponent<PlantSpeciesSeeds>() != null) {
			transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.GetCurrentPopulation() + "(" + speciesScript.GetComponent<PlantSpeciesSeeds>().startingSeedCount + ")");
			return;
		}
		transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.GetCurrentPopulation());

	}

	void UpdateText() {
		if (speciesScript.GetComponent<PlantSpeciesSeeds>() != null) {
			transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.GetCurrentPopulation() + "(" + speciesScript.GetComponent<PlantSpeciesSeeds>().GetSeedCount() + ")");
			return;
		}
		transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.GetCurrentPopulation());
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesHolderScript : MonoBehaviour {

    private void Start() {
		Refresh();
    }

    public void DeleteSpecies() {
		Destroy(gameObject);
	}

	public void SelectSpecies() {
		SpeciesManager.Instance.SelectSpecies(this);
    }

	public void Refresh () {
		Species speciesScript = GetComponent<Species>();
		GetColorImage().color = speciesScript.speciesColor;
		if (GetComponent<PlantSpeciesAwns>() != null) {
			GetNameText().text = speciesScript.speciesDisplayName + " Pop(" + speciesScript.startingPopulation + ")(" + GetComponent<PlantSpeciesAwns>().startingSeedCount + ")";
			return;
        }
		GetNameText().text = speciesScript.speciesDisplayName + " Pop(" + speciesScript.startingPopulation + ")";
    }

    public void Destroy() {
		Destroy(GetComponent<Button>());
		Destroy(GetComponent<Image>());
		Destroy(this);
    }

    public Image GetColorImage() {
		return transform.GetComponent<Image>();
    }
	public Text GetNameText() {
		return transform.GetChild(1).GetComponent<Text>();
	}
}

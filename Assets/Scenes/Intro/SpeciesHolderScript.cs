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
		BasicSpeciesScript speciesScript = GetComponent<BasicSpeciesScript>();
		GetColorImage().color = speciesScript.speciesColor;
		if (GetComponent<PlantSpeciesSeeds>() != null) {
			GetNameText().text = speciesScript.speciesDisplayName + " Pop(" + speciesScript.startingPopulation + ")(" + GetComponent<PlantSpeciesSeeds>().startingSeedCount + ")";
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

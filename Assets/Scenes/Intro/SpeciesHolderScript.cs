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
		GetNameText().text = speciesScript.speciesDisplayName + " Pop(" + speciesScript.organismCount + ")";
    }

	public void StartSimulation(EarthScript _earth, SunScript _sun) {
		Destroy(GetComponent<Button>());
		Destroy(GetComponent<Image>());

		GetComponent<BasicSpeciesScript>().StartBasicSimulation(_earth, _sun);
		Destroy(this);
	}
	
	public Image GetColorImage() {
		return transform.GetComponent<Image>();
    }
	public Text GetNameText() {
		return transform.GetChild(1).GetComponent<Text>();
	}
}

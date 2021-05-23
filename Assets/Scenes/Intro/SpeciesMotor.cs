using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesMotor : MonoBehaviour {

	GameObject canvasUI;
	[SerializeField]
	GameObject populaitonCountPrefab;
	float refreshTime;
	public float maxRefreshTime;
	int refreshCount;

	public void StartSimulation(EarthScript _earth, SunScript _sun) {
		canvasUI = GameObject.Find("Canvas");
		GetGraphWindow().gameObject.SetActive(false);
		for (int i = 0; i < transform.childCount; i++) {
			GameObject newCountPrefab = Instantiate(populaitonCountPrefab, GetPopulationCountParent());
			newCountPrefab.GetComponent<SpeciesPopulaitonCount>().SetSpecies(transform.GetChild(i).GetComponent<BasicSpeciesScript>(), i);
		}
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).GetComponent<SpeciesHolderScript>() != null) {
				transform.GetChild(i).GetComponent<SpeciesHolderScript>().StartSimulation(_earth, _sun);
			}
		}
		AddNewData();
		refreshTime = maxRefreshTime;
	}

	void FixedUpdate() {
		if (refreshTime <= 0) {
			refreshTime = maxRefreshTime;
			refreshCount++;
			AddNewData();
		}
		refreshTime -= Time.fixedDeltaTime;
	}

	public void AddNewData() {
		bool newYMaximum = false;
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).GetComponent<BasicSpeciesScript>() != null) {
				BasicSpeciesScript speciesScript = transform.GetChild(i).GetComponent<BasicSpeciesScript>();
				if (GetGraphWindow().SetPopulationMax(speciesScript.organismCount)) {
					newYMaximum = true;
                }
				speciesScript.RefreshPopulationList();
			}
		}
		if (newYMaximum) {
			GetGraphWindow().RefreshPopulationMax();
        }
		for (int i = 0; i < transform.childCount; i++) {
			if (transform.GetChild(i).GetComponent<BasicSpeciesScript>() != null) {
				BasicSpeciesScript speciesScript = transform.GetChild(i).GetComponent<BasicSpeciesScript>();
				GetGraphWindow().AddNewDot(refreshCount, speciesScript.GetCurrentPopulation(), speciesScript.speciesColor, speciesScript);
			}
		}
	}

	public void ToggleGraph() {
		if (GetGraphWindow().gameObject.activeSelf) {
			GetGraphWindow().gameObject.SetActive(false);
		} else {
			GetGraphWindow().gameObject.SetActive(true);
		}
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


	public GraphWindow GetGraphWindow() {
		return canvasUI.transform.GetChild(0).GetComponent<GraphWindow>();
    }

	public Transform GetPopulationCountParent() {
		return canvasUI.transform.GetChild(1);
    }
}
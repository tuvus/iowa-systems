﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesPopulaitonCount : MonoBehaviour {

    private Species speciesScript;
    private int index;

    public void SetSpecies(Species speciesScript, int index) {
        this.speciesScript = speciesScript;
        GetComponent<Image>().color = this.speciesScript.speciesColor;
        UpdateInitialText();

        this.index = index;
        GetComponent<RectTransform>().localPosition = new Vector2(-30, 45 - (32 * index));
    }

    void LateUpdate() {
        UpdateText();
    }

    void UpdateInitialText() {
        if (speciesScript.GetComponent<PlantSpeciesAwns>() != null) {
            transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.startingPopulation + "(" + speciesScript.GetComponent<PlantSpeciesAwns>().startingSeedCount + ")");
            return;
        }
        transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.startingPopulation);

    }

    void UpdateText() {
        if (speciesScript.GetComponent<PlantSpeciesAwns>() != null) {
            transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.GetCurrentPopulation() + "(" + speciesScript.GetComponent<PlantSpeciesAwns>().seedList.activeOrganismCount + ")");
            return;
        }
        transform.GetChild(1).GetComponent<Text>().text = (speciesScript.speciesDisplayName + "Pop:" + speciesScript.GetCurrentPopulation());
    }
}

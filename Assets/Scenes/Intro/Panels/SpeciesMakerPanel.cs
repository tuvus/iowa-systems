using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesMakerPanel : MonoBehaviour {

	Transform speciesPresetHolder;
	public GameObject selectedSpecies;
	bool editingSpecies = false;
	Color color;

	private void Start() {
		speciesPresetHolder = GameObject.Find("SpeciesPresetHolder").transform;
		color.a = 1;
        for (int i = 0; i < speciesPresetHolder.childCount; i++) {
			GetSpieciesDropDown().options.Add(new Dropdown.OptionData(speciesPresetHolder.GetChild(i).GetComponent<Species>().speciesName));
        }
	}
	
	public void DisplayPanel(bool _trueOrFalse = true) {
		GetComponent<Image>().enabled = _trueOrFalse;
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(_trueOrFalse);
		}
		DisplaySeedSpeceis(false);
		if (_trueOrFalse == true) {
			SelectSpecies();
        } else {
			selectedSpecies = null;
			GetMainPanelController().DisplayPanel(true);
        }
	}

	public void SelectSpecies () {
		selectedSpecies = null;
		string speciesName = GetSpieciesDropDown().options[GetSpieciesDropDown().value].text;
		for (int i = 0; i < speciesPresetHolder.childCount; i++) {
			Species targetSpecies = speciesPresetHolder.GetChild(i).GetComponent<Species>();
			if (targetSpecies.speciesName == speciesName) {
				selectedSpecies = targetSpecies.gameObject;
				color = targetSpecies.speciesColor;
				SetSlidersToColor(color);
				GetSpeciesPopulationCountSlider().value = targetSpecies.startingPopulation;
				RefreshOrganismCount();
				if (targetSpecies.GetComponent<PlantSpeciesAwns>() != null) {
					GetSpeciesSeedCountSlider().value = targetSpecies.GetComponent<PlantSpeciesAwns>().startingSeedCount;
					DisplaySeedSpeceis(true);
				} else {
					DisplaySeedSpeceis(false);
                }
				ResetFieldInputName();
			}
		}
		GetSpieciesDropDown().interactable = true;
		editingSpecies = false;
		GetDoneButtonText().text = "CreateSpecies";
	}

	public void EditSpecies(SpeciesHolderScript _speciesHolder) {
		selectedSpecies = _speciesHolder.gameObject;
		Species species = selectedSpecies.GetComponent<Species>();
        for (int i = 0; i < GetSpieciesDropDown().options.Count; i++) {
			if (GetSpieciesDropDown().options[i].text == species.speciesName) {
				GetSpieciesDropDown().SetValueWithoutNotify(i);
			}
		}
		GetSpieciesDropDown().interactable = false;
		ResetFieldInputName();
		GetSpeciesPopulationCountSlider().value = species.startingPopulation;
		RefreshOrganismCount();
		if (species.GetComponent<PlantSpeciesAwns>() != null) {
			GetSpeciesSeedCountSlider().value = species.GetComponent<PlantSpeciesAwns>().startingSeedCount;
			DisplaySeedSpeceis(true);
        } else {
			DisplaySeedSpeceis(false);
        }
		SetSlidersToColor(species.speciesColor);
		GetDoneButtonText().text = "ConfirmEdit";
		editingSpecies = true;
	}

	public void CreateSpecies () {
		if (editingSpecies) {
			Species speciesScript = selectedSpecies.GetComponent<Species>();
			speciesScript.speciesDisplayName = GetSpieciesNameInputField().text;
			speciesScript.startingPopulation = Mathf.RoundToInt(GetSpeciesPopulationCountSlider().value);
			speciesScript.speciesColor = color;
			if (speciesScript.GetComponent<PlantSpeciesAwns>() != null)
				speciesScript.GetComponent<PlantSpeciesAwns>().startingSeedCount = (int)GetSpeciesSeedCountSlider().value;

			selectedSpecies.GetComponent<SpeciesHolderScript>().Refresh();
		} else {
			GameObject newSpecies = Instantiate(selectedSpecies, SpeciesManager.Instance.transform);

			Species speciesScript = newSpecies.GetComponent<Species>();
			speciesScript.speciesDisplayName = GetSpieciesNameInputField().text;
			speciesScript.startingPopulation = Mathf.RoundToInt(GetSpeciesPopulationCountSlider().value);
			speciesScript.speciesColor = color;
			if (speciesScript.GetComponent<PlantSpeciesAwns>() != null)
				speciesScript.GetComponent<PlantSpeciesAwns>().startingSeedCount = (int)GetSpeciesSeedCountSlider().value;

			selectedSpecies.GetComponent<SpeciesHolderScript>().Refresh();
		}
		editingSpecies = false;
		DisplayPanel(false);
	}

	public void ResetFieldInputName() {
		//GetSpieciesNameInputField().text = GetSpieciesDropDown().options[GetSpieciesDropDown().value].text;
		GetSpieciesNameInputField().text =	selectedSpecies.GetComponent<Species>().speciesDisplayName;
	}

	public void RefreshOrganismCount() {
		GetSpeciesPopulationCountText().text = ("PopulationCount:" + GetSpeciesPopulationCountSlider().value);
	}

	public void DisplaySeedSpeceis(bool _trueOrFalse) {
		GetSpeciesSeedCountPanel().gameObject.SetActive(_trueOrFalse);
		if (_trueOrFalse)
			RefreshSeedCount();
	}

	public void RefreshSeedCount() {
		GetSpeciesSeedCountText().text = ("SeedCount:" + GetSpeciesSeedCountSlider().value);
	}

	public void SetSlidersToColor(Color _color) {
		GetRedColorSlider().value = _color.r;
		GetGreenColorSlider().value = _color.g;
		GetBlueColorSlider().value = _color.b;
		RefreshColor();
	}

	public void RefreshColor () {
		color = new Color(GetRedColorSlider().value, GetGreenColorSlider().value, GetBlueColorSlider().value, 1);
		GetColorSampleImage().color = color;
	}

	public void AddListOfSpecies(List<GameObject> _speciesList) {
		for (int i = 0; i < _speciesList.Count; i++) {
			GameObject newSpecies = Instantiate(_speciesList[i].GetComponent<SpeciesHolderScript>().gameObject, SpeciesManager.Instance.transform);
			Species speciesScript = newSpecies.GetComponent<Species>();
			newSpecies.transform.GetChild(1).GetComponent<Text>().text = speciesScript.speciesDisplayName;
			newSpecies.transform.GetComponent<Image>().color = speciesScript.speciesColor;
		}
	}

	public void Cancel() {
		editingSpecies = false;
		DisplayPanel(false);
    }

    #region PanelComponents
    public Dropdown GetSpieciesDropDown() {
		return transform.GetChild(1).GetComponent<Dropdown>();
	}

	public InputField GetSpieciesNameInputField() {
		return transform.GetChild(2).GetComponent<InputField>();
	}
	public Text GetSpeciesPopulationCountText() {
		return transform.GetChild(3).GetChild(0).GetComponent<Text>();
	}

	public Slider GetSpeciesPopulationCountSlider() {
		return transform.GetChild(3).GetChild(1).GetComponent<Slider>();
    }

	public GameObject GetSpeciesSeedCountPanel() {
		return transform.GetChild(4).gameObject;
	}

	public Text GetSpeciesSeedCountText() {
		return GetSpeciesSeedCountPanel().transform.GetChild(0).GetComponent<Text>();
	}

	public Slider GetSpeciesSeedCountSlider() {
		return GetSpeciesSeedCountPanel().transform.GetChild(1).GetComponent<Slider>();
	}


	public Slider GetRedColorSlider() {
		return transform.GetChild(5).GetChild(0).GetComponent<Slider>();
	}

	public Slider GetGreenColorSlider() {
		return transform.GetChild(5).GetChild(1).GetComponent<Slider>();
	}

	public Slider GetBlueColorSlider() {
		return transform.GetChild(5).GetChild(2).GetComponent<Slider>();
	}


	public Image GetColorSampleImage() {
		return transform.GetChild(5).GetChild(3).GetComponent<Image>();
	}

	public Text GetDoneButtonText() {
		return transform.GetChild(6).GetChild(0).GetComponent<Text>();
    }
    #endregion

    public MainPanelController GetMainPanelController () {
		return transform.parent.GetChild(0).GetComponent<MainPanelController>();
	}
}
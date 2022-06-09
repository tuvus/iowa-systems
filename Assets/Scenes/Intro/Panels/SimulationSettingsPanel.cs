using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSettingsPanel : MonoBehaviour {
    SimulationScript simulation;
    private int[] earthSizeArray = new int[] { 500, 750, 1000, 1250, 1500, 1750, 2000, 2250, 2500, 3000, 4000, 5000, 7500, 10000 };
    private int[] graphRefreshRateArray = new int[] { 12, 24, 48, 96, 168, 336, 720, 1080, 2880, 4320, 8640 };

    void Start() {
        simulation = SimulationScript.Instance;
        GetSimulationSpeedInputField().text = simulation.simulationSpeed.ToString();
        GetEarthSizeSlider().maxValue = earthSizeArray.Length - 1;
        GetEarthSizeSlider().value = 9;
        GetGraphRefreshSlider().maxValue = graphRefreshRateArray.Length - 1;
        GetGraphRefreshSlider().value = 4;
        OnChangeEarthSizeChange();
        OnChangeGraphRefreshChange();
    }

    public void DisplayPanel(bool _trueOrFalse = true) {
        GetComponent<Image>().enabled = _trueOrFalse;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(_trueOrFalse);
        }
        if (_trueOrFalse == true) {
            GetMainPanelController().DisplayPanel(false);
            GetSunRotationEffectToggle().isOn = simulation.sunRotationEffect;
        } else {
            GetMainPanelController().DisplayPanel(true);
        }
    }

    public void OnSimulationSpeedChange() {
        simulation.simulationSpeed = int.Parse(GetSimulationSpeedInputField().text);
    }

    public void OnChangeEarthSizeChange () {
        GetEarthSizeText().text = "EarthSize:" + earthSizeArray[(int)GetEarthSizeSlider().value];
        simulation.earthSize = earthSizeArray[(int)GetEarthSizeSlider().value];
    }

    public void OnChangeGraphRefreshChange() {
        int totalHours = graphRefreshRateArray[(int)GetGraphRefreshSlider().value];
        SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime = totalHours;
        if (totalHours < 24)
            GetGraphRefreshText().text = "GraphRate: " + (int)totalHours + "Hours";
        else if (totalHours < 168)
            GetGraphRefreshText().text = "GraphRate: " + (int)(totalHours * 10.0f / 24) / 10.0f + "Days";
        else if (totalHours < 720)
            GetGraphRefreshText().text = "GraphRate: " + (int)(totalHours * 10.0f / 168) / 10.0f + "Weeks";
        else if (totalHours < 8640)
            GetGraphRefreshText().text = "GraphRate: " + (int)(totalHours * 10.0f / 720) / 10.0f + "Months";
        else
            GetGraphRefreshText().text = "GraphRate: " + (int)(totalHours * 10.0f / 8640) / 10.0f + "Years";
    }

    public void OnChangeSunRotationEffect() {
        simulation.sunRotationEffect = GetSunRotationEffectToggle().isOn;
    }

    public void Back() {
        DisplayPanel(false);
    }

    #region PanelComponents
    InputField GetSimulationSpeedInputField() {
        return transform.GetChild(1).GetChild(1).GetComponent<InputField>();
    }
    Text GetEarthSizeText() {
        return transform.GetChild(2).GetChild(0).GetComponent<Text>();
    }

    Slider GetEarthSizeSlider() {
        return transform.GetChild(2).GetChild(1).GetComponent<Slider>();
    }

    Text GetGraphRefreshText() {
        return transform.GetChild(3).GetChild(0).GetComponent<Text>();
    }

    Slider GetGraphRefreshSlider() {
        return transform.GetChild(3).GetChild(1).GetComponent<Slider>();
    }

    Toggle GetSunRotationEffectToggle() {
        return transform.GetChild(4).GetChild(1).GetComponent<Toggle>();
    }

    #endregion

    public MainPanelController GetMainPanelController() {
        return transform.parent.GetChild(0).GetComponent<MainPanelController>();
    }
}
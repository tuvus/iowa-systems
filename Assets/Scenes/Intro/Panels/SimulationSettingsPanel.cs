using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationSettingsPanel : MonoBehaviour {
    SimulationScript simulation;
    private int[] earthSizeArray = new int[] { 500, 750, 1000, 1250, 1500, 1750, 2000, 2250, 2500, 3000, 4000, 5000, 7500, 10000 };
    private int[] graphRefreshRateArray = new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 50, 60, 80, 100, 120, 160, 240 };

    void Start() {
        simulation = SimulationScript.Instance;
        GetSimulationSpeedInputField().text = simulation.simulationSpeed.ToString();
        GetEarthSizeSlider().maxValue = earthSizeArray.Length - 1;
        GetEarthSizeSlider().value = 6;
        GetGraphRefreshSlider().maxValue = graphRefreshRateArray.Length - 1;
        GetGraphRefreshSlider().value = 7;
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
        GetGraphRefreshText().text = "GraphRefreshRate:" + graphRefreshRateArray[(int)GetGraphRefreshSlider().value];
        SpeciesManager.Instance.GetSpeciesMotor().maxRefreshTime = graphRefreshRateArray[(int)GetGraphRefreshSlider().value];
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
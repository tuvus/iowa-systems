using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserSettingsPanel : MonoBehaviour {

    private void Awake() {
        for (int i = 0; i < QualitySettings.names.Length; i++) {
            GetQualityLevelDropdown().options.Add(new Dropdown.OptionData(QualitySettings.names[i]));
        }
        for (int i = 0; i < GetQualityLevelDropdown().options.Count; i++) {
            if (GetQualityLevelDropdown().options[i].text == QualitySettings.names[QualitySettings.GetQualityLevel()])
                GetQualityLevelDropdown().SetValueWithoutNotify(i);
        }
    }

    public void DisplayPanel(bool _trueOrFalse = true) {
        GetComponent<Image>().enabled = _trueOrFalse;
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(_trueOrFalse);
        }

        if (_trueOrFalse == true) {
            GetRenderWorldToggle().isOn = User.Instance.GetRenderWorldUserPref();
            GetRenderShadowsToggle().isOn = User.Instance.GetRenderShadowsUserPref();
            GetRenderSunToggle().isOn = User.Instance.GetRenderSunUserPref();
            GetRenderSyboxToggle().isOn = User.Instance.GetRenderSkyboxUserPref();
            GetFramesPerSeccondInputField().text = User.Instance.GetFramesPerSeccondUserPref().ToString();
        } else {
            UpdateDesiredFramesPerSeccondUserPref();
            PlayerPrefs.Save();
            if (Simulation.Instance.simulationInitialised)
                User.Instance.OnChangedSettings();
        }
    }

    public void UpdateRenderWorldUserPref() {
        if (GetRenderWorldToggle().isOn) {
            PlayerPrefs.SetInt("RenderWorld",1);
        } else {
            PlayerPrefs.SetInt("RenderWorld", 0);
        }
    }

    public void UpdateRenderShadowsUserPref() {
        if (GetRenderShadowsToggle().isOn) {
            PlayerPrefs.SetInt("RenderShadows", 1);
        } else {
            PlayerPrefs.SetInt("RenderShadows", 0);
        }
    }

    public void UpdateRenderSunToggleUserPref() {
        if (GetRenderSunToggle().isOn) {
            PlayerPrefs.SetInt("RenderSun", 1);
        } else {
            PlayerPrefs.SetInt("RenderSun", 0);
        }
    }

    public void UpdateRenderSyboxUserPref() {
        if (GetRenderSyboxToggle().isOn) {
            PlayerPrefs.SetInt("RenderSkybox", 1);
        } else {
            PlayerPrefs.SetInt("RenderSkybox", 0);
        }
    }

    public void UpdateQualitySettingsDropdown () {
        QualitySettings.SetQualityLevel(GetQualityLevelDropdown().value);
    }

    public void UpdateDesiredFramesPerSeccondUserPref() {
        PlayerPrefs.SetInt("FramesPerSeccond", int.Parse(GetFramesPerSeccondInputField().text));
    }


    #region PanelComponents
    public Toggle GetRenderWorldToggle() {
        return transform.GetChild(1).GetChild(1).GetComponent<Toggle>();
    }

    public Toggle GetRenderShadowsToggle() {
        return transform.GetChild(2).GetChild(1).GetComponent<Toggle>();
    }

    public Toggle GetRenderSunToggle() {
        return transform.GetChild(3).GetChild(1).GetComponent<Toggle>();
    }

    public Toggle GetRenderSyboxToggle() {
        return transform.GetChild(4).GetChild(1).GetComponent<Toggle>();
    }

    public Dropdown GetQualityLevelDropdown() {
        return transform.GetChild(5).GetChild(1).GetComponent<Dropdown>();
    }

    public InputField GetFramesPerSeccondInputField() {
        return transform.GetChild(6).GetChild(1).GetComponent<InputField>();
    }
    #endregion

}

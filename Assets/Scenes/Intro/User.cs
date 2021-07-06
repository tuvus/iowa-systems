using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour {
    public static User Instance { get; private set; }


    public delegate void ChangedSettingsEventHandler(User user, SettingsEventArgs args);
    public event ChangedSettingsEventHandler changedSettings;


    public void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        SetUpPlayerPrefs();
        OnChangedSettings();
    }

    public void SetUpPlayerPrefs() {
        if (!PlayerPrefs.HasKey("RenderWorld"))
            PlayerPrefs.SetInt("RenderWorld", 1);
        if (!PlayerPrefs.HasKey("RenderShadows"))
            PlayerPrefs.SetInt("RenderShadows", 0);
        if (!PlayerPrefs.HasKey("RenderSun"))
            PlayerPrefs.SetInt("RenderSun", 0);
        if (!PlayerPrefs.HasKey("RenderSkybox"))
            PlayerPrefs.SetInt("RenderSkybox", 1);
    }

    public void StartSimulation() {
        GetUserMotor().enabled = true;
        GetUserMotor().StartSimulation();
        OnChangedSettings();
    }

    public virtual void OnChangedSettings() {
        if (GetRenderSkyboxUserPref()) {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        } else {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }
        if (changedSettings != null) {
            changedSettings(this, new SettingsEventArgs { rendering = GetRenderWorldUserPref(), shadows = GetRenderShadowsUserPref(), sun = GetRenderSunUserPref(), skybox = GetRenderSkyboxUserPref() });
        }
    }

    public UserMotor GetUserMotor() {
        return GetComponent<UserMotor>();
    }

public bool GetRenderWorldUserPref() {
		if (PlayerPrefs.GetInt("RenderWorld") == 0)
			return false;
		return true;
	}

	public bool GetRenderShadowsUserPref() {
		if (PlayerPrefs.GetInt("RenderShadows") == 0)
			return false;
		return true;
	}

	public bool GetRenderSunUserPref() {
		if (PlayerPrefs.GetInt("RenderSun") == 0)
			return false;
		return true;
	}

	public bool GetRenderSkyboxUserPref() {
		if (PlayerPrefs.GetInt("RenderSkybox") == 0)
			return false;
		return true;
	}
}

public class SettingsEventArgs : EventArgs {
	public bool rendering { get; set; }
	public bool shadows { get; set; }
	public bool sun { get; set; }
	public bool skybox { get; set; }
}

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour {
    public static User Instance { get; private set; }

    [Tooltip("Will display messages higher or equal to this number: 0=UnimportantActions, 1=ImportantActions, 2=OrganismDeaths, 3=Nothing")]
    [SerializeField] int debugLogg;

    public void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        SetUpPlayerPrefs();
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
        if (!PlayerPrefs.HasKey("FramesPerSeccond"))
            PlayerPrefs.SetInt("FramesPerSeccond", 60);
        if (!PlayerPrefs.HasKey("GraphBuffer"))
            PlayerPrefs.SetFloat("GraphBuffer", 1);
            PlayerPrefs.SetFloat("GraphBuffer", 1);
    }

    public void SetupSimulation() {
        GetUserMotor().enabled = true;
        GetUserMotor().SetupSimulation(GetGraphBufferUserPref());
        OnChangedSettings();
    }

    public virtual void OnChangedSettings() {
        if (GetRenderSkyboxUserPref()) {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        } else {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }
        Simulation.Instance.SetFrameRate(GetFramesPerSeccondUserPref());
        Simulation.Instance.GetEarth().OnSettingsChanged(GetRenderWorldUserPref(), GetFramesPerSeccondUserPref());
        Simulation.Instance.GetSun().OnSettingsChanged(GetRenderSunUserPref(),GetRenderShadowsUserPref());
    }

    /// <summary>
    /// For Debuging purposes print level will be between 1 and 3 on importance.
    /// </summary>
    internal void PrintState(string text, string speciesDisplayName, int printLevel) {
        if (Application.isEditor && printLevel > debugLogg)
            Debug.Log(speciesDisplayName + ":" + text);
        //Debug.Log(speciesDisplayName + ":" + text + " time" + SimulationScript.Instance.GetEarth().worldTime);

    }

    #region GetMethods
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

    public int GetFramesPerSeccondUserPref() {
        return PlayerPrefs.GetInt("FramesPerSeccond");
    }

    public float GetGraphBufferUserPref() {
        return PlayerPrefs.GetFloat("GraphBuffer");
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour {
    private int[] timeScaleArray = new int[] { 0, 1, 2, 5, 10, 15, 20, 25 };
    private int timeScale;

    public void IncreaceTimeStep () {
        if (timeScale < timeScaleArray.Length - 1) {
            timeScale += 1;
            UpdateTimeScale();
        }
    }

    public void DecreaceTimeStep() {
        if (timeScale > 0) {
            timeScale -= 1;
            UpdateTimeScale();
        }
    }

    public void Pause() {
        timeScale = 0;
        UpdateTimeScale();
    }

    void UpdateTimeScale() {
        Time.timeScale = timeScaleArray[timeScale];
        GetTimeStepText().text = "SimulationSpeed:" + Time.timeScale;
    }

    Text GetTimeStepText() {
        return transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }
}

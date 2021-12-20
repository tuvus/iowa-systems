using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour {
    private readonly int[] timeScaleArray = new int[] { 0, 1, 2, 5, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
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
        GetFrameManager().SetItterationsPerFrame(timeScaleArray[timeScale]);
        //GetTimeStepText().text = "SimulationSpeed:" + GetFrameManager().GetItterationsPerFrame();
    }

    private void LateUpdate() {
        if (GetFrameManager().GetItterationsPerFrame() > GetFrameManager().GetItterationsThatOccuredLastFrame()) {
            GetTimeStepText().text = "SimulationSpeed:" + GetFrameManager().GetItterationsPerFrame() + "<color=red>(" + GetFrameManager().GetItterationsThatOccuredLastFrame() + ") </color>";
        } else {
            GetTimeStepText().text = "SimulationSpeed:" + GetFrameManager().GetItterationsPerFrame();
        }
    }

    Text GetTimeStepText() {
        return transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }

    public FrameManager GetFrameManager() {
        return SimulationScript.Instance.GetEarth().GetFrameManager();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour {
    private readonly int[] timeScaleArray = new int[] { 0, 1, 2, 4, 12, 24, 36, 48, 96, 168, 252, 336, 720, 1080, 1440, 2880, 4320, 8640 };
    private int timeScale;

    Text timeElapsedText;
    Text timeStepText;
    Text targetText;

    public void SetupSimulation() {
        timeElapsedText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        timeStepText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        targetText = transform.GetChild(2).GetChild(0).GetComponent<Text>();
    }

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
        if (!Simulation.Instance.simulationInitialised)
            return;
        GetFrameManager().SetWantedIterationsPerSeccond(timeScaleArray[timeScale]);
        targetText.text = "Target: " + TimeUtil.ConvertHoursToDecimalString(timeScaleArray[timeScale]);
    }

    private void LateUpdate() {
        if (GetFrameManager().GetWantedIterationsPerSeccond() == 0) {
            timeStepText.text = "Paused";
        } else {
            int totalHours = GetFrameManager().GetIterationsOverLastSecond();
            timeStepText.text = TimeUtil.ConvertHoursToString(totalHours) + "/sec";
            timeElapsedText.text = "Time Elapsed:" + TimeUtil.ConvertHoursToString(Simulation.Instance.GetEarth().worldTime);
        }
    }

    public FrameManager GetFrameManager() {
        return Simulation.Instance.GetEarth().GetFrameManager();
    }
}

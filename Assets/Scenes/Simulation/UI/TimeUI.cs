using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour {
    private readonly int[] timeScaleArray = new int[] { 0, 1, 2, 4, 12, 24, 36, 48, 96, 168, 252, 336, 720, 1080, 1440, 2880, 4320, 8640 };
    private int timeScale;

    Text timeStepText;
    Text targetText;

    public void SetupSimulation() {
        timeStepText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        targetText = transform.GetChild(1).GetChild(0).GetComponent<Text>();
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
        if (!SimulationScript.Instance.simulationInitialised)
            return;
        GetFrameManager().SetWantedIterationsPerSeccond(timeScaleArray[timeScale]);
        if (timeScaleArray[timeScale] < 24)
            targetText.text = "Target:" + (int)timeScaleArray[timeScale] + "h"; 
        else if (timeScaleArray[timeScale] < 168)
            targetText.text = "Target:" + (int)(timeScaleArray[timeScale] * 10.0f / 24) / 10.0f + "d"; 
        else if (timeScaleArray[timeScale] < 720)
            targetText.text = "Target:" + (int)(timeScaleArray[timeScale] * 10.0f / 168) / 10.0f + "w";
        else if (timeScaleArray[timeScale] < 8640)
            targetText.text = "Target:" + (int)(timeScaleArray[timeScale] * 10.0f / 720) / 10.0f + "m";
        else
            targetText.text = "Target:" + (int)(timeScaleArray[timeScale] * 10.0f / 8640) / 10.0f + "y";
    }

    private void LateUpdate() {
        if (GetFrameManager().GetWantedIterationsPerSeccond() == 0) {
            timeStepText.text = "Paused";
        } else {
            int totalHours = GetFrameManager().GetIterationsOverSecondProjection();
            if (totalHours >= 8640) {
                int years = totalHours / 8640;
                totalHours = totalHours % 8640;
                int month = totalHours / 720;
                totalHours = totalHours % 720;
                int weeks = totalHours / 168;
                totalHours = totalHours % 168;
                int days = totalHours / 24;
                totalHours = totalHours % 24;
                timeStepText.text = years + "y, " + month + "m, " + weeks + "w, " + days + "d, " + totalHours + "h/Sec";
            } else if (totalHours >= 720) {
                int month = totalHours / 720;
                totalHours = totalHours % 720;
                int weeks = totalHours / 168;
                totalHours = totalHours % 168;
                int days = totalHours / 24;
                totalHours = totalHours % 24;
                timeStepText.text = month + "m, " + weeks + "w, " + days + "d, " + totalHours + "h/Sec";
            } else if (totalHours >= 168) {
                int weeks = totalHours / 168;
                totalHours = totalHours % 168;
                int days = totalHours / 24;
                totalHours = totalHours % 24;
                timeStepText.text = weeks + "w, " + days + "d, " + totalHours + "h/Sec";
            } else if (totalHours >= 24) {
                int days = totalHours / 24;
                totalHours = totalHours % 24;
                timeStepText.text = days + "d, " + totalHours + "h/Sec";
            } else {
                timeStepText.text = totalHours + "h/Sec";
            }
        }
    }



    public FrameManager GetFrameManager() {
        return SimulationScript.Instance.GetEarth().GetFrameManager();
    }
}

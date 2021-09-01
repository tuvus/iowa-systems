using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour {
    int wantedItterationsPerFrame;
    int itterationsPerFrame;

    float frameStartTime;
    float itterationStartTime;
    float itterationEndTime;

    int itterationsThatOccuredLastFrame;

    #region CalculatingTime
    public bool CanStartNewItterationBeforeNextFrame() {
        if (GetTimeRemainingInFrame() > GetItterationTimeEstimate() * 1.4f) {
            return true;
        }
        return false;
    }

    float GetTimeRemainingInFrame() {
        return GetWantedUpdatePerFrame() - (GetTimeSinceStartup() - frameStartTime);
    }

    float GetWantedUpdatePerFrame() {
        float timePerFrame = 1f / wantedItterationsPerFrame;
         return timePerFrame;
    }

    float GetItterationTimeEstimate() {
        return itterationEndTime - itterationStartTime;
    }
    #endregion

    #region LogingTime
    public void UpdateFrameStartTime() {
        frameStartTime = GetTimeSinceStartup();
    }

    public void LogSimulationItterationStart() {
        itterationStartTime = GetTimeSinceStartup();
    }

    public void LogSimulationItterationEnd() {
        itterationEndTime = GetTimeSinceStartup();
    }
    #endregion

    #region VariableControlls
    public void SetWantedItterationsPerFrame(int wantedItterationsPerFrame) {
        this.wantedItterationsPerFrame = wantedItterationsPerFrame;
    }

    public void SetItterationsPerFrame(int itterationsPerFrame) {
        this.itterationsPerFrame = itterationsPerFrame;
    }

    public int GetItterationsPerFrame() {
        return itterationsPerFrame;
    }

    public void SetItterationsThatOccuredThisFrame(int itterations) {
        itterationsThatOccuredLastFrame = itterations;
    }

    public int GetItterationsThatOccuredLastFrame() {
        return itterationsThatOccuredLastFrame;
    }
    #endregion

    float GetTimeSinceStartup () {
        return Time.realtimeSinceStartup;
    }

}

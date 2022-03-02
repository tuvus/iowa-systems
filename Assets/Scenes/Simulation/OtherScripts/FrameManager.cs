using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour {
    int wantedIterationsPerSeccond;
    int[] iterationsOverSecond;
    int iterationsOverSecondIndex;
    int iterationsOverSecondCount;
    int iterationHistoryLength;

    float frameStartTime;
    float iterationStartTime;
    float iterationEndTime;

    #region CalculatingTime
    public int GetWantedIterationsPerSeccond() {
        return wantedIterationsPerSeccond;
    }

    public int GetWantedIterationsThisFrame() {

        float index = iterationsOverSecondIndex % ((float)iterationsOverSecond.Length / (float)(wantedIterationsPerSeccond % iterationsOverSecond.Length));
        if (wantedIterationsPerSeccond % iterationsOverSecond.Length > 0 &&
            index <= 1 && index != 0) {
            return Mathf.Max(wantedIterationsPerSeccond / iterationsOverSecond.Length, 0) + 1;
        }
        return Mathf.Max(wantedIterationsPerSeccond / iterationsOverSecond.Length, 0);
    }

    public bool CanStartNewIterationBeforeNextFrame() {
        if (GetTimeRemainingInFrame() > GetIterationTimeEstimate() * 1.4f) {
            return true;
        }
        return false;
    }

    float GetTimeRemainingInFrame() {
        return GetTimePerFrame() - (GetTimeSinceStartup() - frameStartTime);
    }

    float GetTimePerFrame() {
        float timePerFrame = 1f / iterationsOverSecond.Length;
         return timePerFrame;
    }

    float GetIterationTimeEstimate() {
        return iterationEndTime - iterationStartTime;
    }

    public int GetIterationsThatOccuredLastFrame() {
        if (iterationsOverSecondIndex != 0) {
            return iterationsOverSecond[iterationsOverSecondIndex - 1];
        }
        return iterationsOverSecond[iterationsOverSecond.Length - 1];
    }

    public int GetIterationsOverLastSeccond() {
        return iterationsOverSecondCount;
    }
    #endregion

    #region LogingTime
    public void UpdateFrameStartTime() {
        frameStartTime = GetTimeSinceStartup();
    }

    public void LogSimulationItterationStart() {
        iterationStartTime = GetTimeSinceStartup();
    }

    public void LogSimulationItterationEnd() {
        iterationEndTime = GetTimeSinceStartup();
    }
    #endregion

    #region VariableControlls
    public void EndFrame(int iterations) {
        iterationsOverSecondCount -= iterationsOverSecond[iterationsOverSecondIndex];
        iterationsOverSecond[iterationsOverSecondIndex] = iterations;
        iterationsOverSecondCount += iterations;
        iterationsOverSecondIndex++;
        if (iterationsOverSecondIndex >= iterationsOverSecond.Length)
            iterationsOverSecondIndex = 0;
        iterationHistoryLength++;
    }

    public void SetWantedIterationsPerSeccond(int wantedIterationsPerSeccond) {
        this.wantedIterationsPerSeccond = wantedIterationsPerSeccond;
        iterationHistoryLength = 0;
    }

    public void SetFramesPerSeccond(int fps) {
        iterationsOverSecond = new int[fps];
        iterationHistoryLength = 0;
        iterationsOverSecondIndex = 0;
        iterationsOverSecondCount = 0;
    }
    #endregion

    float GetTimeSinceStartup () {
        return Time.realtimeSinceStartup;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameManager : MonoBehaviour {
    int wantedIterationsPerSecond;
    int[] iterationsOverSecond;
    int iterationsOverSecondIndex;
    int iterationsOverSecondCount;

    float[] frameTimesOverSecond;
    int frameIterationsOverSecondIndex;
    int framesOverSecondCount;
    float timeOverFrames;

    float frameStartTime;
    float iterationStartTime;
    float iterationEndTime;

    #region CalculatingTime
    public int GetWantedIterationsPerSeccond() {
        return wantedIterationsPerSecond;
    }

    public int GetWantedIterationsThisFrame() {
        float index = iterationsOverSecondIndex % ((float)iterationsOverSecond.Length / (float)(wantedIterationsPerSecond % iterationsOverSecond.Length));
        if (wantedIterationsPerSecond % iterationsOverSecond.Length > 0 &&
            index <= 1 && index != 0) {
            return Mathf.Max(wantedIterationsPerSecond / iterationsOverSecond.Length, 0) + 1;
        }
        return Mathf.Max(wantedIterationsPerSecond / iterationsOverSecond.Length, 0);
    }

    public bool ShouldStartNewIteration() {
        if (QualitySettings.GetQualityLevel() == 0)
            return IsInIterationTimePeriod();
        return CanStartNewIterationBeforeNextFrame();
    }

    public bool IsInIterationTimePeriod() {
        return GetTimeRemainingInFrame() >= 0;
    }

    public bool CanStartNewIterationBeforeNextFrame() {
        return GetTimeRemainingInFrame() > GetIterationTimeEstimate();
    }

    float GetTimeRemainingInFrame() {
        return GetTimePerFrame() - GetTimeSinceStartOfFrame();
    }

    float GetTimePerFrame() {
        return 1f / iterationsOverSecond.Length;
    }

    float GetTimeSinceStartOfFrame() {
        return GetTimeSinceStartup() - frameStartTime;
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

    public int GetIterationsOverLastSecond() {
        return iterationsOverSecondCount;
    }

    /// <summary>
    /// Not working
    /// </summary>
    /// <returns></returns>
    public int GetIterationsOverSecondProjection() {
        return Mathf.RoundToInt(iterationsOverSecondCount / GetTimeOverSeccond());
    }

    public float GetTimeOverSeccond() {
        if (framesOverSecondCount == 0)
            return 0;
        return timeOverFrames / (frameTimesOverSecond.Length / framesOverSecondCount);
    }
    #endregion

    #region LogingTime
    public void UpdateFrameStartTime() {
        frameStartTime = GetTimeSinceStartup();
        if (wantedIterationsPerSecond > 0) {
            timeOverFrames -= frameTimesOverSecond[iterationsOverSecondIndex];
            frameTimesOverSecond[frameIterationsOverSecondIndex] = Time.deltaTime;
            timeOverFrames += frameTimesOverSecond[frameIterationsOverSecondIndex];

            frameIterationsOverSecondIndex++;
            if (frameIterationsOverSecondIndex >= frameTimesOverSecond.Length)
                frameIterationsOverSecondIndex = 0;
            framesOverSecondCount = Mathf.Min(framesOverSecondCount + 1, frameTimesOverSecond.Length);
        }
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
    }

    public void SetWantedIterationsPerSeccond(int wantedIterationsPerSeccond) {
        this.wantedIterationsPerSecond = wantedIterationsPerSeccond;
    }

    public void SetFramesPerSeccond(int fps) {
        iterationsOverSecond = new int[fps];
        frameTimesOverSecond = new float[fps];
        iterationsOverSecondIndex = 0;
        iterationsOverSecondCount = 0;
        timeOverFrames = 0;
        framesOverSecondCount = 0;
    }
    #endregion

    float GetTimeSinceStartup() {
        return Time.realtimeSinceStartup;
    }

}

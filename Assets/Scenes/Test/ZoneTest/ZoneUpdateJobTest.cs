using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct ZoneUpdateJobTest : IJobParallelFor {
    [ReadOnly] NativeArray<float3> zonePositions;
    NativeMultiHashMap<int,int>.ParallelWriter neiboringZones;
    int maxNeiboringZones;
    float distance;

    public static JobHandle BeginJob(NativeArray<float3> zonePositions, NativeMultiHashMap<int, int>.ParallelWriter neiboringZones, int maxNeiboringZones, float distance) {
        ZoneUpdateJobTest job = new ZoneUpdateJobTest() { zonePositions = zonePositions, neiboringZones = neiboringZones, maxNeiboringZones = maxNeiboringZones, distance = distance };
        return IJobParallelForExtensions.Schedule(job, zonePositions.Length, 1);
    }

    public void Execute(int index) {
        int neiboringZonesCount = 0;
        for (int i = 0; i < zonePositions.Length; i++) {
            if (i == index)
                continue;
            if (Vector3.Distance(zonePositions[index], zonePositions[i]) < distance) {
                neiboringZones.Add(index, i);
                neiboringZonesCount++;
                if (neiboringZonesCount == maxNeiboringZones) {
                    return;
                }
            }
        }
    }
}

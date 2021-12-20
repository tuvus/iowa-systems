using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct ZoneUpdateJob : IJobParallelFor {
    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;
    NativeMultiHashMap<int, int>.ParallelWriter neiboringZones;
    int maxNeiboringZones;
    float distance;

    public static JobHandle BeginJob(NativeArray<ZoneController.ZoneData> zones, NativeMultiHashMap<int, int>.ParallelWriter neiboringZones, int maxNeiboringZones, float distance) {
        ZoneUpdateJob job = new ZoneUpdateJob() { zones = zones, neiboringZones = neiboringZones, maxNeiboringZones = maxNeiboringZones, distance = distance };
        return IJobParallelForExtensions.Schedule(job, zones.Length, 1);
    }

    public void Execute(int index) {
        int neiboringZonesCount = 0;
        for (int i = 0; i < zones.Length; i++) {
            if (i == index)
                continue;
            if (Vector3.Distance(zones[index].position, zones[i].position) < distance) {
                neiboringZones.Add(index, i);
                neiboringZonesCount++;
                if (neiboringZonesCount == maxNeiboringZones) {
                    return;
                }
            }
        }
    }
}

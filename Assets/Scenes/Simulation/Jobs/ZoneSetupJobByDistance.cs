using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct ZoneSetupJobByDistance : IJobParallelFor {
    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;
    NativeMultiHashMap<int, int>.ParallelWriter neiboringZones;
    NativeArray<float> maxZoneSize;
    int maxNeiboringZones;
    double distance;

    public static JobHandle BeginJob(NativeArray<ZoneController.ZoneData> zones, NativeMultiHashMap<int, int>.ParallelWriter neiboringZones, 
        int maxNeiboringZones, double distance, NativeArray<float> maxZoneSize) {
        ZoneSetupJobByDistance job = new ZoneSetupJobByDistance() { zones = zones, neiboringZones = neiboringZones, maxNeiboringZones = maxNeiboringZones, 
            distance = distance, maxZoneSize = maxZoneSize };
        return IJobParallelForExtensions.Schedule(job, zones.Length, 1);
    }

    public void Execute(int index) {
        int neiboringZonesCount = 0;
        float zoneSize = -1;
        for (int i = 0; i < zones.Length; i++) {
            if (i == index)
                continue;
            float zoneDistance = Vector3.Distance(zones[index].position, zones[i].position);
            if (zoneDistance < distance) {
                neiboringZones.Add(index, i);
                neiboringZonesCount++;
                if (zoneSize < zoneDistance / 2) {
                    zoneSize = zoneDistance / 2;
                }
                if (neiboringZonesCount == maxNeiboringZones) {
                    break;
                }
            }
        }
        maxZoneSize[index] = zoneSize;
    }
}

using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZoneSetupJobByClosest : IJobParallelFor {
    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;
    NativeMultiHashMap<int, int>.ParallelWriter neiboringZones;
    NativeArray<float> maxZoneSize;
    int maxNeiboringZones;

    public static JobHandle BeginJob(NativeArray<ZoneController.ZoneData> zones, NativeMultiHashMap<int, int>.ParallelWriter neiboringZones,
        int maxNeiboringZones, NativeArray<float> maxZoneSize) {
        ZoneSetupJobByClosest job = new ZoneSetupJobByClosest() { zones = zones, neiboringZones = neiboringZones,
            maxNeiboringZones = maxNeiboringZones, maxZoneSize = maxZoneSize };
        return IJobParallelForExtensions.Schedule(job, zones.Length, 1);
    }

    public void Execute(int index) {
        List<int> tempNeiboringZones = new List<int>(maxNeiboringZones);
        List<float> tempNeiboringZonesDistance = new List<float>(maxNeiboringZones);
        for (int i = 0; i < zones.Length; i++) {
            if (i == index)
                continue;
            float distance = Vector3.Distance(zones[index].position, zones[i].position);
            if (tempNeiboringZones.Count < maxNeiboringZones) {
                bool added = false;
                for (int f = 0; f < tempNeiboringZonesDistance.Count; f++) {
                    if (distance < tempNeiboringZonesDistance[f]) {
                        tempNeiboringZones.Insert(f, i);
                        tempNeiboringZonesDistance.Insert(f, distance);
                        added = true;
                        break;
                    }
                }
                if (!added) {
                    tempNeiboringZones.Add(i);
                    tempNeiboringZonesDistance.Add(distance);
                }
            }else if (distance < tempNeiboringZonesDistance[tempNeiboringZonesDistance.Count - 1]) {
                for (int f = 0; f < tempNeiboringZonesDistance.Count; f++) {
                    if (distance < tempNeiboringZonesDistance[f]) {
                        tempNeiboringZones[f] = i;
                        tempNeiboringZonesDistance[f] = distance;
                        break;
                    }
                }
            }
        }
        maxZoneSize[index] = tempNeiboringZonesDistance[tempNeiboringZonesDistance.Count - 1];
        for (int i = 0; i < tempNeiboringZones.Count; i++) {
            neiboringZones.Add(index,tempNeiboringZones[i]);
        }
    }
}

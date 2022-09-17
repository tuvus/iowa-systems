using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;

public struct FindZonesJob : IJobParallelFor {
    NativeArray<FindZoneController.FindZoneData> findZones;

    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;
    [ReadOnly] NativeParallelMultiHashMap<int, int> neiboringZones;

    public static JobHandle BeginJob(NativeArray<FindZoneController.FindZoneData> findZones, int findZoneCount, NativeArray<ZoneController.ZoneData> zones,
        NativeParallelMultiHashMap<int, int> neiboringZones) {
        FindZonesJob job = new FindZonesJob { findZones = findZones, zones = zones, neiboringZones = neiboringZones };
        return IJobParallelForExtensions.Schedule(job, findZoneCount, 1);
    }

    public void Execute(int index) {
        if (findZones[index].zone == -1)
            findZones[index] = FindZoneFromAllZones(index);
        else
            findZones[index] = FindNearbyZone(index);
    }

    FindZoneController.FindZoneData FindZoneFromAllZones(int index) {
        float distance = -1;
        int zoneIndex = -1;
        for (int i = 0; i < zones.Length; i++) {
            float newDistance = GetDistanceBetweenPoints(findZones[index].position, zones[i].position);
            if (newDistance < distance || distance < 0) {
                distance = newDistance;
                zoneIndex = i;
            }
        }
        return new FindZoneController.FindZoneData(findZones[index], zoneIndex);
    }

    FindZoneController.FindZoneData FindNearbyZone(int index) {
        List<int> nearbyZones = ZoneCalculator.GetNearbyZones(zones, neiboringZones, findZones[index].zone, findZones[index].position, findZones[index].range);
        float distance = GetDistanceBetweenPoints(findZones[index].position, zones[nearbyZones[0]].position);
        int zoneIndex = nearbyZones[0];
        for (int i = 1; i < nearbyZones.Count; i++) {
            float newDistance = GetDistanceBetweenPoints(findZones[index].position, zones[nearbyZones[i]].position);
            if (newDistance < distance) {
                distance = newDistance;
                zoneIndex = nearbyZones[i];
            }
        }
        return new FindZoneController.FindZoneData(findZones[index], zoneIndex);
    }

    float GetDistanceBetweenPoints(float3 pos1, float3 pos2) {
        return math.distance(pos1, pos2);
    }
}
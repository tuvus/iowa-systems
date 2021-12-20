using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

public struct FindZonesJob : IJobParallelFor {
    NativeArray<FindZoneController.FindZoneData> findZones;

    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;

    public static JobHandle BeginJob(NativeArray<FindZoneController.FindZoneData> findZones, int findZoneCount, NativeArray<ZoneController.ZoneData> zones) {
        FindZonesJob job = new FindZonesJob { findZones = findZones, zones = zones };
        return IJobParallelForExtensions.Schedule(job, findZoneCount, 1);
    }

    public void Execute(int index) {
        float distance = -1;
        int zoneIndex = -1;
        for (int i = 0; i < zones.Length; i++) {
            float newDistance = GetDistanceBetweenPoints(findZones[index].position, zones[i].position);
            if (newDistance < distance || distance < 0) {
                distance = newDistance;
                zoneIndex = i;
            }
        }
        findZones[index] = new FindZoneController.FindZoneData(findZones[index], zoneIndex);
    }

    public float GetDistanceBetweenPoints(float3 pos1, float3 pos2) {
        return math.distance(pos1, pos2);
    }
}
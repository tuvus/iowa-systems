using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class ZoneController : MonoBehaviour {
    public enum ZoneSetupType {
        Closest = 1,
        Distance = 2,
    }

    Earth earth;
    public FindZoneController FindZoneController { private set; get; }
    public NativeArray<ZoneData> zones;
    public NativeParallelMultiHashMap<int, int> neiboringZones;
    public NativeParallelMultiHashMap<int2, int2> organismsByFoodTypeInZones;

    internal JobHandle job;

    public struct ZoneData {
        public float3 position;
        public float maxSize;
        public float waterDepth;

        public ZoneData(float3 position) {
            this.position = position;
            this.maxSize = -2;
            this.waterDepth = 2;
        }

        public ZoneData(float3 position, float maxSize) {
            this.position = position;
            this.maxSize = maxSize;
            this.waterDepth = 2;
        }
    }

    #region SetupAndWrapup
    public void SetupZoneController(Earth earth) {
        this.earth = earth;
        FindZoneController = GetComponent<FindZoneController>();
        FindZoneController.SetUpJobController(null);
    }

    public void SpawnZones(float radius, int numberOfZones, int maxNeiboringZones, int numberOfPlants, int numberOfAnimals, ZoneSetupType zoneSetup) {
        Allocate(numberOfZones, maxNeiboringZones, numberOfPlants, numberOfAnimals);
        float phi = Mathf.PI * (3 - Mathf.Sqrt(5));
        for (int i = 0; i < numberOfZones; i++) {
            float yPosition = 1 - (i / (float)(numberOfZones - 1)) * 2;
            float tempRadius = Mathf.Sqrt(1 - yPosition * yPosition);

            float theta = phi * i;
            float xPosition = Mathf.Cos(theta) * tempRadius;
            float zPosition = Mathf.Sin(theta) * tempRadius;
            float3 newZonePosition = new float3(new Vector3(xPosition, yPosition, zPosition) * radius / 2);
            zones[i] = new ZoneData(newZonePosition);
        }
        double distance = 4 * math.PI * math.pow(radius, 2) / (2600 * math.log10(.00051 * numberOfZones + .49) + 1000) / (radius / 16);
        NativeArray<float> maxZoneSize = new NativeArray<float>(zones.Length, Allocator.TempJob);
        if (zoneSetup == ZoneSetupType.Distance) {
            JobHandle zoneDistanceJob = FindDistanceNeiboringZones(distance, maxNeiboringZones, maxZoneSize);
            zoneDistanceJob.Complete();
        } else if (zoneSetup == ZoneSetupType.Closest) {
            JobHandle zoneClosestJob = FindClosestNeiboringZones(maxNeiboringZones, maxZoneSize);
            zoneClosestJob.Complete();
        }
        for (int i = 0; i < maxZoneSize.Length; i++) {
            zones[i] = new ZoneData(zones[i].position, maxZoneSize[i]);
        }
        maxZoneSize.Dispose();
    }

    void Allocate(int numberOfZones, int maxNeibroingZones, int numberOfPlants, int numberOfAnimals) {
        zones = new NativeArray<ZoneData>(numberOfZones, Allocator.Persistent);
        neiboringZones = new NativeParallelMultiHashMap<int, int>(numberOfZones * maxNeibroingZones, Allocator.Persistent);
        organismsByFoodTypeInZones = new NativeParallelMultiHashMap<int2, int2>(numberOfPlants * 3, Allocator.Persistent);
    }

    JobHandle FindDistanceNeiboringZones(double distance, int maxNeibroingZones, NativeArray<float> maxZoneSize) {
        job = ZoneSetupJobByDistance.BeginJob(zones, neiboringZones.AsParallelWriter(), maxNeibroingZones, distance, maxZoneSize);
        return job;
    }

    JobHandle FindClosestNeiboringZones(int maxNeibroingZones, NativeArray<float> maxZoneSize) {
        job = ZoneSetupJobByClosest.BeginJob(zones, neiboringZones.AsParallelWriter(), maxNeibroingZones, maxZoneSize);
        return job;
    }

    void OnDestroy() {
        if (neiboringZones.IsCreated)
            neiboringZones.Dispose();
        if (zones.IsCreated)
            zones.Dispose();
        if (organismsByFoodTypeInZones.IsCreated)
            organismsByFoodTypeInZones.Dispose();
    }
    #endregion

    public void AddFoodTypeToZone(int zone, int foodIndex, int2 location) {
        if (zone == -1) {
            Debug.LogError("Problems");
        }
        organismsByFoodTypeInZones.Add(new int2(zone, foodIndex), location);
    }

    public void RemoveFoodTypeFromZone(int zone, int foodIndex, int2 location) {
        if (organismsByFoodTypeInZones.TryGetFirstValue(new int2(zone, foodIndex), out int2 value, out var iterator)) {
            do {
                if (value.x == location.x && value.y == location.y) {
                    organismsByFoodTypeInZones.Remove(iterator);
                    return;
                }
            } while (organismsByFoodTypeInZones.TryGetNextValue(out value, ref iterator));
        }
    }
}
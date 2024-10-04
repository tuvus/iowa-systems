using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class ZoneController : MonoBehaviour {
    public enum ZoneSetupType {
        Closest = 1,
        Distance = 2,
    }

    Earth earth;
    public ZoneData[] zones;
    public Dictionary<ZoneData, HashSet<ZoneData>> neighboringZones;
    public Dictionary<ZoneData, int2> organismsByFoodTypeInZones;

    public class ZoneData {
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
        neighboringZones = new Dictionary<ZoneData, HashSet<ZoneData>>();
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
        float[] maxZoneSize = new float[zones.Length];
        if (zoneSetup == ZoneSetupType.Distance) {
            foreach (var zone in zones) {
                neighboringZones.Add(zone, new HashSet<ZoneData>());
                SetupZoneByDistance(zone, distance, neighboringZones[zone]);
            }
        } else if (zoneSetup == ZoneSetupType.Closest) {
            foreach (var zone in zones) {
                neighboringZones.Add(zone, new HashSet<ZoneData>());
                SetupZoneByClosest(zone, distance, maxNeiboringZones, neighboringZones[zone]);
            }
        }
        for (int i = 0; i < maxZoneSize.Length; i++) {
            zones[i] = new ZoneData(zones[i].position, maxZoneSize[i]);
        }
    }

    void Allocate(int numberOfZones, int maxNeibroingZones, int numberOfPlants, int numberOfAnimals) {
        zones = new ZoneData[numberOfZones];
        neighboringZones = new Dictionary<ZoneData, HashSet<ZoneData>>(numberOfZones * maxNeibroingZones);
    }

    /// <summary>
    /// Sets up this zone's neighboring zones based on the maxNeighboringZones closest zones to this zone.
    /// Writes the output to neighboringZones which is isolated from other operations.
    /// </summary>
    void SetupZoneByClosest(ZoneData zone, double distance, int maxNeighboringZones, HashSet<ZoneData> nearbyZone) {
        Tuple<ZoneData, float>[] tempNeiboringZones = new Tuple<ZoneData, float>[maxNeighboringZones];
        foreach (var zoneToCheck in zones) {
            if (zoneToCheck == zone) continue;
            float distanceToZone = Vector3.Distance(zone.position, zoneToCheck.position);
            if (distanceToZone >= distance) continue;

            int i = tempNeiboringZones.Length - 1;
            for (; i > 0;) {
                i--;
                if (tempNeiboringZones[i] == null) continue;
                if (tempNeiboringZones[i].Item2 <= distanceToZone) {
                    break;
                }
            }
            if (i >= tempNeiboringZones.Length) continue;
            for (int f = tempNeiboringZones.Length - 2; f >= 0; f--) {
                if (f <= i) break;
                if (tempNeiboringZones[f] == null) continue;
                tempNeiboringZones[f + 1] = tempNeiboringZones[f];
            }

            tempNeiboringZones[i] = new Tuple<ZoneData, float>(zoneToCheck, distanceToZone);
        }
        foreach (var neighbor in tempNeiboringZones.Where(n => n != null)) {
            nearbyZone.Add(neighbor.Item1);
        }
    }

    /// <summary>
    /// Sets up this zone's neighboring zones based on all zones within distance to this zone.
    /// Writes the output to neighboringZones which is isolated from other operations.
    /// </summary>
    void SetupZoneByDistance(ZoneData zone, double distance, HashSet<ZoneData> nearbyZOnes) {
        foreach (var newZone in zones) {
            if (zone == newZone) continue;
            float zoneDistance = Vector3.Distance(zone.position, newZone.position);
            if (zoneDistance <= distance) {
                nearbyZOnes.Add(newZone);
            }
        }
    }
    #endregion
}
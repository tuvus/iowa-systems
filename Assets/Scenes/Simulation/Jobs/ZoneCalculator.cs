using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

public static class ZoneCalculator {
    public static List<int> GetNeiboringZones(int zone, NativeMultiHashMap<int, int> neiboringZones) {
        List<int> neiboringZonesList = new List<int>(15);
        if (neiboringZones.TryGetFirstValue(zone, out int value, out var iterator)) {
            do {
                neiboringZonesList.Add(value);
            } while (neiboringZones.TryGetNextValue(out value, ref iterator));
        }
        return neiboringZonesList;
    }

    public static List<int> GetNearbyZones(NativeArray<ZoneController.ZoneData> zones, NativeMultiHashMap<int, int> neiboringZones, int zone, float3 position, float range) {
        List<int> nearbyZones = new List<int>(zones.Length / 5);
        List<int> tempNeiboringZones = new List<int>(50);
        nearbyZones.Add(zone);
        tempNeiboringZones.AddRange(GetNeiboringZones(zone, neiboringZones));
        for (int i = 0; i < tempNeiboringZones.Count; i++) {
            //Debug.Log(range + zones[tempNeiboringZones[i]].maxSize + " >= " + Vector3.Distance(position, zones[tempNeiboringZones[i]].position));
            if (range + zones[tempNeiboringZones[i]].maxSize >= Vector3.Distance(position, zones[tempNeiboringZones[i]].position)) {
                nearbyZones.Add(tempNeiboringZones[i]);
                List<int> newNeiboringZones = GetNeiboringZones(tempNeiboringZones[i], neiboringZones);
                for (int f = 0; f < newNeiboringZones.Count; f++) {
                    if (!tempNeiboringZones.Contains(newNeiboringZones[f])) {
                        tempNeiboringZones.Add(newNeiboringZones[f]);
                    }
                }
            }
        }
        return nearbyZones;
    }

    public static List<int> GetNearbyZonesFromTwoPositions(NativeArray<ZoneController.ZoneData> zones, NativeMultiHashMap<int, int> neiboringZones, int zone, float3x2 positions, float range) {
        List<int> nearbyZones = new List<int>(zones.Length / 5);
        List<int> tempNeiboringZones = new List<int>(50);
        nearbyZones.Add(zone);
        tempNeiboringZones.AddRange(GetNeiboringZones(zone, neiboringZones));
        for (int i = 0; i < tempNeiboringZones.Count; i++) {
            if (range + zones[tempNeiboringZones[i]].maxSize >= Vector3.Distance(positions.c0, zones[tempNeiboringZones[i]].position) || range + zones[tempNeiboringZones[i]].maxSize >= Vector3.Distance(positions.c1, zones[tempNeiboringZones[i]].position)) {
                nearbyZones.Add(tempNeiboringZones[i]);
                List<int> newNeiboringZones = GetNeiboringZones(tempNeiboringZones[i], neiboringZones);
                for (int f = 0; f < newNeiboringZones.Count; f++) {
                    if (!tempNeiboringZones.Contains(newNeiboringZones[f])) {
                        tempNeiboringZones.Add(newNeiboringZones[f]);
                    }
                }
            }
        }
        return nearbyZones;
    }

    public static List<int> GetPlantsInZone(NativeMultiHashMap<int, int> plantsInZones, int zoneNumber) {
        List<int> plants = new List<int>(50);
        if (plantsInZones.TryGetFirstValue(zoneNumber, out int value, out var iterator)) {
            do {
                plants.Add(value);
            } while (plantsInZones.TryGetNextValue(out value, ref iterator));
        }
        return plants;
    }

    public static List<int> GetAnimalsInZone(NativeMultiHashMap<int, int> animalsInZones, int zoneNumber) {
        List<int> animals = new List<int>(50);
        if (animalsInZones.TryGetFirstValue(zoneNumber, out int value, out var iterator)) {
            do {
                animals.Add(value);
            } while (animalsInZones.TryGetNextValue(out value, ref iterator));
        }
        return animals;
    }

    public static List<ZoneController.DataLocation> GetOrganismsInZoneByFoodType(NativeMultiHashMap<int2, ZoneController.DataLocation> organismsByFoodTypeInZones, int zone, int foodTypeIndex) {
        List<ZoneController.DataLocation> organisms = new List<ZoneController.DataLocation>(50);
        if (organismsByFoodTypeInZones.TryGetFirstValue(new int2(zone, foodTypeIndex), out ZoneController.DataLocation value, out var iterator)) {
            do {
                organisms.Add(value);
            } while (organismsByFoodTypeInZones.TryGetNextValue(out value, ref iterator));
        }
        return organisms;
    }
}
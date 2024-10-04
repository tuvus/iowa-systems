using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using ZoneData = ZoneController.ZoneData;

public static class ZoneCalculator {

    /// <summary>
    /// Searches for the closest zone not using any information of neighboring zones  
    /// </summary>
    public static ZoneData FindZone(List<ZoneData> zones, Species.Organism organism) {
        float distance = -1;
        ZoneData closestZone = null;
        foreach (var zone in zones) {
            float newDistance = GetDistanceBetweenPoints(organism.position, zone.position);
            if (newDistance < distance || distance < 0) {
                distance = newDistance;
                closestZone = zone;
            }
        }

        return closestZone;
    }

    /// <summary>
    /// Returns the closest zone using neighboring zone data.
    /// </summary>
    static ZoneData GetNearestZone(Dictionary<ZoneData, HashSet<ZoneData>> neiboringZones, Vector3 position, ZoneData currentZone) {
        List<ZoneData> zonesToCheck = new List<ZoneData> { currentZone };
        zonesToCheck.AddRange(neiboringZones[currentZone]);

        float distance = -1;
        ZoneData closestZone = currentZone;
        foreach (var zone in zonesToCheck) {
            float newDistance = GetDistanceBetweenPoints(position, zone.position);
            if (newDistance < distance || distance < 0) {
                distance = newDistance;
                closestZone = zone;
            }
        }

        return closestZone;
    }

    /// <summary>
    /// Returns all nearby zones within a certain radius using neighboring zone data.
    /// </summary>
    public static List<ZoneData> GetNearbyZones(Dictionary<ZoneData, HashSet<ZoneData>> neiboringZones, ZoneData currentZone, float3 position, float range) {
        List<ZoneData> zonesToCheck = new List<ZoneData> { currentZone };
        HashSet<ZoneData> zonesChecked = new HashSet<ZoneData>(zonesToCheck);
        List<ZoneData> nearbyZones = new List<ZoneData>();
        
        while (zonesToCheck.Count != 0) {
            ZoneData zone = zonesToCheck.First();
            zonesToCheck.RemoveAt(0);
            if (range + zone.maxSize <= Vector3.Distance(position, zone.position)) {
                nearbyZones.Add(zone);
                foreach (var newZone in neiboringZones[zone]) {
                    if (!zonesChecked.Contains(newZone)) {
                        zonesToCheck.Add(newZone);
                        zonesChecked.Add(newZone);
                    }
                }
            }
        }
        return nearbyZones;
    }

    static float GetDistanceBetweenPoints(float3 pos1, float3 pos2) {
        return math.distance(pos1, pos2);
    }
}
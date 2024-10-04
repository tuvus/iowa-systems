using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Mathematics;

public class FindZoneController {

    private FindZoneData[] findZones;
    private int findZoneCount;

    public struct FindZoneData {
        public int2 organismLocation;
        public float range;
        public int zone;

        public FindZoneData(int2 organismLocation, int zone, float range) {
            this.organismLocation = organismLocation;
            this.range = range;
            this.zone = zone;
        }

        public FindZoneData(FindZoneData oldData, int zone) {
            this.organismLocation = oldData.organismLocation;
            this.range = oldData.range;
            this.zone = zone;
        }
    }
}
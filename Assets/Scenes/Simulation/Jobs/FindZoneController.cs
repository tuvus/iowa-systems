using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class FindZoneController : JobController {

    private NativeArray<FindZoneData> findZones;
    private int findZoneCount;

    public struct FindZoneData {
        public ZoneController.DataLocation dataLocation;
        public float3 position;
        public float range;
        public int zone;

        public FindZoneData(ZoneController.DataLocation dataLocation, int zone, float3 position, float range) {
            this.dataLocation = dataLocation;
            this.position = position;
            this.range = range;
            this.zone = zone;
        }

        public FindZoneData(FindZoneData oldData, int zone) {
            this.dataLocation = oldData.dataLocation;
            this.position = oldData.position;
            this.range = oldData.range;
            this.zone = zone;
        }
    }

    public override JobHandle StartUpdateJob() {
        job = FindZonesJob.BeginJob(findZones, findZoneCount, GetEarth().GetZoneController().zones, GetEarth().GetZoneController().neiboringZones);
        return job;
    }

    public void CompleteZoneJob() {
        for (int i = 0; i < findZoneCount; i++) {
            if (findZones[i].zone == -1)
                Debug.LogError("error");
            GetEarth().GetZoneController().GetOrganismFromDataLocation(findZones[i].dataLocation).SetOrganismZone(findZones[i].zone);
        }
        findZoneCount = 0;
    }

    public void AddFindZoneData(FindZoneData findZoneData) {
        if (findZones.Length <= findZoneCount)
            IncreaceFindZoneLength(1);
        findZones[findZoneCount] = findZoneData;
        findZoneCount++;
    }

    void IncreaceFindZoneLength(int length) {
        NativeArray<FindZoneData> oldFindZone = findZones;
        findZones = new NativeArray<FindZoneData>(oldFindZone.Length + length, Allocator.Persistent);
        NativeArray<FindZoneData>.Copy(oldFindZone,findZones,findZoneCount);
        oldFindZone.Dispose();
    }

    public override void Allocate() {
        findZones = new NativeArray<FindZoneData>(SpeciesManager.Instance.GetAllStartingPlantsAndSeeds() + SpeciesManager.Instance.GetAllStartingAnimals(), Allocator.Persistent);
    }

    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();

        findZones.Dispose();
    }
}

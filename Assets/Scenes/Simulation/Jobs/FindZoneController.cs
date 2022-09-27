using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class FindZoneController : JobController {

    private NativeArray<FindZoneData> findZones;
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

    public override JobHandle StartUpdateJob() {
        job = FindZonesJob.BeginJob(findZones, findZoneCount, GetEarth().GetZoneController().zones, GetEarth().GetZoneController().neiboringZones);
        return job;
    }

    public void CompleteZoneJob() {
        for (int i = 0; i < findZoneCount; i++) {
            if (findZones[i].zone == -1)
                Debug.LogError("error");
            GetEarth().GetAllSpecies()[findZones[i].organismLocation.x].organisms[findZones[i].organismLocation.y] = new Species.Organism(GetEarth().GetAllSpecies()[findZones[i].organismLocation.x].organisms[findZones[i].organismLocation.y], findZones[i].zone);
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
        NativeArray<FindZoneData>.Copy(oldFindZone, findZones, findZoneCount);
        oldFindZone.Dispose();
    }

    public override void Allocate() {
        findZones = new NativeArray<FindZoneData>(SpeciesManager.Instance.GetAllStartingPlantsAndSeeds() + SpeciesManager.Instance.GetAllStartingAnimals(), Allocator.Persistent);
    }

    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();
        if (findZones.IsCreated)
            findZones.Dispose();
    }
}
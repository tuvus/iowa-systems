using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class FindZoneController : BasicJobController {

    public NativeArray<FindZoneData> findZones;
    public int findZoneCount;

    public struct FindZoneData {
        public int speciesIndex;
        public int organismIndex;
        public float3 position;
        public float range;
        public int zone;

        public FindZoneData(int speciesIndex, int organismIndex,float3 position, float range) {
            this.speciesIndex = speciesIndex;
            this.organismIndex = organismIndex;
            this.position = position;
            this.range = range;
            zone = -1;
        }

        public FindZoneData(FindZoneData oldData, int zone) {
            this.speciesIndex = oldData.speciesIndex;
            this.organismIndex = oldData.organismIndex;
            this.position = oldData.position;
            this.range = oldData.range;
            this.zone = zone;
        }
    }

    public override JobHandle StartUpdateJob() {
        job = FindZonesJob.BeginJob(findZones, findZoneCount, earth.GetZoneController().zones);
        return job;
    }

    public void CompleteZoneJob() {
        for (int i = 0; i < findZoneCount; i++) {
            SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[findZones[i].speciesIndex].SetOrganismZone(findZones[i].organismIndex, findZones[i].zone);
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
        findZones = new NativeArray<FindZoneData>(SpeciesManager.Instance.GetAllStartingPlantsAndSeeds(), Allocator.Persistent);
    }

    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();

        findZones.Dispose();
    }
}

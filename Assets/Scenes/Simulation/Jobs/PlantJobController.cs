using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class PlantJobController : JobController {
    public NativeArray<float2> plantReasourceGain;
    public NativeArray<Plant.GrowthStage> plantGrowthStage;
    NativeArray<int> updatePlants;
    

    JobHandle zoneJob;

    public override JobHandle StartUpdateJob() {
        SetUpdateNativeArrays();
        job = PlantUpdateJob.BeginJob(plantReasourceGain,plantGrowthStage,updatePlants,GetPlantSpecies().GetActivePlantCount(),GetSpecies().GetEarth().GetZoneController().allPlants,
            GetSpecies().GetEarth().GetZoneController().zones,GetSpecies().GetEarth().GetZoneController().neiboringZones, GetSpecies().GetEarth().GetZoneController().plantsInZones,
            GetSpecies().GetEarth().earthState,GetPlantSpecies().growthStages,GetPlantSpecies().GetSpeciesSeeds().seedGerminationRequirement);
        return job;
    }

    void SetUpdateNativeArrays() {
        if (GetPlantSpecies().GetActivePlantCount() > updatePlants.Length)
            SetUpdatePlantsLength(GetPlantSpecies().GetActivePlantCount() * 2);
        for (int i = 0; i < GetPlantSpecies().GetActivePlantCount(); i++) {
            updatePlants[i] = GetPlantSpecies().GetPlant(GetPlantSpecies().GetActivePlant(i)).plantDataIndex;
        }
    }

    void SetUpdatePlantsLength(int length) {
        updatePlants.Dispose();
        updatePlants = new NativeArray<int>(length, Allocator.Persistent);

        plantReasourceGain.Dispose(); 
        plantReasourceGain = new NativeArray<float2>(length, Allocator.Persistent);

        plantGrowthStage.Dispose();
        plantGrowthStage = new NativeArray<Plant.GrowthStage>(length, Allocator.Persistent);
    }

    public override void Allocate() {
        plantReasourceGain = new NativeArray<float2>(500, Allocator.Persistent);
        plantGrowthStage = new NativeArray<Plant.GrowthStage>(500, Allocator.Persistent);
        updatePlants = new NativeArray<int>(500, Allocator.Persistent);
    }
    
    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();

        plantReasourceGain.Dispose();
        plantGrowthStage.Dispose();
        updatePlants.Dispose();

        zoneJob.Complete();
    }

    public PlantSpecies GetPlantSpecies() {
        return (PlantSpecies)GetSpecies();
    }
}

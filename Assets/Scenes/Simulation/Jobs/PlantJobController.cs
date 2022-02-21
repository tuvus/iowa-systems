using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class PlantJobController : BasicJobController {
    PlantSpecies plantSpecies;

    public NativeArray<float2> plantReasourceGain;
    public NativeArray<PlantScript.GrowthStage> plantGrowthStage;
    NativeArray<int> updatePlants;
    

    JobHandle zoneJob;
    internal override void SetUpSpecificJobController(BasicSpeciesScript speciesScript) {
        plantSpecies = (PlantSpecies)speciesScript;
    }

    public override JobHandle StartUpdateJob() {
        SetUpdateNativeArrays();
        job = PlantUpdateJob.BeginJob(plantReasourceGain,plantGrowthStage,updatePlants,plantSpecies.GetActivePlantCount(),earth.GetZoneController().allPlants,
            earth.GetZoneController().zones,earth.GetZoneController().neiboringZones, earth.GetZoneController().plantsInZones,
            earth.earthState,plantSpecies.growthStages,plantSpecies.GetSpeciesSeeds().seedGerminationRequirement);
        return job;
    }

    void SetUpdateNativeArrays() {
        if (plantSpecies.GetActivePlantCount() > updatePlants.Length)
            SetUpdatePlantsLength(plantSpecies.GetActivePlantCount() * 2);
        for (int i = 0; i < plantSpecies.GetActivePlantCount(); i++) {
            updatePlants[i] = plantSpecies.GetPlant(plantSpecies.GetActivePlant(i)).plantDataIndex;
        }
    }

    void SetUpdatePlantsLength(int length) {
        updatePlants.Dispose();
        updatePlants = new NativeArray<int>(length, Allocator.Persistent);

        plantReasourceGain.Dispose(); 
        plantReasourceGain = new NativeArray<float2>(length, Allocator.Persistent);

        plantGrowthStage.Dispose();
        plantGrowthStage = new NativeArray<PlantScript.GrowthStage>(length, Allocator.Persistent);
    }

    public override void Allocate() {
        plantReasourceGain = new NativeArray<float2>(500, Allocator.Persistent);
        plantGrowthStage = new NativeArray<PlantScript.GrowthStage>(500, Allocator.Persistent);
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
}

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
    public NativeArray<int2> plantZones;
    public int plantFindZoneCount;
    internal override void SetUpSpecificJobController(BasicSpeciesScript speciesScript) {
        plantSpecies = (PlantSpecies)speciesScript;
    }

    public override JobHandle StartUpdateJob() {
        SetUpdateNativeArrays();
        job = PlantUpdateJob.BeginJob(plantReasourceGain,plantGrowthStage,updatePlants,plantSpecies.GetActivePlants().Count,earth.GetZoneController().allPlants,
            earth.GetZoneController().zones,earth.GetZoneController().neiboringZones, earth.GetZoneController().plantsInZones,
            earth.earthState,plantSpecies.growthStages,plantSpecies.GetSpeciesSeeds().seedGerminationRequirement);
        return job;
    }

    void SetUpdateNativeArrays() {
        int activePlantCount = plantSpecies.GetActivePlants().Count;
        if (activePlantCount > updatePlants.Length)
            IncreaceUpdatePlantsLength(activePlantCount - updatePlants.Length);
        for (int i = 0; i < activePlantCount; i++) {
            updatePlants[i] = plantSpecies.GetActivePlants()[i];
        }
    }

    void IncreaceUpdatePlantsLength(int length) {
        NativeArray<int> oldUpdatePlants = updatePlants;
        updatePlants = new NativeArray<int>(oldUpdatePlants.Length + length, Allocator.Persistent);
        NativeArray<int>.Copy(oldUpdatePlants, updatePlants, oldUpdatePlants.Length);
        oldUpdatePlants.Dispose();
        NativeArray<float2> oldPlantReasourceGain = plantReasourceGain;
        plantReasourceGain = new NativeArray<float2>(oldPlantReasourceGain.Length + length, Allocator.Persistent);
        NativeArray<float2>.Copy(oldPlantReasourceGain, plantReasourceGain, oldPlantReasourceGain.Length);
        oldPlantReasourceGain.Dispose(); 
        NativeArray<PlantScript.GrowthStage> oldPlantGrowthStage = plantGrowthStage;
        plantGrowthStage = new NativeArray<PlantScript.GrowthStage>(oldPlantGrowthStage.Length + length, Allocator.Persistent);
        NativeArray<PlantScript.GrowthStage>.Copy(oldPlantGrowthStage, plantGrowthStage, oldPlantGrowthStage.Length);
        oldPlantGrowthStage.Dispose();
    }

    void SetFindZoneNativeArrays() {
        int plantCount = plantSpecies.GetActivePlants().Count;
        for (int i = 0; i < plantCount; i++) {
            PlantScript plant = plantSpecies.GetPlants()[plantSpecies.GetActivePlants()[i]];
            if (plant.zone == -1 || plant.zone == -2) {
                plantZones[plantFindZoneCount] = new int2(i, -1);
                plantFindZoneCount++;
            }
        }
    }

    public override void Allocate() {
        plantReasourceGain = new NativeArray<float2>(500, Allocator.Persistent);
        plantGrowthStage = new NativeArray<PlantScript.GrowthStage>(500, Allocator.Persistent);
        updatePlants = new NativeArray<int>(500, Allocator.Persistent);
        plantZones = new NativeArray<int2>(500, Allocator.Persistent);
    }
    
    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();

        plantReasourceGain.Dispose();
        plantGrowthStage.Dispose();
        updatePlants.Dispose();

        zoneJob.Complete();
        plantZones.Dispose();
    }
}

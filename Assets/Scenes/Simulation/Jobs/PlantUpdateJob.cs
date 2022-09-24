using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

public struct PlantUpdateJob : IJobParallelFor {
    [WriteOnly] NativeArray<float2> plantReasourceGain;
    [WriteOnly] NativeArray<PlantSpecies.GrowthStage> plantGrowthStage;
    [ReadOnly] NativeArray<int> updatePlants;

    [ReadOnly] NativeArray<Plant.PlantData> allPlants;
    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;
    [ReadOnly] NativeParallelMultiHashMap<int, int> neiboringZones;
    [ReadOnly] NativeParallelMultiHashMap<int, int> plantsInZones;

    [ReadOnly] Earth.EarthState earthState;
    [ReadOnly] NativeArray<PlantSpecies.GrowthStageData> growthStages;
    [ReadOnly] PlantSpeciesSeeds.SeedGerminationRequirement seedGerminationRequirement;


    public static JobHandle BeginJob(NativeArray<float2> plantReasourceGain, NativeArray<PlantSpecies.GrowthStage> plantGrowthStage, 
        NativeArray<int> plants, int plantCount, NativeArray<Plant.PlantData> allPlants , NativeArray<ZoneController.ZoneData> zones, 
        NativeParallelMultiHashMap<int, int> neiboringZones, NativeParallelMultiHashMap<int, int> plantsInZones, Earth.EarthState earthState, 
        NativeArray<PlantSpecies.GrowthStageData> growthStages, PlantSpeciesSeeds.SeedGerminationRequirement seedGerminationRequirement) {
        PlantUpdateJob job = new PlantUpdateJob { plantReasourceGain = plantReasourceGain, plantGrowthStage = plantGrowthStage, 
            updatePlants = plants, allPlants = allPlants, zones = zones, neiboringZones = neiboringZones, plantsInZones = plantsInZones, earthState = earthState, 
            growthStages = growthStages, seedGerminationRequirement = seedGerminationRequirement, };
        return IJobParallelForExtensions.Schedule(job, plantCount, 1);
    }

    public void Execute(int plantIndex) {
        Plant.PlantData plant = allPlants[updatePlants[plantIndex]];
        if (plant.growthStage == PlantSpecies.GrowthStage.Dead) {
            return;
        }
        if (plant.zone == -1) {
            Debug.LogError("The zone of this plant should not be " + plant.zone + ". Curent growth stage is " + plant.growthStage);
            return;
        }
        if (plant.growthStage == PlantSpecies.GrowthStage.Seed) {
            plantGrowthStage[plantIndex] = GetSeedGerminationResults(plant);
            return;
        }
        plantReasourceGain[plantIndex] = new float2(GetSunGain(plant), GetWaterGain(plant));
        plantGrowthStage[plantIndex] = GetGrowthStage(plant);
    }

    PlantSpecies.GrowthStage GetSeedGerminationResults(Plant.PlantData plant) {
        if (plant.age > seedGerminationRequirement.timeMaximum)
            return PlantSpecies.GrowthStage.Dead;
        if (plant.age > seedGerminationRequirement.timeRequirement && earthState.humidity > seedGerminationRequirement.humidityRequirement && earthState.temperature > seedGerminationRequirement.tempetureRequirement)
            return PlantSpecies.GrowthStage.Germinating;
        return PlantSpecies.GrowthStage.Seed;
    }



    float GetDistanceToPlant(Plant.PlantData from, Plant.PlantData to) {
        return math.distance(from.position, to.position);
    }

    float GetRootSize(Plant.PlantData plant) {
        return plant.rootGrowth.x;
    }

    PlantSpecies.GrowthStage GetGrowthStage(Plant.PlantData plant) {
        int stageIndex = (int)plant.growthStage;
        if (stageIndex == growthStages.Length - 1)
            return plant.growthStage;
        if (plant.bladeArea >= growthStages[stageIndex].bladeArea && plant.stemHeight >= growthStages[stageIndex].stemHeight && plant.rootGrowth.y >= growthStages[stageIndex].rootGrowth.y) {
            return growthStages[stageIndex + 1].stage;
        }
        return plant.growthStage;
    }
}
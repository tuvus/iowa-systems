using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

public struct PlantUpdateJob : IJobParallelFor {
    [WriteOnly] NativeArray<float2> plantReasourceGain;
    [WriteOnly] NativeArray<PlantScript.GrowthStage> plantGrowthStage;
    [ReadOnly] NativeArray<int> updatePlants;

    [ReadOnly] NativeArray<PlantScript.PlantData> allPlants;
    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;
    [ReadOnly] NativeMultiHashMap<int, int> neiboringZones;
    [ReadOnly] NativeMultiHashMap<int, int> plantsInZones;

    [ReadOnly] EarthScript.EarthState earthState;
    [ReadOnly] NativeArray<PlantSpecies.GrowthStageData> growthStages;
    [ReadOnly] PlantSpeciesSeeds.SeedGerminationRequirement seedGerminationRequirement;


    public static JobHandle BeginJob(NativeArray<float2> plantReasourceGain, NativeArray<PlantScript.GrowthStage> plantGrowthStage, 
        NativeArray<int> plants, int plantCount, NativeArray<PlantScript.PlantData> allPlants , NativeArray<ZoneController.ZoneData> zones, 
        NativeMultiHashMap<int, int> neiboringZones, NativeMultiHashMap<int, int> plantsInZones, EarthScript.EarthState earthState, 
        NativeArray<PlantSpecies.GrowthStageData> growthStages, PlantSpeciesSeeds.SeedGerminationRequirement seedGerminationRequirement) {
        PlantUpdateJob job = new PlantUpdateJob { plantReasourceGain = plantReasourceGain, plantGrowthStage = plantGrowthStage, 
            updatePlants = plants, allPlants = allPlants, zones = zones, neiboringZones = neiboringZones, plantsInZones = plantsInZones, earthState = earthState, 
            growthStages = growthStages, seedGerminationRequirement = seedGerminationRequirement, };
        return IJobParallelForExtensions.Schedule(job, plantCount, 1);
    }

    public void Execute(int plantIndex) {
        PlantScript.PlantData plant = allPlants[updatePlants[plantIndex]];
        if (plant.stage == PlantScript.GrowthStage.Dead) {
            return;
        }
        if (plant.zone == -1) {
            Debug.LogError("The zone of this plant should not be " + plant.zone + ". Curent growth stage is " + plant.stage);
            return;
        }
        if (plant.stage == PlantScript.GrowthStage.Seed) {
            plantGrowthStage[plantIndex] = GetSeedGerminationResults(plant);
            return;
        }
        plantReasourceGain[plantIndex] = new float2(GetSunGain(plant), GetWaterGain(plant));
        plantGrowthStage[plantIndex] = GetGrowthStage(plant);
    }

    PlantScript.GrowthStage GetSeedGerminationResults(PlantScript.PlantData plant) {
        if (plant.age > seedGerminationRequirement.timeMaximum)
            return PlantScript.GrowthStage.Dead;
        if (plant.age > seedGerminationRequirement.timeRequirement && earthState.humidity > seedGerminationRequirement.humidityRequirement && earthState.temperature > seedGerminationRequirement.tempetureRequirement)
            return PlantScript.GrowthStage.Germinating;
        return PlantScript.GrowthStage.Seed;
    }

    float GetSunGain(PlantScript.PlantData plant) {
        return plant.bladeArea * GetSunValue(plant.position) / 2;
    }

    public float GetSunValue(float3 position) {
        if (SimulationScript.Instance.sunRotationEffect) {
            float objectDistanceFromSun = Vector3.Distance(position, earthState.sunPostion);
            float sunDistanceFromEarth = Vector3.Distance(new float3(0, 0, 0), earthState.sunPostion);
            float sunValue = (objectDistanceFromSun - sunDistanceFromEarth) / earthState.earthRadius * 2;
            if (sunValue < 0)
                return 0;
            return sunValue;
        } else {
            return 0.5f;
        }
    }

    float GetWaterGain(PlantScript.PlantData plant) {
        float rootArea = (math.PI * plant.rootGrowth.x * plant.rootGrowth.y) + (math.pow(plant.rootGrowth.x / 2, 2) * 2);
        float overLapingRootArea = 0f;
        if (plantsInZones.TryGetFirstValue(plant.zone, out int targetPlant, out var iterator)) {
            do {
                float distance = GetDistanceToPlant(plant, allPlants[targetPlant]);
                float rootDistance = GetRootSize(plant) + GetRootSize(allPlants[targetPlant]);
                if (distance < rootDistance) {
                    float reletiveDepth = (plant.rootGrowth.y - allPlants[targetPlant].rootGrowth.y) / plant.rootGrowth.y;
                    float reletiveDistance = rootDistance - distance;
                    overLapingRootArea += (reletiveDistance / 2) / (1 + reletiveDepth);
                }
            } while (plantsInZones.TryGetNextValue(out targetPlant, ref iterator));
        }
        int zoneNumber;
        if (neiboringZones.TryGetFirstValue(plant.zone, out zoneNumber, out var iterator2)) {
            do {
                if (plantsInZones.TryGetFirstValue(zoneNumber, out targetPlant, out var iterator3)) {
                    do {
                        float distance = GetDistanceToPlant(plant, allPlants[targetPlant]);
                        float rootDistance = GetRootSize(plant) + GetRootSize(allPlants[targetPlant]);
                        if (distance < rootDistance) {
                            float reletiveDepth = (plant.rootGrowth.y - allPlants[targetPlant].rootGrowth.y) / plant.rootGrowth.y;
                            float reletiveDistance = rootDistance - distance;
                            overLapingRootArea += (reletiveDistance / 2) / (1 + reletiveDepth);
                        }
                    } while (plantsInZones.TryGetNextValue(out targetPlant, ref iterator3));
                }
            } while (neiboringZones.TryGetNextValue(out zoneNumber, ref iterator2));
        }

        rootArea = rootArea - overLapingRootArea;
        float rootUnderWaterPercent = 1 - (zones[plant.zone].waterDepth / plant.rootGrowth.y);
        if (rootUnderWaterPercent < 0) {
            return 0;
        }
        return rootUnderWaterPercent * rootArea * plant.rootDensity * .01f;
    }

    float GetDistanceToPlant(PlantScript.PlantData from, PlantScript.PlantData to) {
        return math.distance(from.position, to.position);
    }

    float GetRootSize(PlantScript.PlantData plant) {
        return plant.rootGrowth.x;
    }

    PlantScript.GrowthStage GetGrowthStage(PlantScript.PlantData plant) {
        int stageIndex = (int)plant.stage;
        if (stageIndex == growthStages.Length - 1)
            return plant.stage;
        if (plant.bladeArea >= growthStages[stageIndex].bladeArea && plant.stemHeight >= growthStages[stageIndex].stemHeight && plant.rootGrowth.y >= growthStages[stageIndex].rootDepth) {
            return growthStages[stageIndex + 1].stage;
        }
        return plant.stage;
    }
}
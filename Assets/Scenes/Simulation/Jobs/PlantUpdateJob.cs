using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;

public struct PlantUpdateJob : IJobParallelFor {
    [WriteOnly] NativeArray<float2> plantReasourceGain;
    [WriteOnly] NativeArray<Plant.GrowthStage> plantGrowthStage;
    [ReadOnly] NativeArray<int> updatePlants;

    [ReadOnly] NativeArray<Plant.PlantData> allPlants;
    [ReadOnly] NativeArray<ZoneController.ZoneData> zones;
    [ReadOnly] NativeParallelMultiHashMap<int, int> neiboringZones;
    [ReadOnly] NativeParallelMultiHashMap<int, int> plantsInZones;

    [ReadOnly] Earth.EarthState earthState;
    [ReadOnly] NativeArray<PlantSpecies.GrowthStageData> growthStages;
    [ReadOnly] PlantSpeciesSeeds.SeedGerminationRequirement seedGerminationRequirement;


    public static JobHandle BeginJob(NativeArray<float2> plantReasourceGain, NativeArray<Plant.GrowthStage> plantGrowthStage, 
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
        if (plant.growthStage == Plant.GrowthStage.Dead) {
            return;
        }
        if (plant.zone == -1) {
            Debug.LogError("The zone of this plant should not be " + plant.zone + ". Curent growth stage is " + plant.growthStage);
            return;
        }
        if (plant.growthStage == Plant.GrowthStage.Seed) {
            plantGrowthStage[plantIndex] = GetSeedGerminationResults(plant);
            return;
        }
        plantReasourceGain[plantIndex] = new float2(GetSunGain(plant), GetWaterGain(plant));
        plantGrowthStage[plantIndex] = GetGrowthStage(plant);
    }

    Plant.GrowthStage GetSeedGerminationResults(Plant.PlantData plant) {
        if (plant.age > seedGerminationRequirement.timeMaximum)
            return Plant.GrowthStage.Dead;
        if (plant.age > seedGerminationRequirement.timeRequirement && earthState.humidity > seedGerminationRequirement.humidityRequirement && earthState.temperature > seedGerminationRequirement.tempetureRequirement)
            return Plant.GrowthStage.Germinating;
        return Plant.GrowthStage.Seed;
    }

    float GetSunGain(Plant.PlantData plant) {
        return plant.bladeArea * GetSunValue(plant.position);
    }

    public float GetSunValue(float3 position) {
        if (Simulation.Instance.sunRotationEffect) {
            float objectDistanceFromSun = Vector3.Distance(position, earthState.sunPostion);
            float sunDistanceFromEarth = Vector3.Distance(new float3(0, 0, 0), earthState.sunPostion);
            float sunValue = (objectDistanceFromSun - sunDistanceFromEarth) / earthState.earthRadius * 2;
            return Mathf.Max(sunValue,0);
        } else {
            return 0.5f;
        }
    }

    float GetWaterGain(Plant.PlantData plant) {
        float rootArea = (math.PI * plant.rootGrowth.x * plant.rootGrowth.y) + (math.pow(plant.rootGrowth.x / 2, 2) * 2);
        float overLapingRootArea = 0f;
        if (plantsInZones.TryGetFirstValue(plant.zone, out int targetPlant, out var iterator)) {
            do {
                    Debug.Log(1);
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
                            Debug.Log(2);
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

        rootArea = rootArea - (overLapingRootArea / 2);
        float rootUnderWaterPercent = 1 - (zones[plant.zone].waterDepth / plant.rootGrowth.y);
        return Mathf.Max(rootUnderWaterPercent * rootArea * plant.rootDensity * .01f,0);
    }

    float GetDistanceToPlant(Plant.PlantData from, Plant.PlantData to) {
        return math.distance(from.position, to.position);
    }

    float GetRootSize(Plant.PlantData plant) {
        return plant.rootGrowth.x;
    }

    Plant.GrowthStage GetGrowthStage(Plant.PlantData plant) {
        int stageIndex = (int)plant.growthStage;
        if (stageIndex == growthStages.Length - 1)
            return plant.growthStage;
        if (plant.bladeArea >= growthStages[stageIndex].bladeArea && plant.stemHeight >= growthStages[stageIndex].stemHeight && plant.rootGrowth.y >= growthStages[stageIndex].rootGrowth.y) {
            return growthStages[stageIndex + 1].stage;
        }
        return plant.growthStage;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class ZoneController : MonoBehaviour {
    EarthScript earth;
    public FindZoneController FindZoneController { private set; get; }
    public NativeArray<ZoneData> zones;
    public NativeArray<PlantScript.PlantData> allPlants;
    public int allPlantsCount;
    public NativeMultiHashMap<int, int> neiboringZones;
    public NativeMultiHashMap<int, int> plantsInZones;
    public NativeMultiHashMap<int2, int> organismsByFoodTypeInZones;

    internal JobHandle job;

    public struct ZoneData {
        public float3 position;
        public float waterDepth;

        public ZoneData(float3 position, int organismByTypeSize) {
            this.position = position;
            this.waterDepth = 2;
        }
    }

    #region SetupAndWrapup
    public void SetupZoneController(EarthScript earth) {
        this.earth = earth;
        FindZoneController = GetComponent<FindZoneController>();
        FindZoneController.SetUpJobController(earth);
    }
    
    public void SpawnZones(float radius, int numberOfZones, int maxNeiboringZones, int numberOfPlants) {
        Allocate(numberOfZones,maxNeiboringZones,numberOfPlants);
        float phi = Mathf.PI * (3 - Mathf.Sqrt(5));
        for (int i = 0; i < numberOfZones; i++) {
            float yPosition = 1 - (i / (float)(numberOfZones - 1)) * 2;
            float tempRadius = Mathf.Sqrt(1 - yPosition * yPosition);

            float theta = phi * i;
            float xPosition = Mathf.Cos(theta) * tempRadius;
            float zPosition = Mathf.Sin(theta) * tempRadius;
            float3 newZonePosition = new float3(new Vector3(xPosition, yPosition, zPosition) * radius);
            zones[i] = new ZoneData(newZonePosition, 100);
        }
        float distance = 350 / Mathf.Sqrt(numberOfZones);
        JobHandle neiboringZonesJob = FindNeiboringZones(distance,maxNeiboringZones);
        neiboringZonesJob.Complete();
    }

    void Allocate(int numberOfZones,int maxNeibroingZones,int numberOfPlants) {
        zones = new NativeArray<ZoneData>(numberOfZones, Allocator.Persistent);
        allPlants = new NativeArray<PlantScript.PlantData>(SpeciesManager.Instance.GetAllStartingPlantsAndSeeds() * 5, Allocator.Persistent);
        neiboringZones = new NativeMultiHashMap<int, int>(numberOfZones * maxNeibroingZones, Allocator.Persistent);
        plantsInZones = new NativeMultiHashMap<int, int>(numberOfPlants, Allocator.Persistent);
        organismsByFoodTypeInZones = new NativeMultiHashMap<int2, int>(numberOfPlants * 3, Allocator.Persistent);
    }

    JobHandle FindNeiboringZones(float distance, int maxNeibroingZones) {
        job = ZoneUpdateJob.BeginJob(zones, neiboringZones.AsParallelWriter(), maxNeibroingZones, distance);
        return job;
    }
   
    void OnDestroy() {
        organismsByFoodTypeInZones.Dispose();
        neiboringZones.Dispose();
        zones.Dispose();
        allPlants.Dispose();
        plantsInZones.Dispose();
    }
    #endregion

    #region Runtime
    public void AddPlants(List<PlantScript> plants) {
        for (int i = 0; i < plants.Count; i++) {
            AddPlant(plants[i]);
        }
    }

    public void AddPlant(PlantScript plant) {
        if (allPlantsCount == allPlants.Length)
            IncreaceAllPlantLength(1);
        plant.plantDataIndex = allPlantsCount;
        allPlantsCount++;
    }

    public void IncreaceAllPlantLength(int length) {
        NativeArray<PlantScript.PlantData> oldAllPlants = allPlants;
        allPlants = new NativeArray<PlantScript.PlantData>(allPlants.Length + length, Allocator.Persistent);
        for (int i = 0; i < oldAllPlants.Length; i++) {
            allPlants[i] = oldAllPlants[i];
        }
        oldAllPlants.Dispose();
    }

    public long GetFoodTypeWithZoneKey(int zoneNumber, string foodType) {
        int foodIndex = earth.GetIndexOfFoodType(foodType);
        long key = zoneNumber * Mathf.FloorToInt(Mathf.Log10(foodIndex)) + foodIndex;
        print(zoneNumber + " " + foodIndex + " " + key);
        return key;
    }

    List<PlantScript> GetPlantsInZone(int zoneNumber) {
        List<PlantScript> plants = new List<PlantScript>();
        NativeArray<int> plantValues = plantsInZones.GetValueArray(Allocator.Temp);
        for (int i = 0; i < plantValues.Length; i++) {
            plants.Add(GetPlantFromValue(allPlants[plantValues[i]]));
        }
        return plants;
    }

    PlantScript GetPlantFromValue(PlantScript.PlantData plant) {
        return GetPlantSpeciesFromPlant(plant).GetPlants()[plant.plantIndex];
    }

    PlantSpecies GetPlantSpeciesFromPlant(PlantScript.PlantData plant) {
        return SpeciesManager.Instance.GetSpeciesMotor().GetAllPlantSpecies()[plant.speciesIndex];
    }

    public int2 GetZoneKeyFromZoneNumberAndFoodType(int zoneNumber, string foodType) {
        return new int2(zoneNumber, earth.GetIndexOfFoodType(foodType));
    }
    #endregion
}

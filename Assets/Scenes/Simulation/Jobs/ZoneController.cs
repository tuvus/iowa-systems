using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class ZoneController : MonoBehaviour {
    public enum ZoneSetupType {
        Closest = 1,
        Distance = 2,
    }

    EarthScript earth;
    public FindZoneController FindZoneController { private set; get; }
    public NativeArray<ZoneData> zones;
    public NativeArray<PlantScript.PlantData> allPlants;
    public int allPlantsCount;
    public NativeArray<AnimalScript.AnimalData> allAnimals;
    public int allAnimalsCount;
    public NativeMultiHashMap<int, int> neiboringZones;
    public NativeMultiHashMap<int, int> plantsInZones;
    public NativeMultiHashMap<int, int> animalsInZones;
    public NativeMultiHashMap<int2, DataLocation> organismsByFoodTypeInZones;

    public struct DataLocation {
        public enum DataType {
            None = 0,
            Animal = 1,
            Plant = 2,
        }

        public DataType dataType;
        public int dataIndex;

        public DataLocation(DataType dataType, int dataIndex) {
            this.dataType = dataType;
            this.dataIndex = dataIndex;
        }

        public DataLocation(AnimalScript animal) {
            this.dataType = DataType.Animal;
            this.dataIndex = animal.animalDataIndex;
        }

        public DataLocation(PlantScript plant) {
            this.dataType = DataType.Plant;
            this.dataIndex = plant.plantDataIndex;
        }
    }

    internal JobHandle job;

    public struct ZoneData {
        public float3 position;
        public float maxSize;
        public float waterDepth;

        public ZoneData(float3 position) {
            this.position = position;
            this.maxSize = -2;
            this.waterDepth = 2;
        }

        public ZoneData(float3 position, float maxSize) {
            this.position = position;
            this.maxSize = maxSize;
            this.waterDepth = 2;
        }
    }

    #region SetupAndWrapup
    public void SetupZoneController(EarthScript earth) {
        this.earth = earth;
        FindZoneController = GetComponent<FindZoneController>();
        FindZoneController.SetUpJobController(earth);
    }
    
    public void SpawnZones(float radius, int numberOfZones, int maxNeiboringZones, int numberOfPlants, int numberOfAnimals, ZoneSetupType zoneSetup) {
        Allocate(numberOfZones,maxNeiboringZones,numberOfPlants, numberOfAnimals);
        float phi = Mathf.PI * (3 - Mathf.Sqrt(5));
        for (int i = 0; i < numberOfZones; i++) {
            float yPosition = 1 - (i / (float)(numberOfZones - 1)) * 2;
            float tempRadius = Mathf.Sqrt(1 - yPosition * yPosition);

            float theta = phi * i;
            float xPosition = Mathf.Cos(theta) * tempRadius;
            float zPosition = Mathf.Sin(theta) * tempRadius;
            float3 newZonePosition = new float3(new Vector3(xPosition, yPosition, zPosition) * radius / 2);
            zones[i] = new ZoneData(newZonePosition);
        }
        double distance = 4 * math.PI * math.pow(radius, 2) / (2600 * math.log10(.00051 * numberOfZones + .49) + 1000) / (radius / 16);
        NativeArray<float> maxZoneSize = new NativeArray<float>(zones.Length, Allocator.TempJob);
        if (zoneSetup == ZoneSetupType.Distance) {
            JobHandle zoneDistanceJob = FindDistanceNeiboringZones(distance, maxNeiboringZones, maxZoneSize);
            zoneDistanceJob.Complete();
        } else if (zoneSetup == ZoneSetupType.Closest) {
            JobHandle zoneClosestJob = FindClosestNeiboringZones(maxNeiboringZones, maxZoneSize);
            zoneClosestJob.Complete();
        }
        for (int i = 0; i < maxZoneSize.Length; i++) {
            zones[i] = new ZoneData(zones[i].position, maxZoneSize[i]);
        }
        maxZoneSize.Dispose();
    }


    void Allocate(int numberOfZones,int maxNeibroingZones,int numberOfPlants, int numberOfAnimals) {
        zones = new NativeArray<ZoneData>(numberOfZones, Allocator.Persistent);
        allPlants = new NativeArray<PlantScript.PlantData>(SpeciesManager.Instance.GetAllStartingPlantsAndSeeds() * 5, Allocator.Persistent);
        allAnimals = new NativeArray<AnimalScript.AnimalData>(SpeciesManager.Instance.GetAllStartingAnimals() * 5, Allocator.Persistent);
        neiboringZones = new NativeMultiHashMap<int, int>(numberOfZones * maxNeibroingZones, Allocator.Persistent);
        plantsInZones = new NativeMultiHashMap<int, int>(numberOfPlants, Allocator.Persistent);
        animalsInZones = new NativeMultiHashMap<int, int>(numberOfAnimals, Allocator.Persistent);
        organismsByFoodTypeInZones = new NativeMultiHashMap<int2, DataLocation>(numberOfPlants * 3, Allocator.Persistent);
    }

    JobHandle FindDistanceNeiboringZones(double distance, int maxNeibroingZones, NativeArray<float> maxZoneSize) {
        job = ZoneSetupJobByDistance.BeginJob(zones, neiboringZones.AsParallelWriter(), maxNeibroingZones, distance, maxZoneSize);
        return job;
    }

    JobHandle FindClosestNeiboringZones(int maxNeibroingZones, NativeArray<float> maxZoneSize) {
        job = ZoneSetupJobByClosest.BeginJob(zones, neiboringZones.AsParallelWriter(), maxNeibroingZones, maxZoneSize);
        return job;
    }

    void OnDestroy() {
        neiboringZones.Dispose();
        zones.Dispose();
        allPlants.Dispose();
        allAnimals.Dispose();
        plantsInZones.Dispose();
        animalsInZones.Dispose();
        organismsByFoodTypeInZones.Dispose();
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
        allPlants = new NativeArray<PlantScript.PlantData>(oldAllPlants.Length + length, Allocator.Persistent);
        NativeArray<PlantScript.PlantData>.Copy(oldAllPlants, allPlants, oldAllPlants.Length);
        oldAllPlants.Dispose();
    }

    public void AddAnimals(List<AnimalScript> animals) {
        for (int i = 0; i < animals.Count; i++) {
            AddAnimal(animals[i]);
        }
    }

    public void AddAnimal(AnimalScript animal) {
        if (allAnimalsCount == allAnimals.Length)
            IncreaceAllAnimalLength(1);
        animal.animalDataIndex = allAnimalsCount;
        allAnimalsCount++;
    }

    public void IncreaceAllAnimalLength(int length) {
        NativeArray<AnimalScript.AnimalData> oldAllAnimals = allAnimals;
        allAnimals = new NativeArray<AnimalScript.AnimalData>(oldAllAnimals.Length + length, Allocator.Persistent);
        NativeArray<AnimalScript.AnimalData>.Copy(oldAllAnimals, allAnimals, oldAllAnimals.Length);
        oldAllAnimals.Dispose();
    }

    public void AddFoodTypeToZone(int zone, int foodIndex, DataLocation location) {
        if (zone == -1) {
            Debug.LogError("Problems");
        }
        organismsByFoodTypeInZones.Add(new int2(zone,foodIndex), location);
    }

    public void RemoveFoodTypeFromZone(int zone, int foodIndex, DataLocation location) {
        if (organismsByFoodTypeInZones.TryGetFirstValue(new int2(zone, foodIndex), out DataLocation value, out var iterator)) {
            do {
                if (value.dataType == location.dataType && value.dataIndex == location.dataIndex) {
                    organismsByFoodTypeInZones.Remove(iterator);
                    return;
                }
            } while (organismsByFoodTypeInZones.TryGetNextValue(out value, ref iterator));
        }
    }

    public BasicOrganismScript GetOrganismFromDataLocation(DataLocation location) {
        if (location.dataType == DataLocation.DataType.Animal) {
            return GetAnimalFromDataLocation(location);
        }
        if (location.dataType == DataLocation.DataType.Plant) {
            return GetPlantFromDataLocation(location);
        }
        Debug.LogError("Could not get organism from dataLocation");
        return null;
    }

    public AnimalScript GetAnimalFromDataLocation(DataLocation location) {
        if (location.dataType != DataLocation.DataType.Animal)
            Debug.LogError("location datatype did was not of type animal.");
        if (location.dataIndex == -1)
            Debug.LogError("location dataIndex was not correctly set");
        return earth.GetAllAnimalSpecies()[allAnimals[location.dataIndex].specificSpeciesIndex].GetAnimal(allAnimals[location.dataIndex].animalIndex);
    }

    public PlantScript GetPlantFromDataLocation(DataLocation location) {
        if (location.dataType != DataLocation.DataType.Plant)
            Debug.LogError("location datatype did was not of type plant.");
        if (location.dataIndex == -1)
            Debug.LogError("location dataIndex was not correctly set");
        return earth.GetAllPlantSpecies()[allPlants[location.dataIndex].specificSpeciesIndex].GetPlant(allPlants[location.dataIndex].plantIndex);
    }
    #endregion
}
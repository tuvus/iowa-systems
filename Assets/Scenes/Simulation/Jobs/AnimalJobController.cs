using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class AnimalJobController : JobController {
    public NativeArray<Animal.AnimalActions> animalActions;

    NativeArray<int> updateAnimals;

    public override JobHandle StartUpdateJob() {
        SetUpNativeArrays();
        ZoneController zoneController = GetSpecies().GetEarth().GetZoneController();
        job = AnimalUpdateJob.BeginJob(animalActions, updateAnimals, GetAnimalSpecies().GetActiveAnimalsCount(), GetAnimalSpecies().fullFood, GetAnimalSpecies().maxFood, GetAnimalSpecies().GetSightRange(), GetAnimalSpecies().GetEyeType(),
            GetAnimalSpecies().GetEatRange(), GetAnimalSpecies().GetSmellRange(), GetAnimalSpecies().GetFoodIndex(), GetAnimalSpecies().eddibleFoodTypes, 
            GetAnimalSpecies().predatorFoodTypes, zoneController.allAnimals, zoneController.allPlants,
            zoneController.zones, zoneController.neiboringZones, zoneController.animalsInZones, zoneController.plantsInZones,
            zoneController.organismsByFoodTypeInZones);
        return job;
    }

    void SetUpNativeArrays() {
        if (GetAnimalSpecies().GetActiveAnimalsCount() > updateAnimals.Length)
            SetUpdateAnimalsLength(GetAnimalSpecies().GetActiveAnimalsCount() * 2);
        for (int i = 0; i < GetAnimalSpecies().GetActiveAnimalsCount(); i++) {
            updateAnimals[i] = GetAnimalSpecies().GetAnimal(GetAnimalSpecies().GetActiveAnimal(i)).animalDataIndex;
        }
    }

    void SetUpdateAnimalsLength(int length) {
        animalActions.Dispose();
        animalActions = new NativeArray<Animal.AnimalActions>(length, Allocator.Persistent);

        updateAnimals.Dispose();
        updateAnimals = new NativeArray<int>(length, Allocator.Persistent);
    }
    
    public override void Allocate() {
        animalActions = new NativeArray<Animal.AnimalActions>(500, Allocator.Persistent);

        updateAnimals = new NativeArray<int>(500, Allocator.Persistent);
    }

    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();
        animalActions.Dispose();

        updateAnimals.Dispose();
    }

    public AnimalSpecies GetAnimalSpecies() {
        return (AnimalSpecies)GetSpecies();
    }
}

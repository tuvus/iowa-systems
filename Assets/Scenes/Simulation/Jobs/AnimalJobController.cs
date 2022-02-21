using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class AnimalJobController : BasicJobController {
    AnimalSpecies animalSpecies;

    public NativeArray<AnimalScript.AnimalActions> animalActions;

    NativeArray<int> updateAnimals;

    internal override void SetUpSpecificJobController(BasicSpeciesScript speciesScript) {
        animalSpecies = (AnimalSpecies)speciesScript;
    }

    public override JobHandle StartUpdateJob() {
        SetUpNativeArrays();
        ZoneController zoneController = earth.GetZoneController();
        job = AnimalUpdateJob.BeginJob(animalActions, updateAnimals, animalSpecies.GetActiveAnimalsCount(), animalSpecies.fullFood, animalSpecies.maxFood, animalSpecies.GetSightRange(), animalSpecies.GetEyeType(),
            animalSpecies.GetEatRange(), animalSpecies.GetSmellRange(), animalSpecies.GetFoodIndex(), animalSpecies.eddibleFoodTypes, 
            animalSpecies.predatorFoodTypes, zoneController.allAnimals, zoneController.allPlants,
            zoneController.zones, zoneController.neiboringZones, zoneController.animalsInZones, zoneController.plantsInZones,
            zoneController.organismsByFoodTypeInZones);
        return job;
    }

    void SetUpNativeArrays() {
        if (animalSpecies.GetActiveAnimalsCount() > updateAnimals.Length)
            SetUpdateAnimalsLength(animalSpecies.GetActiveAnimalsCount() * 2);
        for (int i = 0; i < animalSpecies.GetActiveAnimalsCount(); i++) {
            updateAnimals[i] = animalSpecies.GetAnimal(animalSpecies.GetActiveAnimal(i)).animalDataIndex;
        }
    }

    void SetUpdateAnimalsLength(int length) {
        animalActions.Dispose();
        animalActions = new NativeArray<AnimalScript.AnimalActions>(length, Allocator.Persistent);

        updateAnimals.Dispose();
        updateAnimals = new NativeArray<int>(length, Allocator.Persistent);
    }
    
    public override void Allocate() {
        animalActions = new NativeArray<AnimalScript.AnimalActions>(500, Allocator.Persistent);

        updateAnimals = new NativeArray<int>(500, Allocator.Persistent);
    }

    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();
        animalActions.Dispose();

        updateAnimals.Dispose();
    }
}

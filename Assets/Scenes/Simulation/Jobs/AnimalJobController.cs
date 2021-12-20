using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class AnimalJobController : BasicJobController {
    BasicAnimalSpecies animalSpecies;

    public NativeArray<AnimalActions> animalActions;

    NativeArray<BasicAnimalScript.AnimalData> animals;
    int animalCount;

    public float speciesFullFood;
    public float speciesMaxFood;
    public float speciesSightRange;
    public float speciesEatRange;
    public float speciesSmellRange;

    int availableMaleMateCount;
    NativeArray<float3> availableMaleMatePositions;
    int availableFemaleMateCount;
    NativeArray<float3> availableFemaleMatesPositions;

    int predatorCount;
    [ReadOnly] NativeArray<BasicAnimalScript.PredatorData> predators;

    internal override void SetUpSpecificJobController(BasicSpeciesScript speciesScript) {
        animalSpecies = (BasicAnimalSpecies)speciesScript;
    }

    public override JobHandle StartUpdateJob() {
        SetUpNativeArrays();
        job = AnimalUpdateJob.BeginJob(animalActions,animals,animalCount, speciesFullFood, speciesMaxFood, speciesSightRange, speciesEatRange, speciesSmellRange,
             availableMaleMateCount,availableMaleMatePositions,availableFemaleMateCount, availableFemaleMatesPositions,predatorCount, predators);
        return job;
    }

    void SetUpNativeArrays() {
        animalCount = animalSpecies.GetAnimals().Count;
        for (int i = 0; i < animalCount; i++) {
            BasicAnimalScript animal = animalSpecies.GetAnimals()[i];
            animals[i] = animals[i].SetupData(animal);
        }

        speciesFullFood = animalSpecies.fullFood;
        speciesMaxFood = animalSpecies.maxFood;
        speciesSightRange = animalSpecies.GetSightRange();
        speciesEatRange = animalSpecies.GetEatRange();
        speciesSmellRange = animalSpecies.GetSmellRange();

        availableMaleMateCount = animalSpecies.GetAvailableMaleMates().Count;
        for (int i = 0; i < availableMaleMateCount; i++) {
            availableMaleMatePositions[i] = animalSpecies.GetAvailableMaleMates()[i].position;
        }
        availableFemaleMateCount = animalSpecies.GetAvailableFemaleMates().Count;
        for (int i = 0; i < availableFemaleMateCount; i++) {
            availableFemaleMatesPositions[i] = animalSpecies.GetAvailableFemaleMates()[i].position;
        }

        //eddibleCount = animalSpecies.GetEddibleFood().Count;
        //for (int i = 0; i < eddibleCount; i++) {
        //    Eddible eddible = animalSpecies.GetEddibleFood()[i];
        //    eddibles[i] = eddibles[i].SetupData(eddible);
        //}

        //predatorCount = animalSpecies.GetPredators().Count;
        //for (int i = 0; i < predatorCount; i++) {
        //    BasicAnimalScript predator = animalSpecies.GetPredators()[i];
        //    predators[i] = predators[i].SetupData(predator);
        //}
    }
    
    public override void Allocate() {
        animalActions = new NativeArray<AnimalActions>(500, Allocator.Persistent);

        animals = new NativeArray<BasicAnimalScript.AnimalData>(500, Allocator.Persistent);

        availableMaleMatePositions = new NativeArray<float3>(500, Allocator.Persistent);
        availableFemaleMatesPositions = new NativeArray<float3>(500, Allocator.Persistent);

        predators = new NativeArray<BasicAnimalScript.PredatorData>(600, Allocator.Persistent);
    }

    internal override void OnDestroy() {
        JobHandle.ScheduleBatchedJobs();
        job.Complete();
        animalActions.Dispose();

        animals.Dispose();

        availableMaleMatePositions.Dispose();
        availableFemaleMatesPositions.Dispose();

        predators.Dispose();
    }
}

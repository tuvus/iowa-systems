using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public class AnimalSpecies : Species {
    public GameObject basicOrganism;

    public Color corpseColor;
    //AnimalStats
    [Tooltip("Weight in kilograms")]
    public float bodyWeight;
    public float maxHealth;
    public float speed;
    [Tooltip("Daily food used by the organism in kilograms")]
    public float foodConsumption;
    [Tooltip("The food in kilograms at which the animal is hungry")]
    public float fullFood;
    [Tooltip("Maximum ammount of food stored in kilograms")]
    public float maxFood;
    [Tooltip("The age at which the animal will die at in days")]
    public int maxAge;

    [Tooltip("The time it takes a corpse to deteriorate in days")]
    public float deteriationTime;

    int foodIndex;
    [SerializeField] List<string> eddibleFoodTypesInput = new List<string>();
    public enum GrowthStage {
        Dead = -2,
        Corpse = -1,
        Juvinile = 0,
        Adult = 1,
    }

    public struct Animal {
        public GrowthStage stage;
        [Tooltip("Weight in kilograms")]
        public float bodyWeight;
        public float health;
        [Tooltip("The food in kilograms stored in the animal")]
        public float food;

        public Animal(GrowthStage stage, float bodyWeight, float health, float food) {
            this.stage = stage;
            this.bodyWeight = bodyWeight;
            this.health = health;
            this.food = food;
        }

        public Animal(Animal animal, float health, float food) {
            this.stage = animal.stage;
            this.bodyWeight = animal.bodyWeight;
            this.health = health;
            this.food = food;
        }

        public Animal(Animal animal, GrowthStage stage) {
            this.stage = stage;
            this.bodyWeight = animal.bodyWeight;
            this.health = animal.health;
            this.food = animal.food;
        }
    }

    [NativeDisableContainerSafetyRestriction] public NativeArray<Animal> animals;

    [NativeDisableContainerSafetyRestriction] public NativeArray<Organism> deadAnimals;
    public NativeArray<int> activeDeadAnimals;
    public int activeDeadAnimalCount;
    public NativeArray<int> inactiveDeadAnimals;
    public int inactiveDeadAnimalCount;

    public NativeArray<int> eddibleFoodTypes;
    public NativeArray<int> predatorFoodTypes;
    private AnimalSpeciesReproductiveSystem reproductiveSystem;

    #region StartSimulation
    public override void SetupSimulation(Earth earth) {
        reproductiveSystem = gameObject.GetComponent<AnimalSpeciesReproductiveSystem>();
        base.SetupSimulation(earth);
        fullFood = maxFood * .7f;
    }

    public override void SetupArrays(int arrayLength) {
        base.SetupArrays(arrayLength);
        animals = new NativeArray<Animal>(arrayLength, Allocator.Persistent);
    }

    public override void SetupSpeciesFoodType() {
        foodIndex = GetEarth().GetIndexOfFoodType(speciesName);
        List<int> tempEddibleFoodTypes = new List<int>();
        for (int i = 0; i < eddibleFoodTypesInput.Count; i++) {
            if (GetEarth().GetIndexOfFoodType(eddibleFoodTypesInput[i]) != -1) {
                tempEddibleFoodTypes.Add(GetEarth().GetIndexOfFoodType(eddibleFoodTypesInput[i]));
            }
        }
        eddibleFoodTypes = new NativeArray<int>(tempEddibleFoodTypes.Count, Allocator.Persistent);
        for (int i = 0; i < tempEddibleFoodTypes.Count; i++) {
            eddibleFoodTypes[i] = tempEddibleFoodTypes[i];
        }
    }

    public void SetupAnimalPredatorSpeciesFoodType() {
        List<int> tempPredatorFoodTypes = new List<int>();
        for (int i = 0; i < SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies().Count; i++) {
            if (i == speciesIndex || SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[i].GetType() != typeof(Animal))
                continue;
            AnimalSpecies animalSpecies = (AnimalSpecies)SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[i];
            if (animalSpecies != null && animalSpecies.eddibleFoodTypes.Contains(GetFoodIndex())) {
                if (!tempPredatorFoodTypes.Contains(animalSpecies.GetFoodIndex()))
                    tempPredatorFoodTypes.Add(animalSpecies.GetFoodIndex());
            }
        }
        predatorFoodTypes = new NativeArray<int>(tempPredatorFoodTypes.Count, Allocator.Persistent);
        for (int i = 0; i < tempPredatorFoodTypes.Count; i++) {
            predatorFoodTypes[i] = tempPredatorFoodTypes[i];
        }
    }

    public override void StartSimulation() {
        Populate();
    }

    public override void Populate() {
        for (int i = 0; i < startingPopulation; i++) {
            SpawnOrganism();
        }
    }
    #endregion

    public override int SpawnOrganism() {
        int animal = base.SpawnOrganism();
        organisms[animal] = new Organism(organisms[animal], Simulation.randomGenerator.NextFloat(reproductiveSystem.reproductionAge / 2, maxAge / 1.2f),
            -1, Vector3.zero, 0);
        animals[animal] = new Animal(reproductiveSystem.SpawnReproductive(animal), bodyWeight, maxHealth, Simulation.randomGenerator.NextFloat(fullFood, maxFood));
        return animal;
    }

    public override int SpawnOrganism(float3 position, int zone, float distance) {
        int animal = base.SpawnOrganism();
        organisms[animal] = new Organism(organisms[animal], 0, -1, Vector3.zero, 0);
        animals[animal] = new Animal();
        throw new NotImplementedException();
        return animal;
    }

    protected override void IncreaseOrganismSize(int newSize) {
        base.IncreaseOrganismSize(newSize);
        NativeArray<Animal> oldAnimals = animals;
        animals = new NativeArray<Animal>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldAnimals.Length; i++) {
            animals[i] = oldAnimals[i];
        }
        oldAnimals.Dispose();
    }

    public int SpawnDeadAnimal() {
        int deadAnimal = ActivateInactiveDeadAnimal();
        deadAnimals[deadAnimal] = new Species.Organism(deadAnimals[deadAnimal], 0, 0, float3.zero, 0, activeDeadAnimalCount - 1, true);
        throw new NotImplementedException("Need to add position and rotation here.");
        return deadAnimal;
    }

    /// <summary>
    /// Gets an inactive dead animal, activates it and returns it.
    /// May change the size of the dead animal arrays
    /// </summary>
    /// <returns>A new active dead animal</returns>
    private int ActivateInactiveDeadAnimal() {
        if (inactiveDeadAnimalCount == 0) {
            IncreaseDeadAnimalSize(deadAnimals.Length * 2);
        }
        int newDeadAnimal = inactiveDeadAnimals[inactiveDeadAnimalCount - 1];
        inactiveDeadAnimalCount--;

        activeDeadAnimals[activeDeadAnimalCount] = newDeadAnimal;
        activeDeadAnimalCount++;
        return newDeadAnimal;
    }

    /// <summary>
    /// Removes the dead animal from the active list and adds it to the inactive list.
    /// Does not acualy change the dead animal's data.
    /// </summary>
    /// <param name="deadAnimalIndex">The index of the dead animal</param>
    public void DeactivateActiveDeadAnimal(int deadAnimalIndex) {
        for (int i = deadAnimals[deadAnimalIndex].maxActiveOrganismIndex; i < activeDeadAnimalCount - 1; i++) {
            activeDeadAnimals[i] = activeDeadAnimals[i + 1];
        }
        activeDeadAnimalCount--;
        inactiveDeadAnimals[inactiveDeadAnimalCount] = deadAnimalIndex;
        inactiveDeadAnimalCount++;
    }

    /// <summary>
    /// Increases the size of the dead animals arrays, active and inactive arrays.
    /// </summary>
    /// <param name="newSize"></param>
    protected virtual void IncreaseDeadAnimalSize(int newSize) {
        throw new NotImplementedException("IncreaseDeadAnimalsSize has not been implamented yet.");
    }

    protected override void UpdateOrganism(int organism) {
        base.UpdateOrganism(organism);
        if (organisms[organism].age > maxAge) {
            organismActionsParallelWriter.Enqueue(new OrganismAction(OrganismAction.Action.Die, organism));
            return;
        }
        if (animals[organism].stage != GrowthStage.Adult && organisms[organism].age > reproductiveSystem.reproductionAge)
            animals[organism] = new Animal(animals[organism], GrowthStage.Adult);
        if (animals[organism].food > 0) {
            float restingFoodReduction = 1f;
            //if (!hasMoved)
            //    restingFoodReduction = .6f;
            animals[organism] = new Animal(animals[organism], math.max(maxHealth, animals[organism].health * GetEarth().simulationDeltaTime / 24), math.max(0, animals[organism].food - GetFoodConsumption() * GetEarth().simulationDeltaTime * restingFoodReduction));
        } else {
            animals[organism] = new Animal(animals[organism], math.max(0, animals[organism].health - GetFoodConsumption() * GetEarth().simulationDeltaTime), 0);
            //if (CheckIfDead("Starvation")) {
            //    return true;
            //}
        }
        //List<int> zonesInSightRange;
        //if (GetEyeType() == EyesOrgan.EyeTypes.Foward) {
        //    zonesInSightRange = ZoneCalculator.GetNearbyZones(GetEarth().GetZoneController(), organisms[organism].zone, animal.animalEyePosition.c0, speciesSightRange); ;
        //} else {
        //    zonesInSightRange = ZoneCalculator.GetNearbyZonesFromTwoPositions(zones, neiboringZones, animal.zone, animal.animalEyePosition, speciesSightRange);
        //}
        //ZoneController.DataLocation closestPredator = GetClosestPredator(animal, zonesInSightRange);
        //if (closestPredator != null) {
        //    throw new NotImplementedException("RunFromPredator");
        //}
        //if (!IsAnimalFull(organism)) {
        //    ZoneController.DataLocation closestBestMouthFood = GetClosestBestMouthFood(animal);
        //    if (closestBestMouthFood.dataType != ZoneController.DataLocation.DataType.None) {
        //        animalActions[animalIndex] = new Animal.AnimalActions(Animal.AnimalActions.ActionType.EatFood, closestBestMouthFood);
        //        return;
        //    }

        //    if (IsAnimalHungry(organism)) {
        //        ZoneController.DataLocation closestBestSightFood = GetClosestBestSightFood(animal, zonesInSightRange);
        //        if (closestBestSightFood.dataType != ZoneController.DataLocation.DataType.None) {
        //            animalActions[animalIndex] = new Animal.AnimalActions(Animal.AnimalActions.ActionType.GoToFood, closestBestSightFood);
        //            return;
        //        }
        //    }
        //}

        //if (!IsAnimalHungry(organism) && IsAnimalReadyToAttemptReproduction(organism)) {
        //    ZoneController.DataLocation closestAvailableMate = GetClosestAvailableMate(animal, zonesInSightRange);
        //    if (closestAvailableMate.dataType == ZoneController.DataLocation.DataType.Animal) {
        //        animalActions[animalIndex] = new Animal.AnimalActions(Animal.AnimalActions.ActionType.AttemptReproduction, closestAvailableMate);
        //        return;
        //    }
        //}

        //if (IsAnimalHungry(organism) || IsAnimalReadyToAttemptReproduction(organism)) {
        //    animalActions[animalIndex] = new Animal.AnimalActions(Animal.AnimalActions.ActionType.Explore);
        //    return;
        //}
    }

    bool IsAnimalFull(int organism) {
        if (animals[organism].food >= maxFood * .9f)
            return true;
        return false;
    }

    bool IsAnimalHungry(int organism) {
        if (animals[organism].food < fullFood)
            return true;
        return false;
    }

    bool IsAnimalReadyToAttemptReproduction(int organism) {
        throw new NotImplementedException();
    }

    float GetEyeDistance(float3x2 eyePositions, float3 to) {
        throw new NotImplementedException();
        if (GetEyeType() == AnimalSpeciesEyes.EyeTypes.Foward) {
            return math.distance(eyePositions.c0, to);
        } else {
            return GetClosestDistanceFromTwoPositions(eyePositions, to);
        }
    }

    float GetClosestDistanceFromTwoPositions(float3x2 from, float3 to) {
        float distance = math.distance(from.c0, to);
        float otherEyeDistance = math.distance(from.c1, to);
        if (otherEyeDistance < distance)
            return otherEyeDistance;
        return distance;
    }

    #region AnimalControls
    public float GetFoodConsumption() {
        return foodConsumption / 24;
    }

    public override void OnSettingsChanged(bool renderOrganisms) {
        //for (int i = 0; i < activeAnimals.Count; i++) {
        //    animals[activeAnimals[i]].GetMeshRenderer().enabled = renderOrganisms;
        //}
        //for (int i = 0; i < activeCorpses.Count; i++) {
        //    animals[activeCorpses[i]].GetMeshRenderer().enabled = renderOrganisms;
        //}
        throw new NotImplementedException();
    }

    public float GetMovementSpeed(int organism) {
        return speed * (((animals[organism].health / maxHealth) / 2) + 0.5f);
    }
    #endregion

    #region GetMethods
    public float GetSightRange() {
        return GetComponent<AnimalSpeciesEyes>().sightRange;
    }

    public AnimalSpeciesEyes.EyeTypes GetEyeType() {
        return GetComponent<AnimalSpeciesEyes>().eyeType;
    }

    public float GetEatRange() {
        return GetComponent<AnimalSpeciesMouth>().eatRange;
    }

    public float GetSmellRange() {
        return GetComponent<AnimalSpeciesNose>().smellRange;
    }

    public float GetReproductiveAge() {
        return reproductiveSystem.reproductionAge;
    }

    public override List<string> GetOrganismFoodTypes() {
        List<string> foodTypes = new List<string>();
        foodTypes.Add(speciesName);
        return foodTypes;
    }

    public int GetFoodIndex() {
        return foodIndex;
    }

    public Color GetCorpseColor() {
        return corpseColor;
    }
    #endregion

    /// <summary>
    /// Called after a simulation has ended but also after the intro scene is unloaded.
    /// </summary>
    public override void OnDestroy() {
        base.OnDestroy();
        if (eddibleFoodTypes.IsCreated)
            eddibleFoodTypes.Dispose();
        if (predatorFoodTypes.IsCreated)
            predatorFoodTypes.Dispose();
        if (animals.IsCreated)
            animals.Dispose();
        if (deadAnimals.IsCreated)
            deadAnimals.Dispose();
        if (activeDeadAnimals.IsCreated)
            activeDeadAnimals.Dispose();
        if (inactiveDeadAnimals.IsCreated)
            inactiveDeadAnimals.Dispose();
    }
}
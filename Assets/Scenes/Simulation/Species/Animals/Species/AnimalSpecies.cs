using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

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

    public Dictionary<Organism, Animal> animals;

    public AnimalSpeciesCarcass speciesCarcass;

    public HashSet<string> eddibleFoodTypes;
    public HashSet<Species> predatorSpecies;
    private AnimalSpeciesReproductiveSystem reproductiveSystem;

    #region StartSimulation
    public override void SetupSimulation(Earth earth) {
        reproductiveSystem = gameObject.GetComponent<AnimalSpeciesReproductiveSystem>();
        base.SetupSimulation(earth);
        fullFood = maxFood * .7f;
        speciesCarcass.SetSpeciesScript(this);
    }

    public override void SetupSpeciesFoodType() {
        eddibleFoodTypes = new HashSet<string>(eddibleFoodTypesInput.Where(t => GetEarth().footTypesUsed.Contains(t)));
    }

    public void SetupAnimalPredatorSpeciesFoodType() {
        foreach (var species in SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()) {
            if (species == this || species.GetType() != typeof(Animal)) continue;
            AnimalSpecies animalSpecies = (AnimalSpecies)species;
            if (animalSpecies.eddibleFoodTypes.Contains(speciesName)) {
                predatorSpecies.Add(animalSpecies);
            }
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

    public override Organism SpawnOrganism() {
        Organism organism = base.SpawnOrganism();
        Animal animal = new Animal(reproductiveSystem.SpawnReproductive(organism), bodyWeight, maxHealth,
            Simulation.randomGenerator.NextFloat(fullFood, maxFood));
        animals.Add(organism, animal);
        //TODO: Add position and rotation
        return organism;
    }

    public override Organism SpawnOrganism(float3 position, int zone, float distance) {
        Organism organism = base.SpawnOrganism();
        Animal animal = new Animal(reproductiveSystem.SpawnReproductive(organism), bodyWeight, maxHealth,
            Simulation.randomGenerator.NextFloat(fullFood, maxFood));
        //TODO: Add position and rotation
        return organism;
    }

    protected override void UpdateOrganism(Organism organism) {
        base.UpdateOrganism(organism);
        if (organism.age > maxAge) {
            // organismActions.Enqueue(new OrganismAction(OrganismAction.Action.Die, organism));
            return;
        }
        if (animals[organism].stage != GrowthStage.Adult && organism.age > reproductiveSystem.reproductionAge)
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

    bool IsAnimalFull(Organism organism) {
        if (animals[organism].food >= maxFood * .9f)
            return true;
        return false;
    }

    bool IsAnimalHungry(Organism organism) {
        if (animals[organism].food < fullFood)
            return true;
        return false;
    }

    bool IsAnimalReadyToAttemptReproduction(int organism) {
        throw new NotImplementedException();
    }

    float GetEyeDistance(float3x2 eyePositions, float3 to) {
        //TODO: Implement eye distance
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

    public float GetMovementSpeed(Organism organism) {
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
}
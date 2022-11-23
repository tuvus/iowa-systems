using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static Earth;
using static PlantSpecies;
using static Species;

public class PlantSpeciesSeeds : PlantSpeciesOrgan {
    public int startingSeedCount;
    public float humidityRequirement;
    public float tempetureRequirement;
    public float seedDispertionRange;
    [Tooltip("The time in days required before germination can begin")]
    public float timeRequirement;
    [Tooltip("The maximum time in days the seed can wait to germinate before it dies off")]
    public float timeMaximum;

    public float awnMaxGrowth;
    public int awnMaxSeedAmount;
    [Tooltip("The time the in days it takes until the awns release thier seeds")]
    public float awnSeedDispertionTime;
    public int awnSeedDispersalSuccessChance;

    public float seedEnergyAmount;

    public struct Awn {
        public float awnsGrowth;
        public float timeUntilDispersion;

        public Awn(float awnsGrowth, float timeUntilDispersion) {
            this.awnsGrowth = awnsGrowth;
            this.timeUntilDispersion = timeUntilDispersion;
        }
    }

    [NativeDisableUnsafePtrRestriction] public NativeArray<Awn> awns;

    [NativeDisableUnsafePtrRestriction] public NativeArray<Organism> seeds;
    public NativeArray<int> activeSeeds;
    public int activeSeedsCount;
    public NativeArray<int> inactiveSeeds;
    public int inactiveSeedsCount;
    public NativeArray<OrganismAction> seedActions;
    public int seedActionsCount;

    SpeciesSeedsUpdateJob speciesSeedsUpdateJob;

    public override void SetupSpeciesOrganArrays(int arraySize) {
        awns = new NativeArray<Awn>(arraySize, Allocator.Persistent);
        seeds = new NativeArray<Organism>(startingSeedCount * 2, Allocator.Persistent);
        activeSeeds = new NativeArray<int>(startingSeedCount * 2, Allocator.Persistent);
        inactiveSeeds = new NativeArray<int>(startingSeedCount * 2, Allocator.Persistent);
        seedActions = new NativeArray<OrganismAction>(startingSeedCount * 2, Allocator.Persistent);
        for (int i = 0; i < seeds.Length; i++) {
            inactiveSeeds[i] = i;
        }
        inactiveSeedsCount = inactiveSeeds.Length;
        activeSeedsCount = 0;
        seedActionsCount = 0;
        speciesSeedsUpdateJob = new SpeciesSeedsUpdateJob(GetPlantSpecies().speciesIndex);
    }

    public override void IncreaseOrganismSize(int newSize) {
        NativeArray<Awn> oldAwns = awns;
        awns = new NativeArray<Awn>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldAwns.Length; i++) {
            awns[i] = oldAwns[i];
        }
        oldAwns.Dispose();
    }

    public void Populate() {
        for (int i = 0; i < startingSeedCount; i++) {
            SpawnSeed();
        }
    }

    public int SpawnSeed() {
        int seed = ActivateInactiveSeed();
        seeds[seed] = new Organism(seeds[seed], Simulation.randomGenerator.NextFloat(0, timeRequirement), 0, float3.zero, 0, activeSeedsCount - 1, true);
        //Debug.LogWarning("Need to add position and rotation here.");
        return seed;
    }

    public int SpawnSeed(float3 position, int zone, float distance) {
        int seed = ActivateInactiveSeed();
        seeds[seed] = new Organism(seeds[seed], 0, zone, position, 0, activeSeedsCount - 1, true);
        //Debug.LogWarning("Need to add position and rotation here.");
        return seed;
    }

    public void SpawnAwns(int organism) {
        if (GetPlantSpecies().plants[organism].stage == GrowthStage.Adult) {
            if (Simulation.randomGenerator.NextBool()) {
                awns[organism] = new Awn(awnMaxGrowth, Simulation.randomGenerator.NextFloat(0, awnSeedDispertionTime));
            } else {
                awns[organism] = new Awn(Simulation.randomGenerator.NextFloat(0, awnMaxGrowth), 0);
            }
        } else {
            awns[organism] = new Awn(0, 0);
        }
    }

    /// <summary>
    /// Gets an inactive seed, activates it and returns it.
    /// May change the size of the seed arrays
    /// </summary>
    /// <returns>A new active seed</returns>
    private int ActivateInactiveSeed() {
        //Make sure that there are free inactive seeds to get
        if (inactiveSeedsCount == 0) {
            IncreaseSeedsSize(seeds.Length * 2);
        }
        //Get the first inactive seed and remove it from the inactiveSeeds list
        int newOrganism = inactiveSeeds[inactiveSeedsCount - 1];
        inactiveSeedsCount--;
        //Add the organism to the activeSeedsList
        activeSeeds[activeSeedsCount] = newOrganism;
        activeSeedsCount++;
        return newOrganism;
    }

    /// <summary>
    /// Removes the seed from the active list and adds it to the inactive list.
    /// Does not acualy change the seed's data.
    /// </summary>
    /// <param name="seedIndex">The index of the seed</param>
    public void DeactivateActiveSeed(int seedIndex) {
        //Check if the seed is still active
        if (!seeds[seedIndex].spawned)
            return;
        //Finds the activeSeedIndex starting at maxActiveSeedIndex and works it's way to the begining.
        //Because of the way activeSeeds are removed the index must be equal to or less than maxActiveSeedIndex.
        int activeSeedIndex = seeds[seedIndex].maxActiveOrganismIndex;
        for (; activeSeedIndex >= -1; activeSeedIndex--) {
            if (activeSeeds[activeSeedIndex] == seedIndex)
                break;
        }
        //Remove the seed from the active list
        for (int i = activeSeedIndex; i < activeSeedsCount - 1; i++) {
            activeSeeds[i] = activeSeeds[i + 1];
        }
        activeSeedsCount--;
        //Add the seed to the inactive list
        inactiveSeeds[inactiveSeedsCount] = seedIndex;
        inactiveSeedsCount++;
        seeds[seedIndex] = new Organism(seeds[seedIndex], -2, false);
    }

    /// <summary>
    /// Increases the size of the seed arrays, active and inactive arrays.
    /// </summary>
    /// <param name="newSize"></param>
    protected virtual void IncreaseSeedsSize(int newSize) {
        NativeArray<Organism> oldSeeds = seeds;
        seeds = new NativeArray<Organism>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldSeeds.Length; i++) {
            seeds[i] = oldSeeds[i];
        }
        oldSeeds.Dispose();
        NativeArray<int> oldActiveSeeds = activeSeeds;
        activeSeeds = new NativeArray<int>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldActiveSeeds.Length; i++) {
            activeSeeds[i] = oldActiveSeeds[i];
        }
        oldActiveSeeds.Dispose();
        NativeArray<int> oldInActiveSeeds = inactiveSeeds;
        inactiveSeeds = new NativeArray<int>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldInActiveSeeds.Length; i++) {
            inactiveSeeds[i] = oldInActiveSeeds[i];
        }
        //Add new inactiveSeeds to the inactiveSeedList and increment inactiveSeedCount
        for (int i = oldInActiveSeeds.Length; i < inactiveSeeds.Length; i++) {
            inactiveSeeds[inactiveSeedsCount] = i;
            inactiveSeedsCount++;
        }
        oldInActiveSeeds.Dispose();
        NativeArray<OrganismAction> oldSeedActions = seedActions;
        seedActions = new NativeArray<OrganismAction>(newSize, Allocator.Persistent);
        for (int i = 0; i < oldSeedActions.Length; i++) {
            seedActions[i] = oldSeedActions[i];
        }
        oldSeedActions.Dispose();
    }

    public override void GrowOrgan(int organism, float growth, ref float bladeArea, ref float stemHeight, ref float2 rootGrowth) {
        if (awns[organism].timeUntilDispersion > 0) {
            awns[organism] = new Awn(0, math.max(0, awns[organism].timeUntilDispersion - GetPlantSpecies().GetEarth().simulationDeltaTime / 24));
            if (awns[organism].timeUntilDispersion <= 0) {
                int disperseSeeds = 0;
                for (int i = 0; i < awnMaxSeedAmount; i++) {
                    if (Simulation.randomGenerator.NextInt(0, 100) < awnSeedDispersalSuccessChance) {
                        disperseSeeds++;
                    }
                }
                int actionIndex = Interlocked.Increment(ref GetPlantSpecies().organismActionsCount);
                GetPlantSpecies().organismActions[actionIndex] = new OrganismAction(OrganismAction.Action.Reproduce, organism, GetPlantSpecies(), disperseSeeds, seedDispertionRange);
            }
            return;
        }
        awns[organism] = new Awn(awns[organism].awnsGrowth + growth / 100, 0);
        if (awns[organism].awnsGrowth >= awnMaxGrowth) {
            awns[organism] = new Awn(0, awnSeedDispertionTime);
        }
    }

    public struct SpeciesSeedsUpdateJob : IJobParallelFor {
        private int species;

        public SpeciesSeedsUpdateJob(int species) {
            this.species = species;
        }

        public JobHandle BeginJob() {
            return IJobParallelForExtensions.Schedule(this, ((PlantSpecies)SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species]).GetSpeciesSeeds().activeSeedsCount, 10);
        }

        public void Execute(int index) {
            ((PlantSpecies)SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species]).GetSpeciesSeeds().UpdateSeed(
                ((PlantSpecies)SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species]).GetSpeciesSeeds().activeSeeds[index]);
        }
    }

    public override JobHandle? StartJob() {
        return speciesSeedsUpdateJob.BeginJob();
    }

    public void UpdateSeed(int seed) {
        seeds[seed] = new Organism(seeds[seed], seeds[seed].age + GetPlantSpecies().GetEarth().simulationDeltaTime);
        if (seeds[seed].age > timeMaximum) {
            int actionIndex = Interlocked.Increment(ref seedActionsCount);
            seedActions[actionIndex] = new OrganismAction(OrganismAction.Action.Die, seed);
            return;
        } else if (seeds[seed].age >= timeRequirement
            && earth.earthState.humidity > humidityRequirement
            && earth.earthState.temperature > tempetureRequirement) {
            int actionIndex = Interlocked.Increment(ref seedActionsCount);
            seedActions[actionIndex] = new OrganismAction(OrganismAction.Action.Reproduce, seed);
        }
    }

    public void UpdateSeedActions() {
        while (seedActionsCount >= 0) {
            //No need to worry about deactivating an already inactive organism, it is handled in DeactivateActiveOrganism()
            if (seedActions[seedActionsCount].organism > seeds.Length || seedActions[seedActionsCount].organism < 0)
                print("Thread error");
            switch (seedActions[seedActionsCount].action) {
                case OrganismAction.Action.Starve:
                    DeactivateActiveSeed(seedActions[seedActionsCount].organism);
                    break;
                case OrganismAction.Action.Die:
                    if (seeds[seedActions[seedActionsCount].organism].spawned)
                        DeactivateActiveSeed(seedActions[seedActionsCount].organism);
                    break;
                case OrganismAction.Action.Bite:
                    break;
                case OrganismAction.Action.Eat:
                    break;
                case OrganismAction.Action.Reproduce:
                    DeactivateActiveSeed(seedActions[seedActionsCount].organism);
                    GrowSeed(seedActions[seedActionsCount]);
                    break;
            }
            seedActionsCount--;
        }
    }

    public void GrowSeed(OrganismAction organismAction) {
        GetPlantSpecies().SpawnOrganism(organismAction.position, organismAction.zone, organismAction.floatValue);
    }

    public override string GetOrganType() {
        return organType;
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == PlantSpecies.GrowthStage.Adult) {
            return awnMaxGrowth / growthModifier;
        }
        return 0;
    }

    public override void OnDestroy() {
        base.OnDestroy();
        if (seeds.IsCreated)
            seeds.Dispose();
        if (activeSeeds.IsCreated)
            activeSeeds.Dispose();
        if (inactiveSeeds.IsCreated)
            inactiveSeeds.Dispose();
        if (awns.IsCreated)
            awns.Dispose();
        if (seedActions.IsCreated)
            seedActions.Dispose();
    }
}
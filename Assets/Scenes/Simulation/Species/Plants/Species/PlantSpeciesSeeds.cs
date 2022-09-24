using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
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
        public float timeUntillDispersion;
    }

    public NativeArray<Awn> awns;

    public NativeArray<Species.Organism> seeds;
    public NativeArray<int> activeSeeds;
    public int activeSeedsCount;
    public NativeArray<int> inactiveSeeds;
    public int inactiveSeedsCount;

    public SeedGerminationRequirement seedGerminationRequirement;

    public struct SeedGerminationRequirement {
        public float humidityRequirement;
        public float tempetureRequirement;
        [Tooltip("The time in days required before germination can begin")]
        public float timeRequirement;
        [Tooltip("The maximum time in days the seed can wait to germinate before it dies off")]
        public float timeMaximum;

        public SeedGerminationRequirement(PlantSpeciesSeeds speciesSeeds) {
            humidityRequirement = speciesSeeds.humidityRequirement;
            tempetureRequirement = speciesSeeds.tempetureRequirement;
            timeRequirement = speciesSeeds.timeRequirement;
            timeMaximum = speciesSeeds.timeMaximum;
        }
    }

    public override void SetupSpeciesOrganArrays(int arraySize) {
        seedGerminationRequirement = new SeedGerminationRequirement(this);
        awns = new NativeArray<Awn>(arraySize, Allocator.Persistent);
        seeds = new NativeArray<Species.Organism>(startingSeedCount * 2, Allocator.Persistent);
        activeSeeds = new NativeArray<int>(startingSeedCount * 2, Allocator.Persistent);
        inactiveSeeds = new NativeArray<int>(startingSeedCount * 2, Allocator.Persistent);
        for (int i = 0; i < seeds.Length; i++) {
            inactiveSeeds[i] = i;
        }
        inactiveSeedsCount = inactiveSeeds.Length;
        activeSeedsCount = 0;
    }

    public void Populate() {
        for (int i = 0; i < startingSeedCount; i++) {
            SpawnSeed();
        }
    }

    public int SpawnSeed() {
        int seed = ActivateInactiveSeed();
        seeds[seed] = new Species.Organism(seeds[seed], Simulation.randomGenerator.NextFloat(0, seedGerminationRequirement.timeRequirement), 0, float3.zero, 0, activeSeedsCount- 1, true);
        throw new NotImplementedException("Need to add position and rotation here.");
        return seed;
    }

    /// <summary>
    /// Gets an inactive seed, activates it and returns it.
    /// May change the size of the seed arrays
    /// </summary>
    /// <returns>A new active seed</returns>
    private int ActivateInactiveSeed() {
        if (inactiveSeedsCount == 0) {
            IncreaseSeedsSize(seeds.Length * 2);
        }
        int newOrganism = inactiveSeeds[inactiveSeedsCount - 1];
        inactiveSeedsCount--;

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
        for (int i = seeds[seedIndex].activeOrganismIndex; i < activeSeedsCount - 1; i++) {
            activeSeeds[i] = activeSeeds[i + 1];
        }
        activeSeedsCount--;
        inactiveSeeds[inactiveSeedsCount] = seedIndex;
        inactiveSeedsCount++;
    }

    /// <summary>
    /// Increases the size of the seed arrays, active and inactive arrays.
    /// </summary>
    /// <param name="newSize"></param>
    protected virtual void IncreaseSeedsSize(int newSize) {
        throw new NotImplementedException("IncreaseSeedSize has not been implamented yet.");
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

    public void OnDestroy() {
        if (seeds.IsCreated)
            seeds.Dispose();
        if (activeSeeds.IsCreated)
            activeSeeds.Dispose();
        if (inactiveSeeds.IsCreated)
            inactiveSeeds.Dispose();
        if (awns.IsCreated)
            awns.Dispose();
    }
}
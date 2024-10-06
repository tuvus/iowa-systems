using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using static Earth;
using static Species;
using Plant = PlantSpecies.Plant;

public class PlantSpeciesSeed : PlantSpeciesOrgan {
    public int startingSeedCount;
    public float humidityRequirement;
    public float tempetureRequirement;
    public float seedDispertionRange;
    public float seedGerminationChance;

    [Tooltip("The time in days required before germination can begin")]
    public float timeRequirement;

    [Tooltip("The maximum time in days the seed can wait to germinate before it dies off")]
    public float timeMaximum;

    public float seedEnergyAmount;

    public class SeedGroup : Organism {
        public int seedCount;
        public SeedGroup() { }

        public SeedGroup(float age, int zone, float3 position, float rotation, int seedCount)
            : base(age, zone, position, rotation) {
            this.seedCount = seedCount;
        }

        public SeedGroup(SeedGroup seedGroup) : base(seedGroup) {
            this.seedCount = seedGroup.seedCount;
        }
    }

    public ObjectSet<SeedGroup> seedGroups;

    public int seedPopulation;

    public override void SetupSpeciesOrgan() {
        seedGroups = new ObjectSet<SeedGroup>();
    }
    
    public void Populate() {
        int spawnedSeeds = 0;
        while (spawnedSeeds < startingSeedCount) {
            int seedsInGroup = math.min(startingSeedCount - spawnedSeeds, Simulation.randomGenerator.NextInt(1, 7));
            SpawnOrganism(seedsInGroup);
            spawnedSeeds += seedsInGroup;
        }
    }

    public Organism SpawnOrganism(int seedCount) {
        SeedGroup seedGroup = new SeedGroup(Simulation.randomGenerator.NextFloat(0, timeRequirement), 0, float3.zero, 0, seedCount);
        seedGroups.Add(seedGroup, new SeedGroup());
        seedPopulation += seedGroup.seedCount;
        return seedGroup;
    }

    public Organism SpawnOrganism(float3 position, int zone, float distance, int seedCount) {
        SeedGroup seedGroup = new SeedGroup(Simulation.randomGenerator.NextFloat(0, timeRequirement), 0, float3.zero, 0, seedCount);
        seedGroups.Add(seedGroup, new SeedGroup());
        seedPopulation += seedGroup.seedCount;
        return seedGroup;
    }

    public void StartJobs(HashSet<Thread> activeThreads) {
        foreach (var organism in seedGroups.readObjects) {
            UpdateSeed(organism);
        }
    }

    public void UpdateSeed(Organism organismR) {
        SeedGroup seedGroupR = (SeedGroup)organismR;
        SeedGroup seedGroupW = seedGroups.GetWritable(seedGroupR);
        float newAge = organismR.age + GetPlantSpecies().GetEarth().simulationDeltaTime;
        if (newAge > timeMaximum) {
            seedGroups.Remove(seedGroupW);
            seedPopulation -= seedGroupR.seedCount;
        } else if (newAge >= timeRequirement
                   && earth.earthState.humidity > humidityRequirement
                   && earth.earthState.temperature > tempetureRequirement) {
            if (seedGerminationChance >= Simulation.randomGenerator.NextFloat(0, 100)) {
                if (seedGroupR.seedCount > 1) {
                    seedGroupW.seedCount = seedGroupR.seedCount - 1;
                } else {
                    seedGroups.Remove(seedGroupW);
                }
                seedPopulation -= 1;
                GetSpecies().SpawnOrganism(seedGroupR.position, seedGroupR.zone, seedDispertionRange);
            }
        } else {
            seedGroupW.age = newAge;
        }
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues,
        PlantSpecies.GrowthStageData previousStageValues) {
        return 0;
    }

    public override void GrowOrgan(Organism organismR, Plant plantR, Plant plantW, float growth) { }

    public override void EndUpdate() {
        base.EndUpdate();
        seedGroups.SwitchObjectSets();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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

        public SeedGroup(int seedCount) : base() {
            this.seedCount = seedCount;
        }

        public SeedGroup(float age, int zone, float3 position, float rotation, int seedCount) : base(age, zone, position, rotation) {
            this.seedCount = seedCount;
        }

        public SeedGroup(SeedGroup seedGroup) : base(seedGroup) {
            this.seedCount = seedGroup.seedCount;
        }
    }

    public ObjectSet<SeedGroup> seedGroups;

    public int seedPopulation {
        [MethodImpl(MethodImplOptions.Synchronized)] get;
        [MethodImpl(MethodImplOptions.Synchronized)] set; 
    }

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

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Organism SpawnOrganism(int seedCount) {
        SeedGroup seedGroup = new SeedGroup(seedCount);
        seedGroups.Add(seedGroup);
        seedGroup.age = Simulation.randomGenerator.NextFloat(0, timeRequirement);
        seedPopulation += seedGroup.seedCount;
        return seedGroup;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Organism SpawnOrganism(float3 position, int zone, float distance, int seedCount) {
        SeedGroup seedGroup = new SeedGroup(Simulation.randomGenerator.NextFloat(0, timeRequirement), 0, float3.zero, 0, seedCount);
        seedGroups.Add(seedGroup);
        seedPopulation += seedGroup.seedCount;
        return seedGroup;
    }

    public void StartJobs(HashSet<Thread> activeThreads, bool threaded) {
        var organismList = seedGroups.readObjects.Where(o => o != null).ToList();
        if (threaded) {
            ParallelLoopResult jobs = Parallel.ForEach<Organism>(organismList, UpdateSeed);
            while (!jobs.IsCompleted) { }
        } else {
            organismList.ForEach(UpdateSeed);
        }
    }

    public void UpdateSeed(Organism organismR) {
        SeedGroup seedGroupR = (SeedGroup)organismR;
        if (seedGroupR == null && organismR == null) throw new Exception("afljaslkjfjsaldf");
        SeedGroup seedGroupW = (SeedGroup)organismR.GetWritable();
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
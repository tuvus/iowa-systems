using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Earth;
using static PlantSpeciesAwns;
using static Species;

public class PlantSpeciesSeed : PlantSpeciesOrgan, IOrganismSpecies {
    public int startingSeedCount;
    public float humidityRequirement;
    public float tempetureRequirement;
    public float seedDispertionRange;
    [Tooltip("The time in days required before germination can begin")]
    public float timeRequirement;
    [Tooltip("The maximum time in days the seed can wait to germinate before it dies off")]
    public float timeMaximum;

    public float seedEnergyAmount;

    public OrganismList<Organism> seedList;
    public NativeArray<Organism> seeds;

    OrganismActionQueue<OrganismAction> seedActions;

    SpeciesSeedsUpdateJob speciesSeedsUpdateJob;

    public override void SetupSpeciesOrganArrays(IOrganismListExtender listExtender) {
        seedList = new OrganismList<Organism>(Math.Max(startingSeedCount * 2, 100), this);
        seeds = seedList.organisms;
        seedActions = new OrganismActionQueue<OrganismAction>(seedList);
        speciesSeedsUpdateJob = new SpeciesSeedsUpdateJob(GetPlantSpecies().speciesIndex);
    }

    public override void OnListUpdate() {
        seeds = seedList.organisms;
    }

    public void Populate() {
        for (int i = 0; i < startingSeedCount; i++) {
            SpawnOrganism();
        }
    }

    public int SpawnOrganism() {
        int seed = seedList.ActivateOrganism();
        seeds[seed] = new Organism(Simulation.randomGenerator.NextFloat(0, timeRequirement), 0, float3.zero, 0);
        //TODO: Need to add position and rotation here
        return seed;
    }

    public int SpawnOrganism(float3 position, int zone, float distance) {
        int seed = seedList.ActivateOrganism();
        seeds[seed] = new Organism(0, zone, position, 0);
        //TODO: Need to add position and rotation here
        return seed;
    }

    public struct SpeciesSeedsUpdateJob : IJobParallelFor {
        private int species;

        public SpeciesSeedsUpdateJob(int species) {
            this.species = species;
        }

        public JobHandle BeginJob() {
            return IJobParallelForExtensions.Schedule(this, ((PlantSpecies)SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species]).GetSpeciesSeeds().speciesSeed.seedList.activeOrganismCount, 10);
        }

        public void Execute(int index) {
            ((PlantSpecies)SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species]).GetSpeciesSeeds().speciesSeed.UpdateSeed(
                ((PlantSpecies)SpeciesManager.Instance.GetSpeciesMotor().GetAllSpecies()[species]).GetSpeciesSeeds().speciesSeed.seedList.activeOrganisms[index]);
        }
    }

    public override void StartJob(List<JobHandle> jobList) {
        speciesSeedsUpdateJob.BeginJob();
    }

    public void UpdateSeed(int seed) {
        seeds[seed] = new Organism(seeds[seed], seeds[seed].age + GetPlantSpecies().GetEarth().simulationDeltaTime);
        if (seeds[seed].age > timeMaximum) {
            seedActions.Enqueue(new OrganismAction(OrganismAction.Action.Die, seed));
            return;
        } else if (seeds[seed].age >= timeRequirement
            && earth.earthState.humidity > humidityRequirement
            && earth.earthState.temperature > tempetureRequirement) {
            seedActions.Enqueue(new OrganismAction(OrganismAction.Action.Reproduce, seed));
        }
    }

    public void UpdateSeedActions() {
        while (!seedActions.Empty()) {
            //No need to worry about deactivating an already inactive organism, it is handled in DeactivateActiveOrganism()
            if (seedActions.Peek().organism > seeds.Length || seedActions.Peek().organism < 0)
                print("Thread error");
            switch (seedActions.Peek().action) {
                case OrganismAction.Action.Starve:
                    seedList.DeactivateActiveOrganism(seedActions.Peek().organism);
                    break;
                case OrganismAction.Action.Die:
                    if (seedList.organismStatuses[seedActions.Peek().organism].spawned)
                        seedList.DeactivateActiveOrganism(seedActions.Peek().organism);
                    break;
                case OrganismAction.Action.Bite:
                    break;
                case OrganismAction.Action.Eat:
                    break;
                case OrganismAction.Action.Reproduce:
                    seedList.DeactivateActiveOrganism(seedActions.Peek().organism);
                    GrowSeed(seedActions.Peek());
                    break;
            }
            seedActions.Dequeue();
        }
    }

    public virtual void ReproduceOrganismParallel(OrganismAction action) {
        int organismsToReproduce = action.amount;
        for (; organismsToReproduce > 0; organismsToReproduce--) {
            if (SpawnOrganism(action.position, action.amount, action.floatValue) == -1) {
                organismsToReproduce--;
                break;
            }
        }
        if (organismsToReproduce > 0) {
            seedActions.Enqueue(new OrganismAction(action, organismsToReproduce));
        }
    }

    public virtual void KillOrganismParallel(OrganismAction action) {
        seedList.DeactivateActiveOrganismParallel(action.organism);
    }

    public void GrowSeed(OrganismAction organismAction) {
        KillOrganismParallel(new OrganismAction(OrganismAction.Action.Die, organismAction.organism));
        GetPlantSpecies().SpawnOrganism(organismAction.position, organismAction.zone, organismAction.floatValue);
    }

    public override void Deallocate() {
        seedList.Deallocate();
    }

    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        throw new NotImplementedException();
    }

    public override void GrowOrgan(int organism, float growth, ref float bladeArea, ref float stemHeight, ref float2 rootGrowth) {
        throw new NotImplementedException();
    }
}

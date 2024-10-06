using System;
using System.Collections.Generic;
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

    [Tooltip("The time in days required before germination can begin")]
    public float timeRequirement;

    [Tooltip("The maximum time in days the seed can wait to germinate before it dies off")]
    public float timeMaximum;

    public float seedEnergyAmount;

    public HashSet<Organism> seedList;
    public Organism[] seeds;

    List<OrganismAction> seedActions;

    public void Populate() {
        for (int i = 0; i < startingSeedCount; i++) {
            SpawnOrganism();
        }
    }

    public Organism SpawnOrganism() {
        Organism organism = new Organism(Simulation.randomGenerator.NextFloat(0, timeRequirement), 0, float3.zero, 0);

        //TODO: Need to add position and rotation here
        return organism;
    }

    public Organism SpawnOrganism(float3 position, int zone, float distance) {
        Organism organism = new Organism(0, zone, position, 0);
        //TODO: Need to add position and rotation here
        return organism;
    }

    public void UpdateSeed(int seed) {
        seeds[seed] = new Organism(seeds[seed], seeds[seed].age + GetPlantSpecies().GetEarth().simulationDeltaTime);
        if (seeds[seed].age > timeMaximum) {
            // seedActions.Enqueue(new OrganismAction(OrganismAction.Action.Die, seed));
            return;
        } else if (seeds[seed].age >= timeRequirement
                   && earth.earthState.humidity > humidityRequirement
                   && earth.earthState.temperature > tempetureRequirement) {
            // seedActions.Enqueue(new OrganismAction(OrganismAction.Action.Reproduce, seed));
        }
    }

    public void UpdateSeedActions() {
        // while (!seedActions.Empty()) {
        //     //No need to worry about deactivating an already inactive organism, it is handled in DeactivateActiveOrganism()
        //     if (seedActions.Peek().organism > seeds.Length || seedActions.Peek().organism < 0)
        //         print("Thread error");
        //     switch (seedActions.Peek().action) {
        //         case OrganismAction.Action.Starve:
        //             seedList.DeactivateActiveOrganism(seedActions.Peek().organism);
        //             break;
        //         case OrganismAction.Action.Die:
        //             if (seedList.organismStatuses[seedActions.Peek().organism].spawned)
        //                 seedList.DeactivateActiveOrganism(seedActions.Peek().organism);
        //             break;
        //         case OrganismAction.Action.Bite:
        //             break;
        //         case OrganismAction.Action.Eat:
        //             break;
        //         case OrganismAction.Action.Reproduce:
        //             seedList.DeactivateActiveOrganism(seedActions.Peek().organism);
        //             GrowSeed(seedActions.Peek());
        //             break;
        //     }
        //     seedActions.Dequeue();
        // }
    }

    public void GrowSeed(OrganismAction organismAction) {
        // KillOrganismParallel(new OrganismAction(OrganismAction.Action.Die, organismAction.organism));
        GetPlantSpecies().SpawnOrganism(organismAction.position, organismAction.zone, organismAction.floatValue);
    }


    public override float GetGrowthRequirementForStage(PlantSpecies.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues,
        PlantSpecies.GrowthStageData previousStageValues) {
        throw new NotImplementedException();
    }

    public override void GrowOrgan(Organism organismR, Plant plantR, Plant plantW, float growth) { }
}
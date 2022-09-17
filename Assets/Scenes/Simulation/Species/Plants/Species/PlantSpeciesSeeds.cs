using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int seedCount { private set; get; }

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

    public void SetupPlantSpeciesSeeds() {
        seedGerminationRequirement = new SeedGerminationRequirement(this);
    }

    public override void MakeOrganism(Plant plant) {
        SeedOrgan seeds = plant.gameObject.AddComponent<SeedOrgan>();
        seeds.SetupOrgan(this, plant);
    }

    public void Populate() {
        GetPlantSpecies().PreSpawn(startingSeedCount);
        for (int i = 0; i < startingSeedCount; i++) {
            Plant newseed = GetPlantSpecies().SpawnRandomSeed();
            newseed.SpawnSeedRandom(Simulation.randomGenerator.NextFloat(0, seedGerminationRequirement.timeRequirement));
            AddSeed();
        }
    }

    public void SpreadSeed(Plant parent) {
        Plant newSeed = GetPlantSpecies().SpawnSeed(parent, seedDispertionRange);
        newSeed.SpawnSeed();
        AddSeed();
    }

    public void AddSeed() {
        seedCount++;
    }

    public void SeedDeath() {
        seedCount--;
    }

    public void GrowSeed() {
        GetPlantSpecies().SeedGrownToPlant();
        seedCount--;
    }

    public override string GetOrganType() {
        return organType;
    }

    public override float GetGrowthRequirementForStage(Plant.GrowthStage stage, PlantSpecies.GrowthStageData thisStageValues, PlantSpecies.GrowthStageData previousStageValues) {
        if (stage == Plant.GrowthStage.Adult) {
            return awnMaxGrowth / growthModifier;
        }
        return 0;
    }


}
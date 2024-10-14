using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static PlantSpecies;
using static PlantSpeciesSeed;
using static Species;
using Plant = PlantSpecies.Plant;


public class PlantSpeciesAwns : PlantSpeciesOrgan {
    public float awnMaxGrowth;
    public int awnMaxSeedAmount;
    [Tooltip("The time the in days it takes until the awns release thier seeds")]
    public float awnSeedDispertionTime;
    public int awnSeedDispersalSuccessChance;
    public float seedDispertionRange;

    public PlantSpeciesSeed speciesSeed;

    public class Awn : ICloneable {
        public float awnsGrowth;
        public float timeUntilDispersion;

        public Awn(Organism organism, float awnsGrowth, float timeUntilDispersion){
            this.awnsGrowth = awnsGrowth;
            this.timeUntilDispersion = timeUntilDispersion;
        }

        public Awn(Awn awn){
            this.awnsGrowth = awn.awnsGrowth;
            this.timeUntilDispersion = awn.timeUntilDispersion;
        }

        public object Clone() {
            return MemberwiseClone();
        }
    }

    public override void SetupSpeciesOrgan() {
    }

    public void Populate() {
        speciesSeed.Populate();
    }

    public override void SpawnOrgan(Organism organism) {
        Plant plant = organism.GetOrgan<Plant>();
        if (plant.stage == GrowthStage.Adult) {
            if (Simulation.randomGenerator.NextBool()) {
                organism.AddOrgan(new Awn(organism, awnMaxGrowth, Simulation.randomGenerator.NextFloat(0, awnSeedDispertionTime)));
            } else {
                organism.AddOrgan(new Awn(organism, Simulation.randomGenerator.NextFloat(0, awnMaxGrowth), 0));
            }
        } else {
            organism.AddOrgan(new Awn(organism, 0, 0));
        }
    }

    public override void GrowOrgan(Organism organismR, Plant plantR, Plant plantW, float growth) {
        Awn awnR = organismR.GetOrgan<Awn>();
        Awn awnW = organismR.GetWritable().GetOrgan<Awn>();
        if (awnR.timeUntilDispersion > 0) {
            float newTimeUntilDispersion = math.max(0, awnR.timeUntilDispersion - GetPlantSpecies().GetEarth().simulationDeltaTime / 24);
            if (newTimeUntilDispersion > 0) {
                awnW.timeUntilDispersion = newTimeUntilDispersion;
                return;
            }
            awnW.timeUntilDispersion = 0;
            int seedsToDisperse = 0;
            for (int i = 0; i < awnMaxSeedAmount; i++) {
                if (Simulation.randomGenerator.NextInt(0, 100) < awnSeedDispersalSuccessChance) seedsToDisperse++;
            }
            if (seedsToDisperse != 0)
                GetPlantSpecies().GetPlantSpeciesSeeds().SpawnOrganism(organismR.position, organismR.zone, seedDispertionRange, seedsToDisperse);
        } else {
            float newGrowth = awnR.awnsGrowth + growth / 100;
            if (newGrowth >= awnMaxGrowth) {
                awnW.awnsGrowth = 0;
                awnW.timeUntilDispersion = awnSeedDispertionTime * Simulation.randomGenerator.NextFloat(.7f, 1.3f);
            } else {
                awnW.awnsGrowth = newGrowth;
            }
        }
    }

    public override string GetOrganType() {
        return organType;
    }

    public override float GetGrowthRequirementForStage(GrowthStage stage, GrowthStageData thisStageValues, GrowthStageData previousStageValues) {
        if (stage == GrowthStage.Adult) {
            return awnMaxGrowth / growthModifier;
        }
        return 0;
    }
}
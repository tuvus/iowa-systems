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

    public class Awn : MapObject<Organism> {
        public float awnsGrowth;
        public float timeUntilDispersion;

        public Awn(Organism organism, float awnsGrowth, float timeUntilDispersion) : base(organism){
            this.awnsGrowth = awnsGrowth;
            this.timeUntilDispersion = timeUntilDispersion;
        }

        public Awn(Awn awn) : base(awn.setObject) {
            this.awnsGrowth = awn.awnsGrowth;
            this.timeUntilDispersion = awn.timeUntilDispersion;
        }
    }

    public ObjectMap<Organism, Awn> awns;

    public override void SetupSpeciesOrgan() {
        awns = new ObjectMap<Organism, Awn>(GetPlantSpecies().organisms);
    }

    public void Populate() {
        speciesSeed.Populate();
    }

    public void SpawnAwns(Organism organism, Plant plant) {
        if (plant.stage == GrowthStage.Adult) {
            if (Simulation.randomGenerator.NextBool()) {
                Awn awn = new Awn(organism, awnMaxGrowth, Simulation.randomGenerator.NextFloat(0, awnSeedDispertionTime));
                awns.Add(awn, new Awn(awn));
            } else {
                Awn awn = new Awn(organism, Simulation.randomGenerator.NextFloat(0, awnMaxGrowth), 0);
                awns.Add(awn, new Awn(awn));
            }
        } else {
            Awn awn = new Awn(organism, 0, 0);
            awns.Add(awn, new Awn(awn));
        }
    }

    public override void GrowOrgan(Organism organismR, Plant plantR, Plant plantW, float growth) {
        Awn awnR = awns.GetReadable(organismR);
        Awn awnW = awns.GetWritable(organismR);
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

    public override void EndUpdate() {
        base.EndUpdate();
        awns.SwitchObjectSets();
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
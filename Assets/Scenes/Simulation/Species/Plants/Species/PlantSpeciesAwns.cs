using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static PlantSpecies;
using static Species;

public class PlantSpeciesAwns : PlantSpeciesOrgan {
    public float awnMaxGrowth;
    public int awnMaxSeedAmount;
    [Tooltip("The time the in days it takes until the awns release thier seeds")]
    public float awnSeedDispertionTime;
    public int awnSeedDispersalSuccessChance;
    public float seedDispertionRange;

    public PlantSpeciesSeed speciesSeed;

    public struct Awn {
        public float awnsGrowth;
        public float timeUntilDispersion;

        public Awn(float awnsGrowth, float timeUntilDispersion) {
            this.awnsGrowth = awnsGrowth;
            this.timeUntilDispersion = timeUntilDispersion;
        }
    }

    public OrganismAtribute<Awn> awnList;
    public NativeArray<Awn> awns;

    public override void SetupSpeciesOrganArrays(IOrganismListExtender listExtender) {
        speciesSeed.SetSpeciesScript(GetSpecies());
        awnList = new OrganismAtribute<Awn>(listExtender);
        awns = awnList.organismAttributes;
        speciesSeed.SetupSpeciesOrganArrays(listExtender);
    }

    public override void OnListUpdate() {
        awns = awnList.organismAttributes;
        speciesSeed.OnListUpdate();
    }

    public void Populate() {
        speciesSeed.Populate();
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
                GetPlantSpecies().organismActions.Enqueue(new OrganismAction(OrganismAction.Action.Reproduce, organism, GetPlantSpecies(), disperseSeeds, seedDispertionRange));
            }
            return;
        }
        awns[organism] = new Awn(awns[organism].awnsGrowth + growth / 100, 0);
        if (awns[organism].awnsGrowth >= awnMaxGrowth) {
            awns[organism] = new Awn(0, awnSeedDispertionTime);
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

    public override void Deallocate() {
        base.Deallocate();
        speciesSeed.Deallocate();
    }
}
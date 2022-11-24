using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static AnimalSpecies;
using static PlantSpeciesSeeds;

public class AnimalSpeciesReproductiveSystem : AnimalSpeciesOrgan {
    public GameObject reproductiveSystemPrefab;

    [Tooltip("The time to birth after conception in days")]
    public float birthTime;
    [Tooltip("The time after attempting reproduction in hours")]
    public float reproductionDelay;
    [Tooltip("The time age at which an animal is an adult and can reproduce in days")]
    public float reproductionAge;
    [Tooltip("The ammount of offspring to birth")]
    public int reproducionAmount;
    [Tooltip("The chance that each new offspring is successfully birthed")]
    public int birthSuccessPercent;

    public OrganismAtribute<ReproductiveSystem> reproductiveSystemsList;
    public NativeArray<ReproductiveSystem> reproductiveSystems;

    public struct ReproductiveSystem {
        [Tooltip("The sex of the animal, false = female, true = male")]
        public bool sex;
        [Tooltip("The time to birth after conception in days")]
        public float birthTime;
        [Tooltip("The time after attempting reproduction in hours")]
        public float reproductionDelay;

        public ReproductiveSystem(bool sex, float birthTime, float reproductionDelay) {
            this.sex = sex;
            this.birthTime = birthTime;
            this.reproductionDelay = reproductionDelay;
        }
    }

    public override void SetupSpeciesOrganArrays(IOrganismListExtender listExtender) {
        reproductiveSystemsList = new OrganismAtribute<ReproductiveSystem>(listExtender);
        reproductiveSystems = reproductiveSystemsList.organismAttributes;
    }

    public override void OnListUpdate() {
        NativeArray<ReproductiveSystem> oldReproductiveSystems = reproductiveSystems;
        reproductiveSystems = new NativeArray<ReproductiveSystem>(GetSpecies().organismList.GetListCapacity(), Allocator.Persistent);
        for (int i = 0; i < oldReproductiveSystems.Length; i++) {
            reproductiveSystems[i] = oldReproductiveSystems[i];
        }
        oldReproductiveSystems.Dispose();
    }

    public GrowthStage SpawnReproductive(int organism) {
        reproductiveSystems[organism] = new ReproductiveSystem(Simulation.randomGenerator.NextBool(),0, reproductionDelay * Simulation.randomGenerator.NextFloat(0f, 1.2f));
        if (GetAnimalSpecies().organisms[organism].age >= reproductionAge) {
            return GrowthStage.Adult;
        }
        return GrowthStage.Juvinile;
    }
}
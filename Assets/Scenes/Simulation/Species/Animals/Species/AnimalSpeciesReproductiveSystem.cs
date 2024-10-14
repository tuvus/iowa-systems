using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static AnimalSpecies;
using Organism = Species.Organism;

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


    public class ReproductiveSystem : ICloneable {
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

        public object Clone() {
            return MemberwiseClone();
        }
    }

    public override void SetupSpeciesOrgan() {
    }

    public GrowthStage SpawnReproductive(Organism organism) {
        organism.AddOrgan(new ReproductiveSystem(Simulation.randomGenerator.NextBool(), 0,
            reproductionDelay * Simulation.randomGenerator.NextFloat(0f, 1.2f)));
        if (organism.age >= reproductionAge)
            return GrowthStage.Adult;
        return GrowthStage.Juvinile;
    }
}
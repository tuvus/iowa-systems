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

    public ObjectMap<Organism, ReproductiveSystem> reproductiveSystems;

    public class ReproductiveSystem : MapObject<Organism> {
        [Tooltip("The sex of the animal, false = female, true = male")]
        public bool sex;
        [Tooltip("The time to birth after conception in days")]
        public float birthTime;
        [Tooltip("The time after attempting reproduction in hours")]
        public float reproductionDelay;

        public ReproductiveSystem(Organism organism, bool sex, float birthTime, float reproductionDelay) : base(organism) {
            this.sex = sex;
            this.birthTime = birthTime;
            this.reproductionDelay = reproductionDelay;
        }

        public ReproductiveSystem(ReproductiveSystem reproductiveSystem) : base(reproductiveSystem.setObject) {
            this.sex = reproductiveSystem.sex;
            this.birthTime = reproductiveSystem.birthTime;
            this.reproductionDelay = reproductiveSystem.reproductionDelay;
        }
    }

    public override void SetupSpeciesOrgan() {
        reproductiveSystems = new ObjectMap<Organism, ReproductiveSystem>(GetSpecies().organisms);
    }

    public GrowthStage SpawnReproductive(Organism organism) {
        ReproductiveSystem reproductive = new ReproductiveSystem(organism, Simulation.randomGenerator.NextBool(), 0,
            reproductionDelay * Simulation.randomGenerator.NextFloat(0f, 1.2f));
        reproductiveSystems.Add(reproductive, new ReproductiveSystem(reproductive));
        if (organism.age >= reproductionAge)
            return GrowthStage.Adult;
        return GrowthStage.Juvinile;
    }

    public override void KillOrganism(Organism organism) {
        base.KillOrganism(organism);
        reproductiveSystems.Remove(organism);
    }

    public override void EndUpdate() {
        base.EndUpdate();
        reproductiveSystems.SwitchObjectSets();
    }
}
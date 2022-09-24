using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

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

    public NativeArray<ReproductiveSystem> reproductiveSystems;

    public struct ReproductiveSystem {
        [Tooltip("The time to birth after conception in days")]
        public float birthTime;
        [Tooltip("The time after attempting reproduction in hours")]
        public float reproductionDelay;
    }

    public override void SetupSpeciesOrganArrays(int arraySize) {
        reproductiveSystems = new NativeArray<ReproductiveSystem>(arraySize, Allocator.Persistent);
    }

    public void OnDestroy() {
        if (reproductiveSystems.IsCreated)
            reproductiveSystems.Dispose();
    }
}
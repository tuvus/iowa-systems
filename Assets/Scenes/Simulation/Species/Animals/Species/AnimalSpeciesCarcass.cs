using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using static Species;

public class AnimalSpeciesCarcass : AnimalSpeciesOrgan {
    [Tooltip("The time it takes a corpse to deteriorate in days")]
    public float deteriationTime;

    public AnimalSpeciesCarcass() {

    }
    
    public Organism SpawnOrganism() {
        Organism organism = new Organism(0, 0, float3.zero, 0);
        //TODO: Add position and rotation
        return organism;
    }

    public Organism SpawnOrganism(float3 position, int zone, float distance) {
        Organism organism = new Organism(0, zone, position, 0);
        //TODO: Add position and rotation
        return organism;
    }
}
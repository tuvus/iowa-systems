using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AnimalSpeciesNose : AnimalSpeciesOrgan {
    public GameObject nose;

    [Tooltip("How far away the animal can smell in meters")]
    public float smellRange;

    public override void SetupSpeciesOrganArrays(int arraySize) {
        //No aditional data needs to be stored here, it is currently being stored in the Plant struct
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AnimalSpeciesEars : AnimalSpeciesOrgan {
    [Tooltip("How far away the animal can hear in meters")]
    public float hearRange;

    public override void SetupSpeciesOrganArrays(int arraySize) {
        //No aditional data needs to be stored here, it is currently being stored in the Plant struct
    }
}

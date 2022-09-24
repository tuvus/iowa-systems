using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AnimalSpeciesMouth : AnimalSpeciesOrgan {
    [Tooltip("How big each bite taken is in kilograms")]
    public float biteSize;
    [Tooltip("How long it takes to eat each bite")]
    public float eatSpeed;
    [Tooltip("How far away the organism can bite in meters")]
    public float eatRange;


    public override void SetupSpeciesOrganArrays(int arraySize) {
        //No aditional data needs to be stored here, it is currently being stored in the Plant struct
    }
}

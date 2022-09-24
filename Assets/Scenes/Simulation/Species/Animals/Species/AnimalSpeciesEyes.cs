using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AnimalSpeciesEyes : AnimalSpeciesOrgan {
    [Tooltip("How far away the animal can see in meters")]
    public float sightRange;
    [Tooltip("The eye type that the animal has")]
    public EyesOrgan.EyeTypes eyeType;


    public override void SetupSpeciesOrganArrays(int arraySize) {
        //No aditional data needs to be stored here, it is currently being stored in the Plant struct
    }
}

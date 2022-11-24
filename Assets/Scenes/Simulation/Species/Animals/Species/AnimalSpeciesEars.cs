using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AnimalSpeciesEars : AnimalSpeciesOrgan {
    [Tooltip("How far away the animal can hear in meters")]
    public float hearRange;
}

﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class AnimalSpeciesEyes : AnimalSpeciesOrgan {
    public enum EyeTypes {
        Foward = 0,
        Side = 1,
    }

    [Tooltip("How far away the animal can see in meters")]
    public float sightRange;
    [Tooltip("The eye type that the animal has")]
    public EyeTypes eyeType;
}

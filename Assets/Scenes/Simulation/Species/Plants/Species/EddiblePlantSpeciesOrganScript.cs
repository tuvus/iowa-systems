using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EddiblePlantSpeciesOrgan : PlantSpeciesOrgan {
    public override string GetOrganType() {
        return organType;
    }
}

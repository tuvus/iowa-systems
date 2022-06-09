using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EddiblePlantSpeciesOrganScript : PlantSpeciesOrgan {
    
    internal void MakeEddibleOrganism(EddiblePlantOrgan eddiblePlantOrgan, Plant plant) {
        plant.eddibleOrgans.Add(eddiblePlantOrgan);
    }

    public override string GetOrganType() {
        return organType;
    }
}

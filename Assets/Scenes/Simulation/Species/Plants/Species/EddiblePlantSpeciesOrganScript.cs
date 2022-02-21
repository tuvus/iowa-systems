using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EddiblePlantSpeciesOrganScript : BasicPlantSpeciesOrganScript {
    
    internal void MakeEddibleOrganism(EddiblePlantOrganScript eddiblePlantOrgan, BasicOrganismScript organismScript) {
        organismScript.GetComponent<PlantScript>().eddibleOrgans.Add(eddiblePlantOrgan);
    }

    public override string GetOrganType() {
        return organType;
    }
}

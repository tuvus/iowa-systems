using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicPlantOrganScript : BasicOrganScript {
    internal BasicPlantSpeciesOrganScript basicPlantSpeciesOrganScript;
    internal PlantScript plantScript;

    internal override void SetupOrgan(BasicSpeciesOrganScript _basicSpeciesOrganScript) {
        basicPlantSpeciesOrganScript = (BasicPlantSpeciesOrganScript)_basicSpeciesOrganScript;
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
    }
}

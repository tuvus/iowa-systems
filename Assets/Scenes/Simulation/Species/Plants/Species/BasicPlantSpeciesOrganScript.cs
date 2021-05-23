using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicPlantSpeciesOrganScript : BasicSpeciesOrganScript {
    internal PlantSpeciesScript plantSpecies;

    public override void SetSpeciesScript(BasicSpeciesScript _species) {
        plantSpecies = (PlantSpeciesScript)_species;
    }
}

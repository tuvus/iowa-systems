using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicPlantSpeciesOrganScript : BasicSpeciesOrganScript {
    internal PlantSpecies plantSpecies;

    public override void SetSpeciesScript(BasicSpeciesScript _species) {
        plantSpecies = (PlantSpecies)_species;
    }

    public virtual string GetOrganType() {
        return null;
    }
}

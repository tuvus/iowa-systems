using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicPlantSpeciesOrganScript : BasicSpeciesOrganScript {
    internal PlantSpecies plantSpecies;
    public string organType;
    public int organFoodIndex;
    public override void SetSpeciesScript(BasicSpeciesScript _species) {
        plantSpecies = (PlantSpecies)_species;
    }

    public void SetupSpeciesOrganFoodType() {
        organFoodIndex = plantSpecies.GetEarthScript().GetIndexOfFoodType(GetOrganType());
    }

    public virtual string GetOrganType() {
        return null;
    }

    public int GetOrganFoodIndex() {
        return organFoodIndex;
    }
}

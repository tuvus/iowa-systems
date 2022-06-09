using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpeciesOrgan : MonoBehaviour {
    private Species species;

    public void SetSpeciesScript(Species species) {
        this.species = species;
    }

    public Species GetSpecies() {
        return species;
    }
}

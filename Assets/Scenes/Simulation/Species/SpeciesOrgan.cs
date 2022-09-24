using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpeciesOrgan : MonoBehaviour {
    private Species species;

    public void SetSpeciesScript(Species species) {
        this.species = species;
    }

    public virtual void SetupSpeciesOrganArrays(int arraySize) { }

    public Species GetSpecies() {
        return species;
    }
}
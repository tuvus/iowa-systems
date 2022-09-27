using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public abstract class SpeciesOrgan : MonoBehaviour {
    private Species species;

    public void SetSpeciesScript(Species species) {
        this.species = species;
    }

    public virtual void SetupSpeciesOrganArrays(int arraySize) { }

    public virtual void IncreaseOrganismSize(int newSize) { }


    public virtual JobHandle? StartJob() {
        return null;
    }

    public Species GetSpecies() {
        return species;
    }
}
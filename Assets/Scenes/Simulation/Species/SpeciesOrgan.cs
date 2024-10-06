using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public abstract class SpeciesOrgan : MonoBehaviour {
    private Species species;

    public void SetSpeciesScript(Species species) {
        this.species = species;
    }

    public virtual void SetupSpeciesOrgan() { }

    public virtual void KillOrganism(Species.Organism organism) { }
    
    public virtual void EndUpdate() { }
    
    public Species GetSpecies() {
        return species;
    }
}
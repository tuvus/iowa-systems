using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public abstract class SpeciesOrgan : MonoBehaviour, IOrganismListCapacityChange {
    private Species species;

    public void SetSpeciesScript(Species species) {
        this.species = species;
    }

    public virtual void SetupSpeciesOrganArrays(IOrganismListExtender listExtender) { }

    public virtual void OnListUpdate() { }

    public virtual void StartJob(List<JobHandle> jobList) {
    }

    public Species GetSpecies() {
        return species;
    }

    public virtual void Deallocate() {
    }
}
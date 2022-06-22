using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

public abstract class JobController : MonoBehaviour {
    private Species species;

    internal JobHandle job;

    public void SetUpJobController(Species species) {
        this.species = species;
        Allocate();
    }

    public abstract void Allocate();

    public abstract JobHandle StartUpdateJob();

    internal abstract void OnDestroy();

    public Species GetSpecies() {
        return species;
    }

    public Earth GetEarth() {
        return Simulation.Instance.GetEarth();
    }
}

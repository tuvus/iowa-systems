using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

public abstract class BasicJobController : MonoBehaviour {
    internal EarthScript earth;
    internal BasicSpeciesScript speciesScript;

    internal JobHandle job;

    public void SetUpJobController(EarthScript earth) {
        this.earth = earth;
        Allocate();
    }

    public void SetUpJobController(BasicSpeciesScript speciesScript, EarthScript earth) {
        this.earth = earth;
        this.speciesScript = speciesScript;
        SetUpSpecificJobController(speciesScript);
        Allocate();
    }

    internal virtual void SetUpSpecificJobController(BasicSpeciesScript speciesScript) { 
    }

    public abstract void Allocate();

    public abstract JobHandle StartUpdateJob();

    internal abstract void OnDestroy();
}

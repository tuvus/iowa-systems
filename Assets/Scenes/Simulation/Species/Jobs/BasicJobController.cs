using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

public abstract class BasicJobController : MonoBehaviour {
    internal BasicSpeciesScript speciesScript;

    internal JobHandle job;

    public void SetUpJobController(BasicSpeciesScript speciesScript) {
        this.speciesScript = speciesScript;
        SetUpSpecificJobController(speciesScript);
        Allocate();
    }

    internal abstract void SetUpSpecificJobController(BasicSpeciesScript speciesScript);

    public abstract void Allocate();

    public abstract JobHandle StartJob();

    internal abstract void OnDestroy();
}

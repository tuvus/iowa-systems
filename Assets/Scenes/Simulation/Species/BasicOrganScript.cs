using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicOrganScript : MonoBehaviour {
    internal BasicSpeciesOrganScript basicSpeciesOrganScript;
    internal BasicOrganismScript basicOrganismScript;
    public void SetupBasicOrgan(BasicSpeciesOrganScript _basicSpeciesOrganScript) {
        basicSpeciesOrganScript = _basicSpeciesOrganScript;
        basicOrganismScript = GetComponentInParent<BasicOrganismScript>();
        SetupOrgan(_basicSpeciesOrganScript);
        SetUpSpecificOrgan();
    }

    internal abstract void SetupOrgan(BasicSpeciesOrganScript _basicSpeciesOrganScript);

    internal abstract void SetUpSpecificOrgan();
}

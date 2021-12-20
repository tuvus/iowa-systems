using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicSpeciesOrganScript : MonoBehaviour {
    internal BasicSpeciesScript speciesScript;
    public abstract void MakeOrganism(BasicOrganismScript newOrganism);

    public void SetBasicSpeciesScript(BasicSpeciesScript species) {
        speciesScript = species;
        SetSpeciesScript(species);
    }

    public abstract void SetSpeciesScript(BasicSpeciesScript species);
}

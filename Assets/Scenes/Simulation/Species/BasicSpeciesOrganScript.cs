using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicSpeciesOrganScript : MonoBehaviour {
    internal BasicSpeciesScript speciesScript;
    public abstract void MakeOrganism(BasicOrganismScript _newOrganism);

    public void SetBasicSpeciesScript(BasicSpeciesScript _species) {
        speciesScript = _species;
        SetSpeciesScript(_species);
    }

    public abstract void SetSpeciesScript(BasicSpeciesScript _species);
}

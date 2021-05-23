using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAnimalOrganScript : BasicOrganScript {
    internal BasicAnimalSpeciesOrganScript basicAnimalSpeciesOrganScript;
    internal BasicAnimalScript basicAnimalScript;
    internal BasicBehaviorScript behaviorScript;

    internal override void SetupOrgan(BasicSpeciesOrganScript _basicSpeciesOrganScript) {
        basicAnimalSpeciesOrganScript = (BasicAnimalSpeciesOrganScript)_basicSpeciesOrganScript;
        basicAnimalScript = basicOrganismScript.GetComponent<BasicAnimalScript>();
        behaviorScript = basicOrganismScript.GetComponent<BasicBehaviorScript>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAnimalOrganScript : BasicOrganScript {
    internal BasicAnimalSpeciesOrganScript basicAnimalSpeciesOrganScript;
    internal AnimalScript animalScript;

    internal override void SetupOrgan(BasicSpeciesOrganScript basicSpeciesOrganScript) {
        basicAnimalSpeciesOrganScript = (BasicAnimalSpeciesOrganScript)basicSpeciesOrganScript;
        animalScript = (AnimalScript)basicOrganismScript;
        animalScript.AddOrgan(this);
    }

    public abstract void UpdateOrgan();

    internal override void RemoveFoodTypeFromZone(int zoneIndex) {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleReproductiveSystem : BasicAnimalOrganScript {

    public ReproductiveSystem reproductive;

    public float timeAfterReproduction;

    internal override void SetUpSpecificOrgan() {
        timeAfterReproduction = reproductive.animalSpeciesReproductive.reproductionDelay * Random.Range(0.0f, .2f);
    }

    void FixedUpdate() {
        UpdateReproduction();
    }

    void UpdateReproduction() {
        if (timeAfterReproduction > 0) {
            timeAfterReproduction -= Time.fixedDeltaTime * .2f;
            if (timeAfterReproduction <= 0)
                timeAfterReproduction = 0;
        }
    }

    public bool Concieve() {
        if (timeAfterReproduction <= 0 && basicAnimalScript.mate != null) {
            basicAnimalScript.mate.behavior.reproductive.femaleReproductiveSystem.Concieve();
            timeAfterReproduction = reproductive.animalSpeciesReproductive.reproductionDelay * Random.Range(0.0f, .3f);
            return true;
        }
        return false;
    }

    public bool ReadyToConcieve() {
        if (timeAfterReproduction <= 0 && basicAnimalScript.mate != null) {
            return false;
        }
        return false;
    }
}

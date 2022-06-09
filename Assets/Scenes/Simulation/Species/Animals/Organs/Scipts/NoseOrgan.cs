using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoseOrgan : AnimalOrgan {

    public void SetupOrgan(AnimalSpeciesNose speciesNose, Animal animal) {
        base.SetupOrgan(speciesNose, animal);
    }
    public override void UpdateOrgan() {
    }

    public override void ResetOrgan() {
    }

    public AnimalSpeciesNose GetSpeciesNose() {
        return (AnimalSpeciesNose)GetSpeciesOrgan();
    }
}

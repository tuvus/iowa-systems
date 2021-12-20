using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesBlade : BasicPlantSpeciesOrganScript {
    public float maxBladeArea;
    public string organType;

    public override void MakeOrganism(BasicOrganismScript newOrganism) {
        BladeOrgan bladeOrgan = newOrganism.gameObject.AddComponent<BladeOrgan>();
        bladeOrgan.SetupBasicOrgan(this);
        bladeOrgan.speciesBlade = this;
    }

    public override string GetOrganType() {
        return organType;
    }
}

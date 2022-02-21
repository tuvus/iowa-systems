using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesBlade : EddiblePlantSpeciesOrganScript {
    public float maxBladeArea;

    public override void MakeOrganism(BasicOrganismScript newOrganism) {
        BladeOrgan bladeOrgan = newOrganism.gameObject.AddComponent<BladeOrgan>();
        bladeOrgan.SetupBasicOrgan(this, newOrganism);
        bladeOrgan.speciesBlade = this;
        MakeEddibleOrganism(bladeOrgan,newOrganism);
    }
}
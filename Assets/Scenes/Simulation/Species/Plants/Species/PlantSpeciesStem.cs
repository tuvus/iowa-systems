using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesStem : EddiblePlantSpeciesOrganScript {
    public float maxStemHeight;

    public override void MakeOrganism(BasicOrganismScript newOrganism) {
        StemOrgan stemOrgan = newOrganism.gameObject.AddComponent<StemOrgan>();
        stemOrgan.SetupBasicOrgan(this, newOrganism);
        stemOrgan.speciesStem = this;
        MakeEddibleOrganism(stemOrgan, newOrganism);
    }
}

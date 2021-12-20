using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesStem : BasicPlantSpeciesOrganScript {
    public float maxStemHeight;
    public string organType;


    public override void MakeOrganism(BasicOrganismScript newOrganism) {
        StemOrgan stemOrgan = newOrganism.gameObject.AddComponent<StemOrgan>();
        stemOrgan.SetupBasicOrgan(this);
        stemOrgan.speciesStem = this;
    }

    public override string GetOrganType() {
        return organType;
    }
}

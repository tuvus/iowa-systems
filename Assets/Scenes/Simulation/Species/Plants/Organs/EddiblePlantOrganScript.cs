using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EddiblePlantOrganScript : BasicPlantOrganScript {
    internal override void SetupOrgan(BasicSpeciesOrganScript basicSpeciesOrganScript) {
        base.SetupOrgan(basicSpeciesOrganScript);
        plantScript.eddibleOrgans.Add(this);
    }

    public abstract float EatPlantOrgan(AnimalScript animal, float biteSize);

    public override void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().AddFoodTypeToZone(zoneIndex, GetFoodIndex(), dataLocation);
    }

    public override void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().RemoveFoodTypeFromZone(zoneIndex, GetFoodIndex(), dataLocation);
    }

    internal abstract int GetFoodIndex();
}

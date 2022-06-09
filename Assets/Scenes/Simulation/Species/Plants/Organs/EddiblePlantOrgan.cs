using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EddiblePlantOrgan : PlantOrgan {
    public void SetupOrgan(PlantSpeciesOrgan plantSpeciesOrgan, Plant plant) {
        base.SetupOrgan(plantSpeciesOrgan, plant);
        plant.eddibleOrgans.Add(this);
    }

    public abstract float EatPlantOrgan(Animal animal, float biteSize);

    public override void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().AddFoodTypeToZone(zoneIndex, GetFoodIndex(), dataLocation);
    }

    public override void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().RemoveFoodTypeFromZone(zoneIndex, GetFoodIndex(), dataLocation);
    }

    internal abstract int GetFoodIndex();
}

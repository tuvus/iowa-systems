using Unity.Mathematics;
using UnityEngine;

public class RootOrgan : PlantOrgan {
    public float rootDensity;

    public override void ResetOrgan() {
    }

    public override void GrowOrgan(float growth) {
        GetPlant().ChangeRootGrowth(new float2 (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].rootGrowth.x, GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].rootGrowth.y + (growth * GetPlantSpeciesRoots().growthModifier)));
    }

    public override void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().AddFoodTypeToZone(zoneIndex, GetPlantSpeciesRoots().organFoodIndex, dataLocation);
    }

    public override void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().RemoveFoodTypeFromZone(zoneIndex, GetPlantSpeciesRoots().organFoodIndex, dataLocation);
    }

    public PlantSpeciesRoots GetPlantSpeciesRoots() {
        return (PlantSpeciesRoots)GetPlantSpeciesOrgan();
    }
}

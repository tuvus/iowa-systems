using Unity.Mathematics;
using UnityEngine;

public class RootOrgan : BasicPlantOrganScript {
    public PlantSpeciesRoots speciesRoots;

    public float rootDensity;

    internal override void SetUpSpecificOrgan() {
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
    }

    public override void ResetOrgan() {
    }

    public override void GrowOrgan(float growth) {
        plantScript.ChangeRootGrowth(new float2 (plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].rootGrowth.x, plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].rootGrowth.y + (growth * speciesRoots.growthModifier)));
    }

    public override void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().AddFoodTypeToZone(zoneIndex, speciesRoots.organFoodIndex, dataLocation);
    }

    public override void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().RemoveFoodTypeFromZone(zoneIndex, speciesRoots.organFoodIndex, dataLocation);
    }
}

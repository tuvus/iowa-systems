using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BladeOrgan : BasicPlantOrganScript {
    public PlantSpeciesBlade speciesBlade;
    public float bladeArea;

    internal override void SetUpSpecificOrgan() {
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
    }

    public override void ResetOrgan() {
        bladeArea = 0;
    }

    public override void UpdateGrowthPriority() {
        switch (plantScript.stage) {
            case PlantScript.GrowthStage.Germinating:
                growthPriority = 0f;
                break;
            case PlantScript.GrowthStage.Sprout:
                growthPriority = 1f;
                break;
            case PlantScript.GrowthStage.Seedling:
                growthPriority = 1f;
                break;
            case PlantScript.GrowthStage.Youngling:
                growthPriority = 1f;
                break;
            case PlantScript.GrowthStage.Adult:
                growthPriority = 0f;
                break;
        };
    }

    public override void GrowOrgan(float growth) {
        bladeArea += growth;
    }

    public override float GetBladeArea() {
        return bladeArea;
    }

    public override void AddToZone(int zoneIndex, int plantDataIndex) {
        plantScript.GetEarthScript().GetZoneController().organismsByFoodTypeInZones.Add(plantScript.GetEarthScript().GetIndexOfFoodType(speciesBlade.GetOrganType()), plantDataIndex);
    }

    public override void RemoveFromZone(int zoneIndex) {
        RemoveFoodTypeFromZone(zoneIndex, plantScript.GetEarthScript().GetIndexOfFoodType(speciesBlade.organType));
    }
}

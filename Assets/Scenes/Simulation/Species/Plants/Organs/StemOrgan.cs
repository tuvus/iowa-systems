using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StemOrgan : BasicPlantOrganScript {
    public PlantSpeciesStem speciesStem;

    public float stemheight;

    internal override void SetUpSpecificOrgan() {
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
    }

    public override void ResetOrgan() {
        stemheight = 0;
    }

    public override void UpdateGrowthPriority() {
        switch (plantScript.stage) {
            case PlantScript.GrowthStage.Seed:
                growthPriority = 0;
                break;
            case PlantScript.GrowthStage.Germinating:
                growthPriority = 0f;
                break;
            case PlantScript.GrowthStage.Sprout:
                growthPriority = .28f;
                break;
            case PlantScript.GrowthStage.Seedling:
                growthPriority = .75f;
                break;
            case PlantScript.GrowthStage.Youngling:
                growthPriority = .8f;
                break;
            case PlantScript.GrowthStage.Adult:
                growthPriority = 0f;
                break;
        };
    }

    public override void GrowOrgan(float growth) {
        stemheight += growth;
    }

    public override float GetStemheight() {
        return stemheight;
    }

    public override float GetBladeArea() {
        return stemheight / 10;
    }

    public override void AddToZone(int zoneIndex, int plantDataIndex) {
        plantScript.GetEarthScript().GetZoneController().organismsByFoodTypeInZones.Add(plantScript.GetEarthScript().GetIndexOfFoodType(speciesStem.GetOrganType()), plantDataIndex);
    }

    public override void RemoveFromZone(int zoneIndex) {
        RemoveFoodTypeFromZone(zoneIndex, plantScript.GetEarthScript().GetIndexOfFoodType(speciesStem.organType));
    }
}

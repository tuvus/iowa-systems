using Unity.Mathematics;
using UnityEngine;

public class RootOrgan : BasicPlantOrganScript {
    public PlantSpeciesRoots speciesRoots;

    public float2 rootGrowth;
    public float rootDensity;

    internal override void SetUpSpecificOrgan() {
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
    }

    public override void SpawnOrganismAdult() {
        rootGrowth = new float2(speciesRoots.rootGrowthMax.x, plantScript.plantSpecies.growthStages[(int)plantScript.stage].rootDepth);
        if (rootGrowth.y > 0)
            GrowOrgan(0);
    }


    public override void ResetOrgan() {
        rootGrowth = new float2(speciesRoots.rootGrowthMax.x, 0);
        rootDensity = 0;
    }

    public override void UpdateGrowthPriority() {
        switch (plantScript.stage) {
            case PlantScript.GrowthStage.Seed:
                growthPriority = 0;
                break;
            case PlantScript.GrowthStage.Germinating:
                growthPriority = 1;
                break;
            case PlantScript.GrowthStage.Sprout:
                growthPriority = .2f;
                break;
            case PlantScript.GrowthStage.Seedling:
                growthPriority = .84f;
                break;
            case PlantScript.GrowthStage.Youngling:
                growthPriority = .3f;
                break;
            case PlantScript.GrowthStage.Adult:
                growthPriority = 0f;
                break;
        }
    }

    public override void GrowOrgan(float growth) {
        rootGrowth.y += growth;
    }

    public override float2 GetRootGrowth() {
        return rootGrowth;
    }

    public override void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().AddFoodTypeToZone(zoneIndex, speciesRoots.organFoodIndex, dataLocation);
    }

    public override void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        GetZoneController().RemoveFoodTypeFromZone(zoneIndex, speciesRoots.organFoodIndex, dataLocation);
    }
}

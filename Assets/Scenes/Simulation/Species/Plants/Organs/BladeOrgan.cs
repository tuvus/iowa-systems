using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BladeOrgan : EddiblePlantOrganScript {
    public PlantSpeciesBlade speciesBlade;
    public float bladeArea;

    internal override void SetUpSpecificOrgan() {
    }

    public override void SpawnOrganismAdult() {
        bladeArea = plantScript.plantSpecies.growthStages[(int)plantScript.stage].bladeArea;
    }

    public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
        if (bladeArea > 0)
            GrowOrgan(0);
    }

    public override void ResetOrgan() {
        base.ResetOrgan();
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

    public override float EatPlantOrgan(AnimalScript animal, float biteSize) {
        if (!spawned)
            return 0;
        for (int i = 0; i < animal.animalSpecies.eddibleFoodTypes.Length; i++) {
            if (animal.animalSpecies.eddibleFoodTypes[i] == speciesBlade.organFoodIndex) {
                if (bladeArea > biteSize) {
                    bladeArea -= biteSize;
                    return biteSize;
                }
                float foodReturn = bladeArea;
                ResetOrgan();
                return foodReturn;

            }
        }
        return 0;
    }

    public override void GrowOrgan(float growth) {
        bladeArea += growth;
        if (!spawned && bladeArea > 0)
            Spawn();
    }

    public override float GetBladeArea() {
        return bladeArea;
    }

    internal override int GetFoodIndex() {
        return speciesBlade.organFoodIndex;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StemOrgan : EddiblePlantOrganScript {
    public PlantSpeciesStem speciesStem;

    public float stemheight;

    internal override void SetUpSpecificOrgan() {
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
    }

    public override void SpawnOrganismAdult() {
        stemheight = plantScript.plantSpecies.growthStages[(int)plantScript.stage].stemHeight;
    }

    public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
        if (stemheight > 0) {
            GrowOrgan(0);
        }
    }

    public override void ResetOrgan() {
        base.ResetOrgan();
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

    public override float EatPlantOrgan(AnimalScript animal, float biteSize) {
        if (!spawned)
            return 0;
        for (int i = 0; i < animal.animalSpecies.eddibleFoodTypes.Length; i++) {
            if (animal.animalSpecies.eddibleFoodTypes[i] == speciesStem.organFoodIndex) {
                if (stemheight > biteSize) {
                    stemheight -= biteSize;
                    return biteSize;
                }
                float foodReturn = stemheight;
                ResetOrgan();
                plantScript.KillOrganism();
                return foodReturn;

            }
        }
        return 0;
    }

    public override void GrowOrgan(float growth) {
        stemheight += growth;
        if (!spawned && stemheight > 0) { 
            Spawn();
        }
    }

    public override float GetStemheight() {
        return stemheight;
    }

    public override float GetBladeArea() {
        return stemheight / 10;
    }

    internal override int GetFoodIndex() {
        return speciesStem.organFoodIndex;
    }
}

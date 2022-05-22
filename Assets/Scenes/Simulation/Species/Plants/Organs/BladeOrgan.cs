using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BladeOrgan : EddiblePlantOrganScript {
    public PlantSpeciesBlade speciesBlade;

    internal override void SetUpSpecificOrgan() {
    }

    public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
        if (plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].bladeArea > 0)
            Spawn();
    }

    public override float EatPlantOrgan(AnimalScript animal, float biteSize) {
        if (!spawned)
            return 0;
        for (int i = 0; i < animal.animalSpecies.eddibleFoodTypes.Length; i++) {
            if (animal.animalSpecies.eddibleFoodTypes[i] == speciesBlade.organFoodIndex) {
                if (plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].bladeArea > biteSize) {
                    plantScript.ChangeBladeArea(plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].bladeArea - biteSize);
                    return biteSize;
                }
                float foodReturn = plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].bladeArea;
                ResetOrgan();
                plantScript.ChangeBladeArea(0);
                return foodReturn;

            }
        }
        return 0;
    }

    public override void GrowOrgan(float growth) {
        plantScript.ChangeBladeArea(plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].bladeArea + (growth * speciesBlade.growthModifier));
        if (!spawned && plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].bladeArea > 0)
            Spawn();
    }

    internal override int GetFoodIndex() {
        return speciesBlade.organFoodIndex;
    }
}

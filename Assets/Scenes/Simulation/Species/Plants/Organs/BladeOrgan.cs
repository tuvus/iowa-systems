using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BladeOrgan : EddiblePlantOrgan {

    public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
        if (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].bladeArea > 0)
            Spawn();
    }

    public override float EatPlantOrgan(Animal animal, float biteSize) {
        if (!spawned || biteSize <= 0)
            return 0;
        for (int i = 0; i < animal.animalSpecies.eddibleFoodTypes.Length; i++) {
            if (animal.animalSpecies.eddibleFoodTypes[i] == GetPlantSpeciesBlade().organFoodIndex) {
                if (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].bladeArea > biteSize) {
                    GetPlant().ChangeBladeArea(GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].bladeArea - biteSize);
                    return biteSize;
                }
                float foodReturn = GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].bladeArea;
                ResetOrgan();
                GetPlant().ChangeBladeArea(0);
                return foodReturn;

            }
        }
        return 0;
    }

    public override void GrowOrgan(float growth) {
        GetPlant().ChangeBladeArea(GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].bladeArea + (growth * GetPlantSpeciesBlade().growthModifier));
        if (!spawned && GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].bladeArea > 0)
            Spawn();
    }

    internal override int GetFoodIndex() {
        return GetPlantSpeciesBlade().organFoodIndex;
    }

    public PlantSpeciesBlade GetPlantSpeciesBlade() {
        return (PlantSpeciesBlade)GetPlantSpeciesOrgan();
    }
}

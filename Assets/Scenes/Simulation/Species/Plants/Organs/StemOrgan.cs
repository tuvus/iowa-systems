using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StemOrgan : EddiblePlantOrgan {

    public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
        if (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].stemHeight > 0)
            Spawn();
    }

    public override void ResetOrgan() {
        base.ResetOrgan();
    }

    public override float EatPlantOrgan(Animal animal, float biteSize) {
        if (!spawned)
            return 0;
        for (int i = 0; i < animal.animalSpecies.eddibleFoodTypes.Length; i++) {
            if (animal.animalSpecies.eddibleFoodTypes[i] == GetPlantSpeciesStem().organFoodIndex) {
                if (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].stemHeight > biteSize) {
                    GetPlant().ChangeStemHeight(GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].stemHeight - biteSize);
                    return biteSize;
                }
                float foodReturn = GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].stemHeight;
                ResetOrgan();
                GetPlant().KillOrganism();
                return foodReturn;

            }
        }
        return 0;
    }

    public override void GrowOrgan(float growth) {
        GetPlant().ChangeStemHeight(GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].stemHeight + (growth * GetPlantSpeciesStem().growthModifier));
        if (!spawned && GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].stemHeight > 0) { 
            Spawn();
        }
    }

    internal override int GetFoodIndex() {
        return GetPlantSpeciesStem().organFoodIndex;
    }

    public PlantSpeciesStem GetPlantSpeciesStem() {
        return (PlantSpeciesStem)GetPlantSpeciesOrgan();
    }
}

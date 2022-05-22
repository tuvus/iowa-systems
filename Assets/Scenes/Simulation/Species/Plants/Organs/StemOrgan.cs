using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StemOrgan : EddiblePlantOrganScript {
    public PlantSpeciesStem speciesStem;

    internal override void SetUpSpecificOrgan() {
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
    }

    public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
        if (plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].stemHeight > 0)
            Spawn();
    }

    public override void ResetOrgan() {
        base.ResetOrgan();
    }

    public override float EatPlantOrgan(AnimalScript animal, float biteSize) {
        if (!spawned)
            return 0;
        for (int i = 0; i < animal.animalSpecies.eddibleFoodTypes.Length; i++) {
            if (animal.animalSpecies.eddibleFoodTypes[i] == speciesStem.organFoodIndex) {
                if (plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].stemHeight > biteSize) {
                    plantScript.ChangeStemHeight(plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].stemHeight - biteSize);
                    return biteSize;
                }
                float foodReturn = plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].stemHeight;
                ResetOrgan();
                plantScript.KillOrganism();
                return foodReturn;

            }
        }
        return 0;
    }

    public override void GrowOrgan(float growth) {
        plantScript.ChangeStemHeight(plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].stemHeight + (growth * speciesStem.growthModifier));
        if (!spawned && plantScript.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex].stemHeight > 0) { 
            Spawn();
        }
    }

    internal override int GetFoodIndex() {
        return speciesStem.organFoodIndex;
    }
}

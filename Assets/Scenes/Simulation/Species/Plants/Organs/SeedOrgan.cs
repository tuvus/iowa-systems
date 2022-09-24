using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class SeedOrgan : PlantOrgan {

	public float awnsGrowth;
	public float timeUntillDispersion;

	public override void SpawnOrganismAdult() {
		if (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].growthStage == PlantSpecies.GrowthStage.Adult) {
			if (Simulation.randomGenerator.NextUInt(0, 10) < 4) {
				awnsGrowth = Simulation.randomGenerator.NextFloat(0, GetPlantSpeciesSeeds().awnMaxGrowth);
			} else {
				awnsGrowth = GetPlantSpeciesSeeds().awnMaxGrowth;
				timeUntillDispersion = Simulation.randomGenerator.NextFloat(0, GetPlantSpeciesSeeds().awnSeedDispertionTime);
			}
		}
	}

	public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
		if (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].growthStage == PlantSpecies.GrowthStage.Seed)
			Spawn();
    }

    public override void ResetOrgan() {
		awnsGrowth = 0;
		timeUntillDispersion = 0;
	}

    public override void GrowOrgan(float growth) {
		if (timeUntillDispersion > 0) {
			timeUntillDispersion = math.max(0, timeUntillDispersion - GetPlant().GetEarthScript().simulationDeltaTime / 24);
			if (timeUntillDispersion <= 0) {
				awnsGrowth = 0;
				SpreadNewSeed(GetPlantSpeciesSeeds().awnMaxSeedAmount);
			}
			return;
		}
		awnsGrowth += growth / 100;
		if (awnsGrowth >= GetPlantSpeciesSeeds().awnMaxGrowth) {
			awnsGrowth = GetPlantSpeciesSeeds().awnMaxGrowth;
			timeUntillDispersion = GetPlantSpeciesSeeds().awnSeedDispertionTime;
		}
	}

	public void SpreadNewSeed(int seedCount) {
		for (int i = 0; i < seedCount; i++) {
			if (Simulation.randomGenerator.NextInt(0,100) < GetPlantSpeciesSeeds().awnSeedDispersalSuccessChance)
				GetPlantSpeciesSeeds().SpreadSeed(GetPlant());
		}
	}

	public override float GetGrowth(float deltaTime) {
		if (GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].growthStage == PlantSpecies.GrowthStage.Germinating || GetPlant().GetEarthScript().GetZoneController().allPlants[GetPlant().plantDataIndex].growthStage == PlantSpecies.GrowthStage.Sprout) {
			return deltaTime * GetPlantSpeciesSeeds().seedEnergyAmount;
		}
		return base.GetGrowth(deltaTime);
    }

    public override void OnOrganismGermination() {
		GetPlantSpeciesSeeds().GrowSeed();
		Despawn();
    }

	public override void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
		GetZoneController().AddFoodTypeToZone(zoneIndex, GetPlantSpeciesSeeds().organFoodIndex, dataLocation);
	}

	public override void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
		GetZoneController().RemoveFoodTypeFromZone(zoneIndex, GetPlantSpeciesSeeds().organFoodIndex, dataLocation);
	}

	public PlantSpeciesSeeds GetPlantSpeciesSeeds() {
		return (PlantSpeciesSeeds)GetPlantSpeciesOrgan();
    }
}

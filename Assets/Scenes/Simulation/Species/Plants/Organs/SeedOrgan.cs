using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class SeedOrgan : BasicPlantOrganScript {

	public PlantSpeciesSeeds speciesSeeds;

	public float awnsGrowth;
	public float timeUntillDispersion;


	internal override void SetUpSpecificOrgan() {
	}

	public override void SpawnOrganismAdult() {
		if (plantScript.stage == PlantScript.GrowthStage.Adult) {
			if (UnityEngine.Random.Range(0, 10) < 4) {
				awnsGrowth = UnityEngine.Random.Range(0, speciesSeeds.awnMaxGrowth);
			} else {
				awnsGrowth = speciesSeeds.awnMaxGrowth;
				timeUntillDispersion = UnityEngine.Random.Range(0, speciesSeeds.awnSeedDispertionTime);

			}
		}
	}

	public override void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
		if (plantScript.stage == PlantScript.GrowthStage.Seed)
			Spawn();
    }



    public override void ResetOrgan() {
		awnsGrowth = 0;
		timeUntillDispersion = 0;
	}

	public override void UpdateGrowthPriority() {
		growthPriority = 0f;
		switch (plantScript.stage) {
			case PlantScript.GrowthStage.Seed:
				growthPriority = 0;
				break;
			case PlantScript.GrowthStage.Adult:
				growthPriority = 1f;
				break;
		};
	}

    public override void GrowOrgan(float growth) {
		if (timeUntillDispersion > 0) {
			timeUntillDispersion -= plantScript.GetEarthScript().simulationDeltaTime;
			if (timeUntillDispersion <= 0) {
				timeUntillDispersion = 0;
				awnsGrowth = 0;
				SpreadNewSeed(speciesSeeds.awnMaxSeedAmount);
			}
			return;
		}
		awnsGrowth += growth / 100;
		if (awnsGrowth >= speciesSeeds.awnMaxGrowth) {
			awnsGrowth = speciesSeeds.awnMaxGrowth;
			timeUntillDispersion = speciesSeeds.awnSeedDispertionTime;
		}
	}

	public void SpreadNewSeed(int seedCount) {
		for (int i = 0; i < seedCount; i++) {
			if (UnityEngine.Random.Range(0,100) < speciesSeeds.awnSeedDispersalSuccessChance)
				speciesSeeds.SpreadSeed(plantScript);
		}
	}

	public override float GetGrowth(float deltaTime) {
		if (plantScript.stage == PlantScript.GrowthStage.Germinating || plantScript.stage == PlantScript.GrowthStage.Sprout) {
			return deltaTime * speciesSeeds.seedEnergyAmount;
		}
		return base.GetGrowth(deltaTime);
    }

    public override void OnOrganismGermination() {
		speciesSeeds.GrowSeed();
		Despawn();
    }

	public override void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
		GetZoneController().AddFoodTypeToZone(zoneIndex, speciesSeeds.organFoodIndex, dataLocation);
	}

	public override void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
		GetZoneController().RemoveFoodTypeFromZone(zoneIndex, speciesSeeds.organFoodIndex, dataLocation);
	}
}

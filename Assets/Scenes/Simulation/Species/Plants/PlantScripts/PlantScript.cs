using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class PlantScript : BasicOrganismScript {
	public enum GrowthStage {
		Dead = -2,
		Seed = -1,
		Germinating = 0,
		Sprout = 1,
		Seedling = 2,
		Youngling = 3,
		Adult = 4,
	}
	public PlantSpecies plantSpecies;
	PlantScript plantParent;

	public int plantDataIndex;
	public float growth;
	public float bladeArea;
	public float stemHeight;
	public float2 rootGrowth;
	public GrowthStage stage;

	public List<BasicPlantOrganScript> organs;
	public List<EddiblePlantOrganScript> eddibleOrgans;

	public struct PlantData {
		public float age;
		public int speciesIndex;
		public int specificSpeciesIndex;
		public int plantIndex;
		public int zone;
		public float3 position;
		public float bladeArea;
		public float stemHeight;
		public float2 rootGrowth;
		public float rootDensity;
		public GrowthStage stage;

		public PlantData(PlantScript plantScript) {
			age = plantScript.age;
			speciesIndex = plantScript.species.speciesIndex;
			specificSpeciesIndex = plantScript.species.specificSpeciesIndex;
			plantIndex = plantScript.specificOrganismIndex;
			zone = plantScript.zone;
			position = plantScript.position;
			bladeArea = plantScript.bladeArea;
			stemHeight = plantScript.stemHeight;
			rootGrowth = plantScript.rootGrowth;
			rootDensity = .1f;
			stage = plantScript.stage;
		}
	}

    #region setup
    public void SetUpPlantOrganism(PlantSpecies plantSpecies) {
		SetUpOrganism(plantSpecies);
		this.plantSpecies = plantSpecies;
		gameObject.name = plantSpecies + "Organism";
	}

	public void SpawnPlantRandom(GrowthStage stage) {
		this.stage = stage;
		age = 0;
        for (int i = 0; i < organs.Count; i++) {
			organs[i].SpawnOrganismAdult();
        }
    }

	public void SpawnSeedRandom(float age) {
		stage = GrowthStage.Seed;
		this.age = age;
    }

	public void SpawnSeed() {
		stage = GrowthStage.Seed;
		age = 0;
	}
    #endregion

    #region PlantUpdate
    public override void RefreshOrganism() {
	}

	public void UpdateOrganismBehavior(float sunGain, float waterGain, GrowthStage stage) {
		if (!spawned)
			return;
		if (this.stage == GrowthStage.Seed) {
			if (stage == GrowthStage.Dead) {
				KillOrganism();
				return;
			}
			if (stage == GrowthStage.Germinating) {
				SeedGerminated();
				return;
			}
			return;
		}
        this.stage = stage;
		if (this.stage != GrowthStage.Germinating)
			GetMeshRenderer().enabled = User.Instance.GetRenderWorldUserPref();
		growth = GetEarthScript().simulationDeltaTime * Mathf.Sqrt(sunGain * waterGain);
		UpdateGrowthPriority();
	}

	void UpdateGrowthPriority() {
        for (int i = 0; i < organs.Count; i++) {
			organs[i].UpdateGrowthPriority();
        }
    }

	public override void UpdateOrganism() {
		age += GetEarthScript().simulationDeltaTime;
		growth += GetGrowth();
		Grow(growth);
		bladeArea = GetBladeArea();
		stemHeight = GetStemHeight();
		rootGrowth = GetRootGrowth();
	}
	#endregion

	#region PlantControls
	public override void AddToZone(int zoneIndex) {
		for (int i = 0; i < organs.Count; i++) {
			organs[i].OnPlantAddToZone(zone, new ZoneController.DataLocation(this));
		}
    }

	public override void RemoveFromZone() {
		base.RemoveFromZone();
	}

	public void Grow(float growth) {
		float newGrowth = growth;
		for (int i = 0; i < organs.Count; i++) {
			float giveGrowth = newGrowth * organs[i].GetGrowthPriority();
			newGrowth -= giveGrowth;
			organs[i].GrowOrgan(giveGrowth);
			if (newGrowth <= 0)
				break;
        }
	}

	public float EatPlant(AnimalScript animal, float biteAmount) {
		float foodReturn = 0;
        for (int i = eddibleOrgans.Count - 1; i >= 0; i--) {
			//Multipling and dividing by 10 reduces the total food gained by eating an entire plant
			float newFood = eddibleOrgans[i].EatPlantOrgan(animal, biteAmount * 10) / 10;
			foodReturn += newFood;
			biteAmount -= newFood;
        }
		return foodReturn;
	}

	void SeedGerminated() {
		stage = GrowthStage.Germinating;
		for (int i = 0; i < organs.Count; i++) {
			organs[i].OnOrganismGermination();
		}
	}

	public override void KillOrganism() {
		if (stage == GrowthStage.Seed)
			plantSpecies.GetSpeciesSeeds().SeedDeath();
		else 
			species.OrganismDeath();
		OrganismDied();
	}

	internal override void OrganismDied() {
		RemoveFromZone();
		for (int i = 0; i < organs.Count; i++) {
			organs[i].OnOrganismDeath();
		}
		plantSpecies.DeactivatePlant(this, PlantSpecies.ListType.activePlants);
	}

	public void ResetPlant() {
		stage = GrowthStage.Dead;
		zone = -1;
		growth = 0;
		age = 0;
		bladeArea = 0;
		stemHeight = 0;
		rootGrowth = new float2(0, 0);
        for (int i = 0; i < organs.Count; i++) {
			organs[i].ResetOrgan();
        }
		GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(this);

	}
	#endregion

	#region GetMethods
	float GetGrowth() {
		float growth = 0;
        for (int i = 0; i < organs.Count; i++) {
			growth += organs[i].GetGrowth(GetEarthScript().simulationDeltaTime);
        }
		return growth;
    }

	float GetBladeArea() {
		float bladeArea = 0;
        for (int i = 0; i < organs.Count; i++) {
			bladeArea += organs[i].GetBladeArea();
        }
		return bladeArea;
    }

	float GetStemHeight() {
		float stemHeight = -2f;
        for (int i = 0; i < organs.Count; i++) {
			stemHeight += organs[i].GetStemheight();
        }
		return stemHeight;
    }

	float2 GetRootGrowth() {
		float2 rootGrowth = 0;
        for (int i = 0; i < organs.Count; i++) {
			rootGrowth += organs[i].GetRootGrowth();
        }
		return rootGrowth;
    }
    #endregion
}
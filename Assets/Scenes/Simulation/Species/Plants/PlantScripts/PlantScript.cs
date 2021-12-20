using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

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

	public List<BasicPlantOrganScript> organs = new List<BasicPlantOrganScript>();

	public struct PlantData {
		public float age;
		public int speciesIndex;
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
			speciesIndex = plantScript.species.specificSpeciesIndex;
			plantIndex = plantScript.specificOrganismIndex;
			zone = plantScript.zone;
			position = plantScript.position;
			bladeArea = plantScript.bladeArea;
			stemHeight = plantScript.stemHeight;
			rootGrowth = plantScript.rootGrowth;
			rootDensity = .1f;
			stage = plantScript.stage;

		}

		public void SetupData(PlantScript plantScript) {
			age = plantScript.age;
			speciesIndex = plantScript.species.specificSpeciesIndex;
			plantIndex = plantScript.specificOrganismIndex;
			zone = plantScript.zone;
			position = plantScript.position;
			bladeArea = plantScript.bladeArea;
			stemHeight = plantScript.stemHeight;
			rootGrowth = plantScript.rootGrowth;
			rootDensity = .1f;
			stage = plantScript.stage;
			plantScript.plantSpecies.GetEarthScript().GetZoneController().allPlants[plantScript.plantDataIndex] = this;
			//Debug.Log(plantIndex + " " + plantScript.plantIndex);
		}
	}

	public override void SetUpSpecificOrganism(BasicOrganismScript parent) {
		//position = GetOrganismMotor().GetModelTransform().position;
		//GetEddible().postion = position;
		zone = -1;
		if (parent != null) {
			plantParent = parent.GetComponent<PlantScript>();
			return;
		}
	}

	#region PlantUpdate
	public override void RefreshOrganism() {
	}

	public void UpdateOrganismBehavior(float sunGain, float waterGain, GrowthStage stage) {
		if (this.stage == GrowthStage.Seed && stage == GrowthStage.Germinating) {
			SeedGerminated();
		}
		growth = GetEarthScript().simulationDeltaTime * Mathf.Sqrt(sunGain * waterGain);
        this.stage = stage;
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
	public void AddToZone(int zoneIndex) {
		zone = zoneIndex;
        for (int i = 0; i < organs.Count; i++) {
			organs[i].AddToZone(zone,plantDataIndex);
        }
    }

	public void RemoveFromZone() {
        for (int i = 0; i < organs.Count; i++) {
			organs[i].RemoveFromZone(zone);
        }
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

	void SeedGerminated() {
		for (int i = 0; i < organs.Count; i++) {
			organs[i].OnOrganismGermination();
		}
	}

	internal override void OrganismDied() {
		RemoveFromZone();
		for (int i = 0; i < organs.Count; i++) {
			organs[i].OnOrganismDeath();
		}
		plantSpecies.DeactivatePlant(this);
	}

	public void ResetPlant() {
		stage = GrowthStage.Dead;
		zone = -1;
		growth = 0;
		bladeArea = 0;
		stemHeight = 0;
		rootGrowth = new float2(0, 0);
        for (int i = 0; i < organs.Count; i++) {
			organs[i].ResetOrgan();
        }
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
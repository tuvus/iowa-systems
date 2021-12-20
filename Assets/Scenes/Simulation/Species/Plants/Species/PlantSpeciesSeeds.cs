using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesSeeds : BasicPlantSpeciesOrganScript {
	//public GameObject seedPrefab;

	public string foodType;
	public int startingSeedCount;
	public float humidityRequirement;
	public float tempetureRequirement;
	public float seedDispertionRange;
	public float timeRequirement;
	public float timeMaximum;

	public float awnMaxGrowth;
	public int awnMaxSeedAmount;
	public float awnSeedDispertionTime;
	public float awnSeedDispersalSuccessChance;

	public float seedEnergyAmount;

	public int seedCount { private set; get; }

	public SeedGerminationRequirement seedGerminationRequirement;

    public struct SeedGerminationRequirement {
		public float humidityRequirement;
		public float tempetureRequirement;
		public float timeRequirement;
		public float timeMaximum;

        public SeedGerminationRequirement(PlantSpeciesSeeds speciesSeeds) {
			humidityRequirement = speciesSeeds.humidityRequirement;
			tempetureRequirement = speciesSeeds.tempetureRequirement;
			timeRequirement = speciesSeeds.timeRequirement;
			timeMaximum = speciesSeeds.timeMaximum;
        }
	}

	public void SetupPlantSpeciesSeeds() {
		seedGerminationRequirement = new SeedGerminationRequirement(this);
    }

	public override void MakeOrganism(BasicOrganismScript _newOrganism) {
		SeedOrgan seeds = _newOrganism.gameObject.AddComponent<SeedOrgan>();
		seeds.speciesSeeds = this;
		seeds.SetupBasicOrgan(this);
	}

	public void MakePlant (PlantScript plant, float growth) {
		
	}

	public void Populate () {
		plantSpecies.PreSpawn(startingSeedCount);
		for (int i = 0; i < startingSeedCount; i++) {
			PlantScript newseed = plantSpecies.SpawnRandomSeed();
			AddSeed();
		}
	}

	public void SpreadSeed(PlantScript parent) {
		PlantScript newSeed = plantSpecies.SpawnSeed(parent, seedDispertionRange);
		AddSeed();
	}

	public void AddSeed() {
		seedCount++;
	}

	public void SeedDeath() {
		seedCount--;
    }

	public void GrowSeed() {
		plantSpecies.SeedGrownToPlant();
		seedCount--;
	}	
}
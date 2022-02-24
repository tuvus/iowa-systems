using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class PlantSpecies : BasicSpeciesScript {
	public GameObject plantPrefab;
	PlantSpeciesSeeds plantSpeciesSeeds;

	public List<BasicPlantSpeciesOrganScript> speciesOrgansOrder;
	[SerializeField] List<Vector3> growthStagesInput = new List<Vector3>();

	public enum ListType {
		unlisted = -1,
		inactivePlants = 0,
		activePlants = 1,
	}

	[SerializeField] List<PlantScript> plants;
	[SerializeField] List<int> activePlants;
	[SerializeField] List<int> inactivePlants;
	[SerializeField] List<ChangePlantList> changePlantList;

    public struct ChangePlantList {
		public ListType toList;
		public ListType fromList;
		public int plantIndex;

        public ChangePlantList(ListType toList, ListType fromList, int plantIndex) {
			this.toList = toList;
			this.fromList = fromList;
			this.plantIndex = plantIndex;
        }
    }

	PlantJobController plantJobController;
	public NativeArray<GrowthStageData> growthStages;

	public struct GrowthStageData {
		public PlantScript.GrowthStage stage;
		public float bladeArea;
		public float stemHeight;
		public float rootDepth;

		public GrowthStageData(PlantScript.GrowthStage stage, float bladeArea, float stemHeight, float rootDepth) {
			this.stage = stage;
			this.bladeArea = bladeArea;
			this.stemHeight = stemHeight;
			this.rootDepth = rootDepth;
		}
	}

	#region StartSimulation
	internal override void SetupSpecificSimulation() {
		plantJobController = gameObject.AddComponent<PlantJobController>();
		plantJobController.SetUpJobController(this,earth);
		plantSpeciesSeeds = GetComponent<PlantSpeciesSeeds>();
		plantSpeciesSeeds.SetupPlantSpeciesSeeds();
		growthStages = new NativeArray<GrowthStageData>(growthStagesInput.Count, Allocator.Persistent);
        for (int i = 0; i < growthStagesInput.Count; i++) {
			growthStages[i] = new GrowthStageData((PlantScript.GrowthStage)i,growthStagesInput[i].x,growthStagesInput[i].y,growthStagesInput[i].z);
        }
    }

    public override void SetupSpeciesFoodType() {
        for (int i = 0; i < speciesOrgansOrder.Count; i++) {
			speciesOrgansOrder[i].SetupSpeciesOrganFoodType();
        }
    }

    internal override void StartSimulation() {
		Populate();
		UpdateOrganismData();
	}
	#endregion

	#region SpawnOrganisms
	public override void PreSpawn(int spawnNumber) {
        for (int i = 0; i < spawnNumber; i++) {
			PlantScript plant = SpawnOrganism(plantPrefab).GetComponent<PlantScript>();
			plant.organs = new List<BasicPlantOrganScript>(speciesOrgansOrder.Count);
			plant.eddibleOrgans = new List<EddiblePlantOrganScript>(speciesOrgansOrder.Count);
			AddOrganism(plant);
			earth.GetZoneController().AddPlant(plant);
			for (int f = 0; f < speciesOrgansOrder.Count; f++) {
				speciesOrgansOrder[f].MakeOrganism(plant);
			}
			plant.SetUpPlantOrganism(this);
			DeactivatePlant(plant, ListType.unlisted, true);
		}
	}

	#region OnSimulationStart
	public override void Populate() {
		int organismsToSpawn = startingPopulation;
		plants = new List<PlantScript>(startingPopulation * 2);
		activePlants = new List<int>(startingPopulation * 2);
		inactivePlants = new List<int>(startingPopulation * 2);
		changePlantList = new List<ChangePlantList>(startingPopulation * 2);
		PreSpawn(organismsToSpawn * 2);
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnRandomOrganism();
        }
        if (plantSpeciesSeeds != null) {
            plantSpeciesSeeds.Populate();
        }
		UpdateOrganismLists();
		earth.StartFindZoneJobs();
		earth.CompleteFindZoneJobs();

	}

	public override void SpawnRandomOrganism() {
		PlantScript plantScript = GetInactivePlant();
		ActivatePlant(plantScript, ListType.inactivePlants, true);
		populationCount++;
		RandomiseOrganismPosition(plantScript);
		AddToFindZone(plantScript);
		plantScript.SpawnPlantRandom(growthStages[Random.Range(0, growthStagesInput.Count)].stage);
	}

	public PlantScript SpawnRandomSeed() {
		PlantScript plantScript = GetInactivePlant();
		ActivatePlant(plantScript, ListType.inactivePlants, true);
		RandomiseOrganismPosition(plantScript);
		AddToFindZone(plantScript);
		return plantScript;
	}
	#endregion

	public PlantScript SpawnSeed(PlantScript parent, float range) {
		PlantScript plantScript = GetInactivePlant();
		RemovePlantFromList(ListType.inactivePlants, plantScript.specificOrganismIndex);
		ActivatePlant(plantScript, ListType.unlisted);
		RandomiseOrganismChildPosition(plantScript, parent, range);
		AddToFindZone(plantScript, parent.zone, range);
		return plantScript;
	}

	public PlantScript SpawnPlant() {
		PlantScript plantScript = GetInactivePlant();
		RemovePlantFromList(ListType.inactivePlants, plantScript.specificOrganismIndex);
		ActivatePlant(plantScript, ListType.unlisted);
		RandomiseOrganismPosition(plantScript);
		SetUpOrgans(plantScript);
		plantScript.stage = PlantScript.GrowthStage.Germinating;
		populationCount++;
		return plantScript;
	}

	PlantScript GetInactivePlant() {
		if (inactivePlants.Count == 0) {
			PreSpawn(1);
		}
		return plants[inactivePlants[0]];
	}

	void SetUpOrgans(PlantScript plantScript) {
		for (int i = 0; i < speciesOrgansOrder.Count; i++) {
			speciesOrgansOrder[i].MakeOrganism(plantScript);
		}
	}
    #endregion

    #region PlantControlls
    public override void UpdateOrganismData() {
        for (int i = 0; i < activePlants.Count; i++) {
            earth.GetZoneController().allPlants[plants[activePlants[i]].plantDataIndex] = new PlantScript.PlantData(plants[activePlants[i]]);
        }
    }

    public override void UpdateOrganismsBehavior() {
        for (int i = 0; i < activePlants.Count; i++) {
			plants[activePlants[i]].UpdateOrganismBehavior(GetPlantJobController().plantReasourceGain[i].x,GetPlantJobController().plantReasourceGain[i].y,GetPlantJobController().plantGrowthStage[i]);
        }
    }

    public override void UpdateOrganisms() {
        for (int i = 0; i < activePlants.Count; i++) {
			plants[activePlants[i]].UpdateOrganism();
        }
    }

	public void SeedGrownToPlant() {
		populationCount++;
    }

	public void AddToFindZone(PlantScript plant, int zone = -1, float range = 0) {
		earth.GetZoneController().FindZoneController.AddFindZoneData(new FindZoneController.FindZoneData(new ZoneController.DataLocation(plant), zone, plant.position, range));
	}

	public override void OnSettingsChanged(bool renderOrganisms) {
		for (int i = 0; i < activePlants.Count; i++) {
			plants[activePlants[i]].CheckRendering();
		}
	}
	#endregion

	#region PlantListControls
	internal override void AddSpecificOrganism(BasicOrganismScript newOrganism) {
		PlantScript newPlant = (PlantScript)newOrganism;
		plants.Add(newPlant);
		newPlant.specificOrganismIndex = plants.Count - 1;
	}

	public void ActivatePlant(PlantScript plant, ListType fromList, bool imediatly = false) {
		if (imediatly) {
			AddAndRemovePlantToList(new ChangePlantList(ListType.activePlants, fromList, plant.specificOrganismIndex));
		} else {
			changePlantList.Add(new ChangePlantList(ListType.activePlants, fromList, plant.specificOrganismIndex));
		}
		plant.spawned = true;
	}

	public void DeactivatePlant(PlantScript plant, ListType fromList, bool imediatly = false) {
		plant.ResetPlant();
		if (imediatly) {
			AddAndRemovePlantToList(new ChangePlantList(ListType.inactivePlants, fromList, plant.specificOrganismIndex));
		} else {
			changePlantList.Add(new ChangePlantList(ListType.inactivePlants, fromList, plant.specificOrganismIndex));
		}
		plant.spawned = false;
		plant.GetMeshRenderer().enabled = false;
	}

	public override void UpdateOrganismLists() {
		while (changePlantList.Count > 0) {
			AddAndRemovePlantToList(changePlantList[0]);
			changePlantList.RemoveAt(0);
		}
	}

	void AddAndRemovePlantToList(ChangePlantList changePlant) {
		RemovePlantFromList(changePlant.fromList, changePlant.plantIndex);
		AddPlantToList(changePlant.toList, changePlant.plantIndex);
	}

	void RemovePlantFromList(ListType fromList, int plantIndex) {
		switch (fromList) {
			case ListType.activePlants:
				activePlants.Remove(plantIndex);
				if (activePlants.Contains(plantIndex))
					print(plantIndex);
				break;
			case ListType.inactivePlants:
				inactivePlants.Remove(plantIndex);
				break;
		}
	}
	
	void AddPlantToList(ListType toList, int plantIndex) {
		switch (toList) {
			case ListType.activePlants:
				activePlants.Add(plantIndex);
				break;
			case ListType.inactivePlants:
				inactivePlants.Add(plantIndex);
				break;
		}
	}
	#endregion

	#region GetMethods

	public PlantScript GetPlant(int plantIndex) {
		return plants[plantIndex];
	}

	public int GetActivePlantCount() {
		return activePlants.Count;
	}

	public int GetActivePlant(int activePlantIndex) {
		return activePlants[activePlantIndex];
	}

	public PlantJobController GetPlantJobController() {
		return plantJobController;
	}

	public PlantSpeciesSeeds GetSpeciesSeeds() {
		return plantSpeciesSeeds;
    }

    public override List<string> GetOrganismFoodTypes() {
		List<string> types = new List<string>();
        for (int i = 0; i < speciesOrgansOrder.Count; i++) {
			if (speciesOrgansOrder[i].GetOrganType() != null) {
				types.Add(speciesOrgansOrder[i].GetOrganType());
			}
        }
		return types;
    }
    #endregion

    public void OnDestroy() {
		if (growthStages.IsCreated)
			growthStages.Dispose();
    }
}

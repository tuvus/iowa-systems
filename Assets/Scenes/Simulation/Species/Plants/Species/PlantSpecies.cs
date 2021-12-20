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

	[SerializeField] private List<PlantScript> plants = new List<PlantScript>();
	[SerializeField] private List<int> activePlants = new List<int>();
	[SerializeField] private List<int> inactivePlants = new List<int>();


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

    internal override void StartSimulation() {
		Populate();
		UpdateOrganismData();
        for (int i = 0; i < plantJobController.plantFindZoneCount; i++) {
			plants[earth.GetZoneController().allPlants[plantJobController.plantZones[i].x].plantIndex].AddToZone(plantJobController.plantZones[i].y);
        }
	}
	#endregion

	#region SpawnOrganisms
	public void PreSpawn(int spawnNumber) {
        for (int i = 0; i < spawnNumber; i++) {
			PlantScript plantScript = SpawnOrganism(plantPrefab).GetComponent<PlantScript>();
			SetupOrganismMotor(plantScript);
			plantScript.plantSpecies = this;
			plantScript.SetUpOrganism(this, null);
			AddOrganism(plantScript);
			earth.GetZoneController().AddPlant(plantScript);
			DeactivatePlant(plantScript);
			for (int f = 0; f < speciesOrgansOrder.Count; f++) {
				speciesOrgansOrder[f].MakeOrganism(plantScript);
			}
		}
	}

	#region OnSimulationStart
	public override void Populate() {
		int organismsToSpawn = startingPopulation;
		PreSpawn(organismsToSpawn * 2);
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnRandomOrganism();
		}
		if (plantSpeciesSeeds != null) {
			plantSpeciesSeeds.Populate();
		}
	}

	PlantScript GetInactivePlant() {
		if (inactivePlants.Count > 0) {
			PreSpawn(1);
		}
		return plants[inactivePlants[0]];
    }

    public override void SpawnRandomOrganism() {
		PlantScript plantScript = GetInactivePlant();
		ActivatePlant(plantScript);
		populationCount++;
		RandomiseOrganismPosition(plantScript);
		AddToFindZone(plantScript);
		plantScript.stage = PlantScript.GrowthStage.Germinating;
	}

	public PlantScript SpawnRandomSeed() {
		PlantScript plantScript = GetInactivePlant();
		ActivatePlant(plantScript);
		RandomiseOrganismPosition(plantScript);
		AddToFindZone(plantScript);
		plantScript.stage = PlantScript.GrowthStage.Seed;
		return plantScript;
	}
    #endregion

    public PlantScript SpawnSeed(PlantScript parent, float range) {
		PlantScript plantScript = GetInactivePlant();
		ActivatePlant(plantScript);
		RandomiseOrganismChildPosition(plantScript, parent, range);
		AddToFindZone(plantScript, range);
		plantScript.stage = PlantScript.GrowthStage.Seed;
		return plantScript;
	}

	public override BasicOrganismScript SpawnOrganism(BasicOrganismScript _parent) {
		PlantScript plantScript = GetInactivePlant();
		plantScript.plantSpecies = this;
		plantScript.SetUpOrganism(this, _parent);
		SetUpOrgans(plantScript);
		plantScript.stage = PlantScript.GrowthStage.Germinating;
		populationCount++;
		return plantScript;
	}

	void SetUpOrgans(PlantScript plantScript) {
		for (int i = 0; i < speciesOrgansOrder.Count; i++) {
			speciesOrgansOrder[i].MakeOrganism(plantScript);
		}
	}
    #endregion

    #region PlantControlls
    public override void UpdateOrganismData() {
        for (int i = 0; i < plants.Count; i++) {
            earth.GetZoneController().allPlants[plants[i].plantDataIndex].SetupData(plants[i]);
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
    #endregion

    #region PlantListControls
    internal override void AddSpecificOrganism(BasicOrganismScript newOrganism) {
		PlantScript newPlant = newOrganism.GetComponent<PlantScript>();
		if (newPlant != null) {
			plants.Add(newPlant);
			newPlant.specificOrganismIndex = plants.Count - 1;
        }
	}

	public void AddToFindZone(PlantScript plant) {
		AddToFindZone(plant, 0);
	}

	public void AddToFindZone(PlantScript plant,float range) {
		earth.GetZoneController().FindZoneController.AddFindZoneData(new FindZoneController.FindZoneData(plant.plantSpecies.speciesIndex, plant.organismIndex, plant.position, range));
    }

	public void ActivatePlant(PlantScript plant) {
		activePlants.Add(plant.specificOrganismIndex);
		inactivePlants.Remove(plant.specificOrganismIndex);
	}

	public void DeactivatePlant(PlantScript plant) {
		plant.ResetPlant();
		activePlants.Remove(plant.specificOrganismIndex);
		inactivePlants.Add(plant.specificOrganismIndex);
	}

    #endregion

    #region GetMethods
	public List<PlantScript> GetPlants() {
		return plants;
    }

	public List<int> GetActivePlants() {
		return activePlants;
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

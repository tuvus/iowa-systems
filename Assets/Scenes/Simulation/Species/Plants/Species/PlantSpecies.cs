using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class PlantSpecies : Species {
    public GameObject plantPrefab;
    PlantSpeciesSeeds plantSpeciesSeeds;

    public List<PlantSpeciesOrgan> speciesOrgansOrder;
    [Tooltip("x=bladeArea, y=stemHeight, z=rootDepth, int w=daysAfterGermination. " +
        "The first element in this is the starting growth at seed. " +
        "Each element in between shows how much growth is needed for the next stage." +
        "The last element is the final growth wanted as an adult.")]
    [SerializeField] List<GrowthStageData> growthStagesInput = new List<GrowthStageData>();

    public enum ListType {
        unlisted = -1,
        inactivePlants = 0,
        activePlants = 1,
    }

    [SerializeField] List<Plant> plants;
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

    [System.Serializable]
    public struct GrowthStageData {
        public Plant.GrowthStage stage;
        public float bladeArea;
        public float stemHeight;
        public float2 rootGrowth;
        public int daysAfterGermination;

        public GrowthStageData(Plant.GrowthStage stage, float bladeArea, float stemHeight, float2 rootGrowth, int daysAfterGermination) {
            this.stage = stage;
            this.bladeArea = bladeArea;
            this.stemHeight = stemHeight;
            this.rootGrowth = rootGrowth;
            this.daysAfterGermination = daysAfterGermination;
        }
    }

    #region StartSimulation
    public override void SetupSimulation(Earth earth) {
        plantJobController = gameObject.AddComponent<PlantJobController>();
        plantJobController.SetUpJobController(this);
        base.SetupSimulation(earth);
        plantSpeciesSeeds = GetComponent<PlantSpeciesSeeds>();
        plantSpeciesSeeds.SetupPlantSpeciesSeeds();
        growthStages = new NativeArray<GrowthStageData>(growthStagesInput.Count, Allocator.Persistent);
        for (int i = 0; i < growthStagesInput.Count; i++) {
            growthStages[i] = growthStagesInput[i];
        }
        for (int i = 0; i < speciesOrgansOrder.Count; i++) {
            speciesOrgansOrder[i].growthPriorities = new NativeArray<float>(growthStages.Length, Allocator.Persistent);
        }
        for (int i = 0; i < growthStages.Length; i++) {
            float totalGrowthRequired = 0;
            if (i == growthStages.Length - 1) {
                //This is the adult growth stage, most organs should not want any growth at this point
                for (int f = 0; f < speciesOrgansOrder.Count; f++) {
                    totalGrowthRequired += speciesOrgansOrder[f].GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i], growthStages[i]);
                }
                for (int j = 0; j < speciesOrgansOrder.Count; j++) {
                    speciesOrgansOrder[j].growthPriorities[i] = speciesOrgansOrder[j].GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i], growthStages[i]) / totalGrowthRequired;
                }
            } else {
                for (int f = 0; f < speciesOrgansOrder.Count; f++) {
                    totalGrowthRequired += speciesOrgansOrder[f].GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i + 1], growthStages[i]);
                }
                for (int j = 0; j < speciesOrgansOrder.Count; j++) {
                    speciesOrgansOrder[j].growthPriorities[i] = speciesOrgansOrder[j].GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i + 1], growthStages[i]) / totalGrowthRequired;
                }
            }
        }
    }

    public override void SetupSpeciesFoodType() {
        for (int i = 0; i < speciesOrgansOrder.Count; i++) {
            speciesOrgansOrder[i].SetupSpeciesOrganFoodType();
        }
    }

    public override void StartSimulation() {
        Populate();
        UpdateOrganismData();
    }
    #endregion

    #region SpawnOrganisms
    public override void PreSpawn(int spawnNumber) {
        for (int i = 0; i < spawnNumber; i++) {
            Plant plant = SpawnOrganism(plantPrefab).GetComponent<Plant>();
            plant.organs = new List<PlantOrgan>(speciesOrgansOrder.Count);
            plant.eddibleOrgans = new List<EddiblePlantOrgan>(speciesOrgansOrder.Count);
            AddOrganism(plant);
            GetEarth().GetZoneController().AddPlant(plant);
            for (int f = 0; f < speciesOrgansOrder.Count; f++) {
                speciesOrgansOrder[f].MakeOrganism(plant);
            }
            plant.SetupOrganism(this);
            DeactivatePlant(plant, ListType.unlisted, true);
        }
    }

    #region OnSimulationStart
    public override void Populate() {
        int organismsToSpawn = startingPopulation;
        plants = new List<Plant>(startingPopulation * 2);
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
        GetEarth().StartFindZoneJobs();
        GetEarth().CompleteFindZoneJobs();

    }

    public override void SpawnRandomOrganism() {
        Plant plant = GetInactivePlant();
        ActivatePlant(plant, ListType.inactivePlants, true);
        populationCount++;
        RandomiseOrganismPosition(plant);
        AddToFindZone(plant);
        plant.SpawnPlantRandom(growthStages[UnityEngine.Random.Range(1, growthStagesInput.Count)].stage);
    }

    public Plant SpawnRandomSeed() {
        Plant plant = GetInactivePlant();
        ActivatePlant(plant, ListType.inactivePlants, true);
        RandomiseOrganismPosition(plant);
        AddToFindZone(plant);
        return plant;
    }
    #endregion

    public Plant SpawnSeed(Plant parent, float range) {
        Plant plant = GetInactivePlant();
        RemovePlantFromList(ListType.inactivePlants, plant.specificOrganismIndex);
        ActivatePlant(plant, ListType.unlisted);
        RandomiseOrganismChildPosition(plant, parent, range);
        AddToFindZone(plant, parent.zone, range);
        return plant;
    }

    public Plant SpawnPlant() {
        Plant plant = GetInactivePlant();
        RemovePlantFromList(ListType.inactivePlants, plant.specificOrganismIndex);
        ActivatePlant(plant, ListType.unlisted);
        RandomiseOrganismPosition(plant);
        SetUpOrgans(plant);
        plant.SpawnPlantRandom(growthStages[UnityEngine.Random.Range(1, growthStagesInput.Count)].stage);
        populationCount++;
        return plant;
    }

    Plant GetInactivePlant() {
        if (inactivePlants.Count == 0) {
            PreSpawn(1);
        }
        return plants[inactivePlants[0]];
    }

    void SetUpOrgans(Plant plant) {
        for (int i = 0; i < speciesOrgansOrder.Count; i++) {
            speciesOrgansOrder[i].MakeOrganism(plant);
        }
    }
    #endregion

    #region PlantControlls
    public override void UpdateOrganismData() {
    }

    public override void UpdateOrganismsBehavior() {
        for (int i = 0; i < activePlants.Count; i++) {
            plants[activePlants[i]].UpdateOrganismBehavior(GetPlantJobController().plantReasourceGain[i].x, GetPlantJobController().plantReasourceGain[i].y, GetPlantJobController().plantGrowthStage[i]);
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

    public GrowthStageData GetGrowthStageData(Plant.GrowthStage stage) {
        return growthStages[(int)stage];
    }

    public void AddToFindZone(Plant plant, int zone = -1, float range = 0) {
        GetEarth().GetZoneController().FindZoneController.AddFindZoneData(new FindZoneController.FindZoneData(new ZoneController.DataLocation(plant), zone, plant.position, range));
    }

    public override void OnSettingsChanged(bool renderOrganisms) {
        for (int i = 0; i < activePlants.Count; i++) {
            plants[activePlants[i]].CheckRendering();
        }
    }
    #endregion

    #region PlantListControls
    public void AddOrganism(Plant newOrganism) {
        base.AddOrganism(newOrganism);
        Plant newPlant = newOrganism;
        plants.Add(newPlant);
        newPlant.specificOrganismIndex = plants.Count - 1;
    }

    public void ActivatePlant(Plant plant, ListType fromList, bool imediatly = false) {
        if (imediatly) {
            AddAndRemovePlantToList(new ChangePlantList(ListType.activePlants, fromList, plant.specificOrganismIndex));
        } else {
            changePlantList.Add(new ChangePlantList(ListType.activePlants, fromList, plant.specificOrganismIndex));
        }
        plant.spawned = true;
    }

    public void DeactivatePlant(Plant plant, ListType fromList, bool imediatly = false) {
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
                    Debug.LogError("Error " + plantIndex + " was not removed from " + fromList);
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

    public Plant GetPlant(int plantIndex) {
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

    /// <summary>
    /// Called after a simulation has ended but also after the intro scene is unloaded.
    /// </summary>
    public void OnDestroy() {
        if (plantJobController != null)
            plantJobController.job.Complete();
        if (growthStages.IsCreated)
            growthStages.Dispose();
    }
}

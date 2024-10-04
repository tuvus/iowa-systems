using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System;

public class PlantSpecies : Species {
    public GameObject plantPrefab;
    PlantSpeciesAwns plantSpeciesAwns;

    public enum GrowthStage {
        Dead = -1,
        Seed = 0,
        Germinating = 1,
        Sprout = 2,
        Seedling = 3,
        Youngling = 4,
        Adult = 5,
    }

    [Tooltip("x=bladeArea, y=stemHeight, z=rootDepth, int w=daysAfterGermination. " +
        "The first element in this is the starting growth at seed. " +
        "Each element in between shows how much growth is needed for the next stage." +
        "The last element is the final growth wanted as an adult.")]
    [SerializeField] public List<GrowthStageData> growthStagesInput = new List<GrowthStageData>();

    [System.Serializable]
    public struct GrowthStageData {
        public GrowthStage stage;
        public float bladeArea;
        public float stemHeight;
        public float2 rootGrowth;
        public int daysAfterGermination;

        public GrowthStageData(GrowthStage stage, float bladeArea, float stemHeight, float2 rootGrowth, int daysAfterGermination) {
            this.stage = stage;
            this.bladeArea = bladeArea;
            this.stemHeight = stemHeight;
            this.rootGrowth = rootGrowth;
            this.daysAfterGermination = daysAfterGermination;
        }
    }

    public GrowthStageData[] growthStages;
    public Dictionary<Organism, Plant> plants;

    public struct Plant {
        public GrowthStage stage;
        public float bladeArea;
        public float stemHeight;
        public float2 rootGrowth;
        public float rootDensity;

        public Plant(GrowthStage stage, float bladeArea, float stemHeight, float2 rootGrowth) {
            this.stage = stage;
            this.bladeArea = bladeArea;
            this.stemHeight = stemHeight;
            this.rootGrowth = rootGrowth;
            this.rootDensity = 1;
        }

        public Plant(GrowthStage stage, GrowthStageData growthStageData) {
            this.stage = stage;
            this.bladeArea = growthStageData.bladeArea;
            this.stemHeight = growthStageData.stemHeight;
            this.rootGrowth = growthStageData.rootGrowth;
            this.rootDensity = 1;
        }
    }

    #region StartSimulation
    public override void SetupSimulation(Earth earth) {
        base.SetupSimulation(earth);
        plants = new Dictionary<Organism, Plant>();
        plantSpeciesAwns = GetComponent<PlantSpeciesAwns>();
        growthStages = new GrowthStageData[growthStagesInput.Count];

        for (int i = 0; i < growthStagesInput.Count; i++) {
            growthStages[i] = growthStagesInput[i];
        }
        for (int i = 0; i < organs.Count; i++) {
            ((PlantSpeciesOrgan)organs[i]).growthPriorities = new float[growthStages.Length];
        }
        for (int i = 0; i < growthStages.Length; i++) {
            float totalGrowthRequired = 0;
            if (i == growthStages.Length - 1) {
                //This is the adult growth stage, most organs should not want any growth at this point
                for (int f = 0; f < organs.Count; f++) {
                    totalGrowthRequired += ((PlantSpeciesOrgan)organs[f]).GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i], growthStages[i]);
                }
                for (int j = 0; j < organs.Count; j++) {
                    ((PlantSpeciesOrgan)organs[j]).growthPriorities[i] = ((PlantSpeciesOrgan)organs[j]).GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i], growthStages[i]) / totalGrowthRequired;
                }
            } else {
                for (int f = 0; f < organs.Count; f++) {
                    totalGrowthRequired += ((PlantSpeciesOrgan)organs[f]).GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i + 1], growthStages[i]);
                }
                for (int j = 0; j < organs.Count; j++) {
                    ((PlantSpeciesOrgan)organs[j]).growthPriorities[i] = ((PlantSpeciesOrgan)organs[j]).GetGrowthRequirementForStage(growthStages[i].stage, growthStages[i + 1], growthStages[i]) / totalGrowthRequired;
                }
            }
        }
    }

    public override List<string> GetOrganismFoodTypes() {
        List<string> types = new List<string>();
        for (int i = 0; i < organs.Count; i++) {
            if (((PlantSpeciesOrgan)organs[i]).GetOrganType() != null) {
                types.Add(((PlantSpeciesOrgan)organs[i]).GetOrganType());
            }
        }
        return types;
    }

    public override void SetupSpeciesFoodType() {
    }

    public override void StartSimulation() {
        Populate();
    }

    public override void Populate() {
        for (int i = 0; i < startingPopulation; i++) {
            SpawnOrganism();
        }
        if (plantSpeciesAwns != null) {
            plantSpeciesAwns.Populate();
        }
    }
    #endregion

    public override Organism SpawnOrganism() {
        Organism organism = base.SpawnOrganism();
        GrowthStage stage = (GrowthStage)Simulation.randomGenerator.NextInt(1, 6);
        organism.age = GetGrowthStageData(stage).daysAfterGermination;
        Plant plant = new Plant(stage, GetGrowthStageData(stage));
        plantSpeciesAwns.SpawnAwns(organism, plant);
        return organism;
    }

    public override Organism SpawnOrganism(float3 position, int zone, float distance) {
        Organism organism = base.SpawnOrganism(position, zone, distance);
        Plant plant = new Plant(GrowthStage.Germinating, 10, 10, 10);
        plantSpeciesAwns.SpawnAwns(organism, plant);
        return organism;
    }

    public GrowthStageData GetGrowthStageData(GrowthStage stage) {
        return growthStages[(int)stage];
    }

    protected override void UpdateOrganism(Organism organism) {
        base.UpdateOrganism(organism);
        if (organism.age > 100) {
            // organismActions.Enqueue(new OrganismAction(OrganismAction.Action.Die, organism));
            return;
        }
        float bladeArea = plants[organism].bladeArea;
        float stemHeight = plants[organism].stemHeight;
        float2 rootGrowth = plants[organism].rootGrowth;
        int stageIndex = (int)plants[organism].stage;
        GrowthStage stage = plants[organism].stage;
        if (stageIndex != growthStages.Length - 1 && bladeArea >= growthStages[stageIndex].bladeArea
            && stemHeight >= growthStages[stageIndex].stemHeight && rootGrowth.y >= growthStages[stageIndex].rootGrowth.y) {
            stage = growthStages[stageIndex + 1].stage;
        }
        float sunValue = 0.5f;
        if (Simulation.Instance.sunRotationEffect) {
            float objectDistanceFromSun = Vector3.Distance(organism.position, GetEarth().GetSunPosition());
            float sunDistanceFromEarth = Vector3.Distance(new float3(0, 0, 0), GetEarth().GetSunPosition());
            sunValue = Mathf.Max((objectDistanceFromSun - sunDistanceFromEarth) / GetEarth().GetRadius() * 2, 0);
        }
        float sunGain = bladeArea * sunValue;
        float rootArea = (math.PI * rootGrowth.x * rootGrowth.y) + (math.pow(rootGrowth.x / 2, 2) * 2);
        float rootUnderWaterPercent = 1;
        //float rootUnderWaterPercent = 1 - (GetEarth().GetZoneController().zones[organisms[organism].zone].waterDepth / rootGrowth.y);
        float waterGain = rootArea * rootUnderWaterPercent * 1;
        for (int i = 0; i < organs.Count; i++) {
            ((PlantSpeciesOrgan)organs[i]).GrowOrgan(organism, GetEarth().simulationDeltaTime * Mathf.Sqrt(sunGain * waterGain), ref bladeArea, ref stemHeight, ref rootGrowth);
        }
        plants[organism] = new Plant(stage, bladeArea, stemHeight, rootGrowth);
    }

    public override void OnSettingsChanged(bool renderOrganisms) {
        //for (int i = 0; i < activePlants.Count; i++) {
        //    plants[activePlants[i]].CheckRendering();
        //}
        throw new NotImplementedException();
    }

    public PlantSpeciesAwns GetSpeciesSeeds() {
        return plantSpeciesAwns;
    }
}
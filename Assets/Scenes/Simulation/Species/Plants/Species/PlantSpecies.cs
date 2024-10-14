using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Linq;
using System.Threading;

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

    public class Plant : ICloneable {
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

        public Plant(Plant plant){
            this.stage = plant.stage;
            this.bladeArea = plant.bladeArea;
            this.stemHeight = plant.stemHeight;
            this.rootGrowth = plant.rootGrowth;
            this.rootDensity = plant.rootDensity;
        }

        public object Clone() {
            return MemberwiseClone();
        }
    }

    #region StartSimulation
    public override void SetupSimulation(Earth earth) {
        base.SetupSimulation(earth);
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
        if (stage != GrowthStage.Adult) {
            organism.age = Simulation.randomGenerator.NextFloat(GetGrowthStageData(stage).daysAfterGermination, GetGrowthStageData(stage + 1).daysAfterGermination);
        } else {
            organism.age = GetGrowthStageData(stage).daysAfterGermination + Simulation.randomGenerator.NextFloat(0, 30);
        }
        organism.AddOrgan(new Plant(stage, GetGrowthStageData(stage)));
        organs.ForEach(o => o.SpawnOrgan(organism));
        return organism;
    }

    public override Organism SpawnOrganism(float3 position, int zone, float distance) {
        Organism organism = base.SpawnOrganism(position, zone, distance);
        organism.AddOrgan(new Plant(GrowthStage.Germinating, 10, 10, 10));
        organs.ForEach(o => o.SpawnOrgan(organism));
        return organism;
    }

    public GrowthStageData GetGrowthStageData(GrowthStage stage) {
        return growthStages[(int)stage];
    }

    public override void StartJobs(HashSet<Thread> activeThreads, bool threaded) {
        base.StartJobs(activeThreads, threaded);
        if (GetPlantSpeciesSeeds() != null) {
            GetPlantSpeciesSeeds().StartJobs(activeThreads, threaded);
        }
    }

    protected override void UpdateOrganism(Organism organismR) {
        base.UpdateOrganism(organismR);
        if (organismR.age > 10000) {
            KillOrganism(organismR);
            return;
        }

        Plant plantR = organismR.GetReadable().GetOrgan<Plant>();
        int stageIndex = (int)plantR.stage;
        if (stageIndex != growthStages.Length - 1 && plantR.bladeArea >= growthStages[stageIndex].bladeArea
            && plantR.stemHeight >= growthStages[stageIndex].stemHeight && plantR.rootGrowth.y >= growthStages[stageIndex].rootGrowth.y) {
            plantR.stage = growthStages[stageIndex + 1].stage;
        }
        float sunValue = 0.5f;
        if (Simulation.Instance.sunRotationEffect) {
            float objectDistanceFromSun = Vector3.Distance(organismR.position, GetEarth().GetSunPosition());
            float sunDistanceFromEarth = Vector3.Distance(new float3(0, 0, 0), GetEarth().GetSunPosition());
            sunValue = Mathf.Max((objectDistanceFromSun - sunDistanceFromEarth) / GetEarth().GetRadius() * 2, 0);
        }
        float sunGain = plantR.bladeArea * sunValue;
        float rootArea = (math.PI * plantR.rootGrowth.x * plantR.rootGrowth.y) + (math.pow(plantR.rootGrowth.x / 2, 2) * 2);
        float rootUnderWaterPercent = 1;
        //float rootUnderWaterPercent = 1 - (GetEarth().GetZoneController().zones[organisms[organism].zone].waterDepth / rootGrowth.y);
        float waterGain = rootArea * rootUnderWaterPercent * 1;

        Plant plantW = organismR.GetWritable().GetOrgan<Plant>();
        foreach (var speciesOrgan in organs) {
            ((PlantSpeciesOrgan)speciesOrgan).GrowOrgan(organismR, plantR, plantW, GetEarth().simulationDeltaTime * Mathf.Sqrt(sunGain * waterGain));
        }
    }

    public override void OnSettingsChanged(bool renderOrganisms) {
        //for (int i = 0; i < activePlants.Count; i++) {
        //    plants[activePlants[i]].CheckRendering();
        //}
        throw new NotImplementedException();
    }

    public PlantSpeciesSeed GetPlantSpeciesSeeds() {
        return (PlantSpeciesSeed)organs.FirstOrDefault(o => o is PlantSpeciesSeed);
    }
}
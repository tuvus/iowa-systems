﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System;
using static Earth;
using UnityEngine.UIElements;
using static PlantSpecies;
using UnityEditor.SceneManagement;

public class PlantSpecies : Species {
    public GameObject plantPrefab;
    PlantSpeciesSeeds plantSpeciesSeeds;

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
    [SerializeField] List<GrowthStageData> growthStagesInput = new List<GrowthStageData>();

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

    public NativeArray<GrowthStageData> growthStages;

    public NativeArray<Plant> plants;

    [System.Serializable]
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
    }

    #region StartSimulation
    public override void SetupSimulation(Earth earth) {
        base.SetupSimulation(earth);
        plantSpeciesSeeds = GetComponent<PlantSpeciesSeeds>();
        growthStages = new NativeArray<GrowthStageData>(growthStagesInput.Count, Allocator.Persistent);
        for (int i = 0; i < growthStagesInput.Count; i++) {
            growthStages[i] = growthStagesInput[i];
        }
        for (int i = 0; i < organs.Count; i++) {
            ((PlantSpeciesOrgan)organs[i]).growthPriorities = new NativeArray<float>(growthStages.Length, Allocator.Persistent);
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

    public override void SetupArrays(int arrayLength) {
        plants = new NativeArray<Plant>(arrayLength, Allocator.Persistent);
        base.SetupArrays(arrayLength);
    }

    public override void SetupSpeciesFoodType() {
        for (int i = 0; i < organs.Count; i++) {
            ((PlantSpeciesOrgan)organs[i]).SetupSpeciesOrganFoodType();
        }
    }

    public override void StartSimulation() {
        Populate();
    }

    public override void Populate() {
        for (int i = 0; i < startingPopulation; i++) {
            SpawnOrganism();
        }
        if (plantSpeciesSeeds != null) {
            plantSpeciesSeeds.Populate();
        }
        GetEarth().StartFindZoneJobs();
        GetEarth().CompleteFindZoneJobs();
    }
    #endregion

    public override int SpawnOrganism() {
        int plant = base.SpawnOrganism();
        organisms[plant] = new Organism(organisms[plant], 0, -1, Vector3.zero, 0);
        plants[plant] = new Plant(GrowthStage.Adult, 10, 10, 10);
        throw new NotImplementedException();
        return plant;
    }

    public override int SpawnOrganism(float3 position, float distance) {
        int plant = base.SpawnOrganism();
        organisms[plant] = new Organism(organisms[plant], 0, -1, Vector3.zero, 0);
        plants[plant] = new Plant(GrowthStage.Germinating, 10, 10, 10);
        throw new NotImplementedException();
        return plant;
    }

    public GrowthStageData GetGrowthStageData(GrowthStage stage) {
        return growthStages[(int)stage];
    }

    protected override void UpdateOrganism(int organism) {
        base.UpdateOrganism(organism);
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
            float objectDistanceFromSun = Vector3.Distance(organisms[organism].position, GetEarth().GetSunPosition());
            float sunDistanceFromEarth = Vector3.Distance(new float3(0, 0, 0), GetEarth().GetSunPosition());
            sunValue =  Mathf.Max((objectDistanceFromSun - sunDistanceFromEarth) / GetEarth().GetRadius() * 2, 0);
        }
        float sunGain = bladeArea * sunValue;
        float rootArea = (math.PI * rootGrowth.x * rootGrowth.y) + (math.pow(rootGrowth.x / 2, 2) * 2);
        float rootUnderWaterPercent = 1 - (GetEarth().GetZoneController().zones[organisms[organism].zone].waterDepth / rootGrowth.y);
        float waterGain = rootArea * rootUnderWaterPercent * 1;
        for (int i = 0; i < organs.Count; i++) {
            ((PlantSpeciesOrgan)organs[i]).GrowOrgan(GetEarth().simulationDeltaTime * Mathf.Sqrt(sunGain * waterGain), ref bladeArea, ref stemHeight, ref rootGrowth);
        }
        plants[organism] = new Plant(stage, bladeArea, stemHeight, rootGrowth);
    }

    public override void OnSettingsChanged(bool renderOrganisms) {
        //for (int i = 0; i < activePlants.Count; i++) {
        //    plants[activePlants[i]].CheckRendering();
        //}
        throw new NotImplementedException();
    }

    public PlantSpeciesSeeds GetSpeciesSeeds() {
        return plantSpeciesSeeds;
    }

    /// <summary>
    /// Called after a simulation has ended but also after the intro scene is unloaded.
    /// </summary>
    public override void OnDestroy() {
        base.OnDestroy();
        if (growthStages.IsCreated)
            growthStages.Dispose();
    }
}
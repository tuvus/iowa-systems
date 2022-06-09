using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class Plant : Organism {
    public enum GrowthStage {
        Dead = -1,
        Seed = 0,
        Germinating = 1,
        Sprout = 2,
        Seedling = 3,
        Youngling = 4,
        Adult = 5,
    }
    public PlantSpecies plantSpecies;
    Plant plantParent;

    public int plantDataIndex;
    public float growth;

    public List<PlantOrgan> organs;
    public List<EddiblePlantOrgan> eddibleOrgans;

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
        public GrowthStage growthStage;

        public PlantData(Plant plant, float bladeArea, float stemHeight, float2 rootGrowth, GrowthStage stage) {
            age = plant.age;
            speciesIndex = plant.species.speciesIndex;
            specificSpeciesIndex = plant.species.specificSpeciesIndex;
            plantIndex = plant.specificOrganismIndex;
            zone = plant.zone;
            position = plant.position;
            this.bladeArea = bladeArea;
            this.stemHeight = stemHeight;
            this.rootGrowth = rootGrowth;
            rootDensity = .1f;
            this.growthStage = stage;
        }
        public PlantData(PlantData plantData, float bladeArea, float stemHeight, float2 rootGrowth, GrowthStage stage) {
            age = plantData.age;
            speciesIndex = plantData.speciesIndex;
            specificSpeciesIndex = plantData.specificSpeciesIndex;
            plantIndex = plantData.plantIndex;
            zone = plantData.zone;
            position = plantData.position;
            this.bladeArea = bladeArea;
            this.stemHeight = stemHeight;
            this.rootGrowth = rootGrowth;
            rootDensity = .1f;
            this.growthStage = stage;
        }

        public PlantData(PlantData plantData, float age, float bladeArea, float stemHeight, float2 rootGrowth, GrowthStage stage) {
            this.age = age;
            speciesIndex = plantData.speciesIndex;
            specificSpeciesIndex = plantData.specificSpeciesIndex;
            plantIndex = plantData.plantIndex;
            zone = plantData.zone;
            position = plantData.position;
            this.bladeArea = bladeArea;
            this.stemHeight = stemHeight;
            this.rootGrowth = rootGrowth;
            rootDensity = .1f;
            this.growthStage = stage;
        }

    }

    #region setup
    public void SetupOrganism(PlantSpecies plantSpecies) {
        base.SetupOrganism(plantSpecies);
        this.plantSpecies = plantSpecies;
        position = transform.position;
        gameObject.name = plantSpecies + "Organism";
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(this, 0, 0, new float2(0, 0), GrowthStage.Dead);
    }

    public void SpawnPlantRandom(GrowthStage stage) {
        SetPlantToStage(stage);
        //age = plantSpecies.GetGrowthStageData(stage).daysAfterGermination * 24;
        CheckRendering();
        for (int i = 0; i < organs.Count; i++) {
            organs[i].SpawnOrganismAdult();
        }
    }

    public void SpawnSeedRandom(float age) {
        SetPlantToStage(GrowthStage.Seed);
        this.age = age;
    }

    public void SpawnSeed() {
        SetPlantToStage(GrowthStage.Seed);
        age = 0;
    }
    #endregion

    #region PlantUpdate
    public override void RefreshOrganism() {
    }

    public void UpdateOrganismBehavior(float sunGain, float waterGain, GrowthStage stage) {
        if (!spawned)
            return;
        if (GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage == GrowthStage.Seed) {
            if (stage == GrowthStage.Dead) {
                KillOrganism();
                return;
            }
            if (stage == GrowthStage.Germinating) {
                SeedGerminated();
                //User.Instance.GetUserMotor().DebugPause();
                age = 0;
                return;
            }
            return;
        }
        if (GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage != stage) {
            ChangeGrowthStage(stage);
        }
        growth = GetEarthScript().simulationDeltaTime * Mathf.Sqrt(sunGain * waterGain);
    }

    public override void UpdateOrganism() {
        if (spawned) {
            age += GetEarthScript().simulationDeltaTime;
            GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], age, GetEarthScript().GetZoneController().allPlants[plantDataIndex].bladeArea, GetEarthScript().GetZoneController().allPlants[plantDataIndex].stemHeight, GetEarthScript().GetZoneController().allPlants[plantDataIndex].rootGrowth, GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage);
            growth += GetGrowth();
            Grow(growth);
        }
    }
    #endregion

    #region PlantControls
    public override void AddToZone(int zoneIndex) {
        for (int i = 0; i < organs.Count; i++) {
            organs[i].OnPlantAddToZone(zone, new ZoneController.DataLocation(this));
        }
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(this, GetEarthScript().GetZoneController().allPlants[plantDataIndex].bladeArea, GetEarthScript().GetZoneController().allPlants[plantDataIndex].stemHeight, GetEarthScript().GetZoneController().allPlants[plantDataIndex].rootGrowth, GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage);
    }

    public override void RemoveFromZone() {
        base.RemoveFromZone();
    }

    public void Grow(float growth) {
        if (growth == 0) {
            return;
        }
        for (int i = 0; i < organs.Count; i++) {
            if (organs[i].GetGrowthPriority(GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage) > 0) {
                organs[i].GrowOrgan(growth * organs[i].GetGrowthPriority(GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage));
            }
        }
    }

    public float EatPlant(Animal animal, float biteAmount) {
        if (!spawned)
            return 0;
        float foodReturn = 0;
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], age, plantSpecies.GetGrowthStageData(GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage).bladeArea, plantSpecies.GetGrowthStageData(GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage).stemHeight, plantSpecies.GetGrowthStageData(GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage).rootGrowth, GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage);
        for (int i = eddibleOrgans.Count - 1; i >= 0; i--) {
            //Multipling and dividing by 10 reduces the total food gained by eating an entire plant
            float newFood = eddibleOrgans[i].EatPlantOrgan(animal, biteAmount * 10) / 10;
            foodReturn += newFood;
            biteAmount -= newFood;
        }
        return foodReturn;
    }

    void SeedGerminated() {
        GetMeshRenderer().enabled = User.Instance.GetRenderWorldUserPref();
        SetPlantToStage(GrowthStage.Seed);
        for (int i = 0; i < organs.Count; i++) {
            organs[i].OnOrganismGermination();
        }
    }

    public override void KillOrganism() {
        if (GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage == GrowthStage.Seed)
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
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], 0, 0, new float2(0, 0), GrowthStage.Dead);
        //print(GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage + " " + GetEarthScript().GetZoneController().allPlants[plantDataIndex].position + " " + GetEarthScript().GetZoneController().allPlants[plantDataIndex].zone + " " + GetEarthScript().GetZoneController().allPlants[plantDataIndex].age);
        zone = -1;
        age = 0;
        for (int i = 0; i < organs.Count; i++) {
            organs[i].ResetOrgan();
        }
    }

    public void CheckRendering() {
        if ((int)GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage > (int)GrowthStage.Germinating)
            GetMeshRenderer().enabled = User.Instance.GetRenderWorldUserPref();
    }

    public void ChangeBladeArea(float bladeArea) {
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], bladeArea, GetEarthScript().GetZoneController().allPlants[plantDataIndex].stemHeight, GetEarthScript().GetZoneController().allPlants[plantDataIndex].rootGrowth, GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage);
    }

    public void ChangeStemHeight(float stemHeight) {
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], GetEarthScript().GetZoneController().allPlants[plantDataIndex].bladeArea, stemHeight, GetEarthScript().GetZoneController().allPlants[plantDataIndex].rootGrowth, GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage);
    }

    public void ChangeRootGrowth(float2 rootGrowth) {
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], GetEarthScript().GetZoneController().allPlants[plantDataIndex].bladeArea, GetEarthScript().GetZoneController().allPlants[plantDataIndex].stemHeight, rootGrowth, GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage);
    }

    public void ChangeGrowthStage(GrowthStage stage) {
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], GetEarthScript().GetZoneController().allPlants[plantDataIndex].bladeArea, GetEarthScript().GetZoneController().allPlants[plantDataIndex].stemHeight, GetEarthScript().GetZoneController().allPlants[plantDataIndex].rootGrowth, stage);
    }

    public void SetPlantToStage(GrowthStage stage) {
        age = plantSpecies.GetGrowthStageData(stage).daysAfterGermination * 24;
        GetEarthScript().GetZoneController().allPlants[plantDataIndex] = new PlantData(GetEarthScript().GetZoneController().allPlants[plantDataIndex], age, plantSpecies.GetGrowthStageData(stage).bladeArea, plantSpecies.GetGrowthStageData(stage).stemHeight, plantSpecies.GetGrowthStageData(stage).rootGrowth, stage);
    }

    public void PrintGrowth() {
        print("Stage: " + GetEarthScript().GetZoneController().allPlants[plantDataIndex].growthStage + ", BladeArea: " + GetEarthScript().GetZoneController().allPlants[plantDataIndex].bladeArea + ", StemHeight: " + GetEarthScript().GetZoneController().allPlants[plantDataIndex].stemHeight + ", Rootgrowth: " + GetEarthScript().GetZoneController().allPlants[plantDataIndex].rootGrowth);
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
    #endregion
}
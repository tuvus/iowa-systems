using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class PlantOrgan : Organ {
    internal bool spawned;

    public void SetupOrgan(PlantSpeciesOrgan plantSpeciesOrgan, Plant plant) {
        base.SetupOrgan(plantSpeciesOrgan, plant);
        plant.organs.Add(this);
    }

    public virtual void SpawnOrganismAdult() {
        return;
    }

    public override void ResetOrgan() {
        Despawn();
    }

    internal void Spawn() {
        if (!spawned) {
            spawned = true;
            AddToZone(GetPlant().zone, new ZoneController.DataLocation(GetPlant()));
        }
    }

    internal void Despawn() {
        if (spawned) {
            spawned = false;
            RemoveFromZone(GetPlant().zone, new ZoneController.DataLocation(GetPlant()));
        }
    }

    public virtual void OnPlantAddToZone(int zone, ZoneController.DataLocation dataLocation) {
        return;
    }


    public virtual void OnOrganismGermination() {
        return;
    }

    public abstract void GrowOrgan(float growth);
    
    /// <summary>
    /// Raw growth gained from the organ.
    /// </summary>
    /// <returns>float</returns>
    public virtual float GetGrowth(float deltaTime) {
        return 0;
    }
   
    /// <summary>
    /// Returns a value from 0 to 1 depending on how much growth should be spend in this organ.
    /// </summary>
    /// <returns>float</returns>
    public float GetGrowthPriority(PlantSpecies.GrowthStage stage) {
        return GetPlantSpeciesOrgan().growthPriorities[(int)stage];
    }

    public PlantSpeciesOrgan GetPlantSpeciesOrgan() {
        return (PlantSpeciesOrgan)GetSpeciesOrgan();
    }

    public Plant GetPlant() {
        return (Plant)GetOrganism();
    }

}
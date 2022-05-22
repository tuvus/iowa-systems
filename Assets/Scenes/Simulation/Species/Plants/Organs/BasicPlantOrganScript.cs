using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class BasicPlantOrganScript : BasicOrganScript {
    internal BasicPlantSpeciesOrganScript basicPlantSpeciesOrganScript;
    internal PlantScript plantScript;
    internal float growthPriority;
    internal bool spawned;

    internal override void SetupOrgan(BasicSpeciesOrganScript basicSpeciesOrganScript) {
        basicPlantSpeciesOrganScript = (BasicPlantSpeciesOrganScript)basicSpeciesOrganScript;
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
        plantScript.organs.Add(this);
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
            AddToZone(plantScript.zone, new ZoneController.DataLocation(plantScript));
        }
    }

    internal void Despawn() {
        if (spawned) {
            spawned = false;
            RemoveFromZone(plantScript.zone, new ZoneController.DataLocation(plantScript));
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
    public float GetGrowthPriority(PlantScript.GrowthStage stage) {
        return basicPlantSpeciesOrganScript.growthPriorities[(int)stage];
    }

}
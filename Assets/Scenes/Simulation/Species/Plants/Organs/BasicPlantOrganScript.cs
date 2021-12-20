using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class BasicPlantOrganScript : BasicOrganScript {
    internal BasicPlantSpeciesOrganScript basicPlantSpeciesOrganScript;
    internal PlantScript plantScript;
    internal float growthPriority;

    internal override void SetupOrgan(BasicSpeciesOrganScript _basicSpeciesOrganScript) {
        basicPlantSpeciesOrganScript = (BasicPlantSpeciesOrganScript)_basicSpeciesOrganScript;
        plantScript = basicOrganismScript.GetComponent<PlantScript>();
        plantScript.organs.Add(this);
    }

    public abstract void ResetOrgan();

    public abstract void GrowOrgan(float growth);

    public abstract void UpdateGrowthPriority();

    public virtual float GetBladeArea() {
        return 0;
    }
    
    public virtual float2 GetRootGrowth() {
        return new float2(0,0);
    }

    public virtual float GetStemheight() {
        return 0;
    }
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
    public float GetGrowthPriority() {
        return growthPriority;
    }

    public virtual void OnOrganismDeath() {
        return;
    }

    public virtual void OnOrganismGermination() {
        return;
    }

    public virtual void AddToZone(int zoneIndex, int plantDataIndex) {
        return;
    }

    public virtual void RemoveFromZone(int zoneIndex) {
        return;
    }

    internal void RemoveFoodTypeFromZone(int zoneIndex,int foodTypeIndex) {
        if (plantScript.GetEarthScript().GetZoneController().organismsByFoodTypeInZones.TryGetFirstValue(foodTypeIndex, out int value, out var iterator)) {
            do {
                if (value == plantScript.plantDataIndex) {
                    plantScript.GetEarthScript().GetZoneController().organismsByFoodTypeInZones.Remove(iterator);
                    return;
                }
            } while (plantScript.GetEarthScript().GetZoneController().organismsByFoodTypeInZones.TryGetNextValue(out value, ref iterator));
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicOrganScript : MonoBehaviour {
    internal BasicSpeciesOrganScript basicSpeciesOrganScript;
    internal BasicOrganismScript basicOrganismScript;
    public void SetupBasicOrgan(BasicSpeciesOrganScript basicSpeciesOrganScript, BasicOrganismScript basicOrganismScript) {
        this.basicSpeciesOrganScript = basicSpeciesOrganScript;
        this.basicOrganismScript = basicOrganismScript;
        SetupOrgan(basicSpeciesOrganScript);
        SetUpSpecificOrgan();
    }

    internal abstract void SetupOrgan(BasicSpeciesOrganScript basicSpeciesOrganScript);

    internal abstract void SetUpSpecificOrgan();

    public abstract void ResetOrgan();

    public virtual void OnOrganismDeath() {
        return;
    }

    public virtual void AddToZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        return;
    }

    public virtual void RemoveFromZone(int zoneIndex, ZoneController.DataLocation dataLocation) {
        return;
    }

    internal virtual void RemoveFoodTypeFromZone(int zoneIndex) {
        return;
    }

    public ZoneController GetZoneController() {
        return basicOrganismScript.GetZoneController();
    }
}

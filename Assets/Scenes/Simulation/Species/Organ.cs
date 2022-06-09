using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Organ : MonoBehaviour {
    private SpeciesOrgan speciesOrgan;
    private Organism organism;

    public void SetupOrgan(SpeciesOrgan basicSpeciesOrganScript, Organism basicOrganismScript) {
        this.speciesOrgan = basicSpeciesOrganScript;
        this.organism = basicOrganismScript;
    }

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
        return organism.GetZoneController();
    }

    public SpeciesOrgan GetSpeciesOrgan() {
        return speciesOrgan;
    }

    public Organism GetOrganism() {
        return organism;
    }
}

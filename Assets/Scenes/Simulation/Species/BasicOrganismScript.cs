using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicOrganismScript : MonoBehaviour {
	internal BasicSpeciesScript species;
	internal BasicOrganismScript parent;

	public int organismIndex;
	public int specificOrganismIndex;
	public bool spawned;
	[SerializeField] internal float age;

	public Vector3 position;
	public int zone;

	internal MeshRenderer meshRenderer;



	#region OrganismSetup
	internal void SetUpOrganism(BasicSpeciesScript species) {
		User.Instance.ChangedSettings += OnSettingsChanged;
		this.species = species;
		meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
		GetMeshRenderer().material.color = species.speciesColor;
		GetOrganismMotor().SetupOrganismMotor(species.earth,this);
	}

	public void OnAddOrganism(object sender, System.EventArgs info) {
		GetEarthScript().OnEndFrame -= OnAddOrganism;
		species.AddOrganism(this);
    }
    #endregion

    #region OrganismUpdate
    public abstract void RefreshOrganism();

	public abstract void UpdateOrganism();

    public void OnSettingsChanged(User _user, SettingsEventArgs _settings) {
		if (spawned)
			GetMeshRenderer().enabled = _settings.Rendering;
	}
	#endregion

	#region OrganismControlls
	public abstract void AddToZone(int zoneIndex);

	public virtual void RemoveFromZone() {
	}

	public void SetOrganismZone(int zone) {
		if (this.zone != zone) {
			if (this.zone != -1)
				RemoveFromZone();
			this.zone = zone;
			if (zone != -1)
				AddToZone(zone);
		}
	}

	public virtual void KillOrganism() {
		species.OrganismDeath();
		OrganismDied();
	}

	internal abstract void OrganismDied();
    #endregion

    #region GetMethods
    public MeshRenderer GetMeshRenderer() {
		return meshRenderer;
    }

	public OrganismMotor GetOrganismMotor() {
		return transform.GetChild(0).GetComponent<OrganismMotor>();
	}

	public EarthScript GetEarthScript() {
		return species.GetEarthScript();
    }

	public ZoneController GetZoneController() {
		return GetEarthScript().GetZoneController();
	}
	#endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicOrganismScript : MonoBehaviour {
	internal BasicSpeciesScript species;
	internal BasicOrganismScript parent;

	public int organismIndex;
	public int specificOrganismIndex;
	[SerializeField] internal float age;

	internal bool organismDead;

	public Vector3 position;
	public int zone;


	#region OrganismSetup
	public void SetUpOrganism(BasicSpeciesScript species, BasicOrganismScript newParent) {
		User.Instance.ChangedSettings += OnSettingsChanged;
		this.species = species;
		parent = newParent;
		SetUpSpecificOrganism(parent);
	}

	public abstract void SetUpSpecificOrganism(BasicOrganismScript parent);

	public void OnAddOrganism(object sender, System.EventArgs info) {
		GetEarthScript().OnEndFrame -= OnAddOrganism;
		species.AddOrganism(this);
    }
    #endregion

    #region OrganismUpdate
    public abstract void RefreshOrganism();

	public abstract void UpdateOrganism();

    public void OnSettingsChanged(User _user, SettingsEventArgs _settings) {
		if (organismDead == false)
			GetMeshRenderer().enabled = _settings.Rendering;
	}
    #endregion

    #region RemoveOrganism
    public void KillOrganism() {
		species.OrganismDeath();
		organismDead = true;
		OrganismDied();
    }

	internal abstract void OrganismDied();
    #endregion

    #region GetMethods
    public MeshRenderer GetMeshRenderer() {
		return transform.GetChild(0).GetComponent<MeshRenderer>();
    }

	public OrganismMotor GetOrganismMotor() {
		return transform.GetChild(0).GetComponent<OrganismMotor>();
	}

	public EarthScript GetEarthScript() {
		return species.GetEarthScript();
    }
    #endregion
}

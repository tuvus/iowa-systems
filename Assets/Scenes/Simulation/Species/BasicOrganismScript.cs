using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicOrganismScript : MonoBehaviour {
	internal BasicSpeciesScript species;
	internal BasicOrganismScript parent;
	[SerializeField] internal float age;

	internal bool organismDead;

	public Vector3 position;

    #region OrganismSetup
    public void SetUpOrganism(BasicSpeciesScript species, BasicOrganismScript newParent) {
		User.Instance.changedSettings += OnSettingsChanged;
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
			GetMeshRenderer().enabled = _settings.rendering;
	}
    #endregion

    #region RemoveOrganism
    public void KillOrganism() {
		organismDead = true;
		species.OrganismDeath(this);
		OrganismDied();
		GetEarthScript().OnEndFrame += OnDestroyOrganism;
    }

	internal void OnDestroyOrganism(object sender, System.EventArgs info) {
		GetEarthScript().OnEndFrame -= OnDestroyOrganism;
		DestroyOrganism();
	}

	internal abstract void OrganismDied();

	void DestroyOrganism() {
		species.RemoveOrganism(this);
		Destroy(gameObject);
	}
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

	public abstract string GetFoodType();

	public Eddible GetEddible() {
		return GetComponent<Eddible>();
    }
    #endregion
}

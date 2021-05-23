using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicOrganismScript : MonoBehaviour {
	internal Rigidbody rb;

	internal BasicSpeciesScript species;
	internal float age;

	internal bool organismDead;

	public void SetUpOrganism(BasicSpeciesScript _species) {
		User.Instance.changedSettings += OnSettingsChanged;
		rb = gameObject.GetComponent<Rigidbody>();
		species = _species;
		SetUpSpecificOrganism();
	}

	public abstract void SetUpSpecificOrganism();

    public void OnSettingsChanged(User _user, SettingsEventArgs _settings) {
		if (organismDead == false)
			GetComponent<MeshRenderer>().enabled = _settings.rendering;
	}

	public void KillOrganism() {
		species.OrganismDealth();
		OrganismDied();
		Destroy(gameObject);
		organismDead = true;
    }

	internal abstract void OrganismDied();

}

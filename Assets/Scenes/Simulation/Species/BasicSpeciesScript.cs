using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicSpeciesScript : MonoBehaviour {
	internal EarthScript earth;
	internal SunScript sun;
	internal SpeciesMotor history;
	public string speciesName;
	public string speciesDisplayName;
	public Color speciesColor;
	internal List<int> populationOverTime = new List<int>();
	[SerializeField]

	//Population start stats
	public int organismCount;

	public void StartBasicSimulation(EarthScript _earth, SunScript _sun) {
		earth = _earth;
		sun = _sun;
        foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.SetBasicSpeciesScript(this);
        }
		StartSimulation();
		StartSpecificSimulation();
	}

	internal abstract void StartSimulation();

	internal abstract void StartSpecificSimulation();


	public abstract void Populate();

	public abstract void SpawnSpecificRandomOrganism();

	public abstract GameObject SpawnSpecificOrganism(GameObject _parent);


	internal BasicOrganismScript SpawnRandomOrganism(GameObject organism) {
		GameObject newOrganism = Instantiate(organism, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
		newOrganism.transform.SetParent(earth.GetOrganismsTransform());

		newOrganism.GetComponent<Renderer>().material.color = speciesColor;
		newOrganism.GetComponent<Renderer>().enabled = User.Instance.GetRenderWorldUserPref();
		BasicOrganismScript basicOrganism = newOrganism.GetComponent<BasicOrganismScript>();
		new SpawnRandomizer().SpawnRandom(newOrganism.transform, earth);
		basicOrganism.species = this;

		organismCount++;
		return newOrganism.GetComponent<BasicOrganismScript>();
	}

	internal BasicOrganismScript SpawnOrganism(GameObject _parent, GameObject organism) {
		GameObject newOrganism = Instantiate(organism, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
		newOrganism.transform.SetParent(earth.GetOrganismsTransform());
		newOrganism.GetComponent<Renderer>().material.color = speciesColor;
		newOrganism.GetComponent<Renderer>().enabled = User.Instance.GetRenderWorldUserPref();
		BasicOrganismScript basicOrganism = newOrganism.GetComponent<BasicOrganismScript>();
		new SpawnRandomizer().SpawnFromParent(newOrganism.transform, _parent, 0.8f, earth);
		basicOrganism.species = this;

		organismCount++;
		return newOrganism.GetComponent<BasicOrganismScript>();
	}

	public EarthScript GetEarthScript() {
		return earth;
	}

	public SunScript GetSunScript() {
		return sun;
	}

	public List<int> ReturnPopulationList() {
		return populationOverTime;
	}

	public void RefreshPopulationList() {
		populationOverTime.Add(organismCount);
    }

	public int GetCurrentPopulation() {
		return organismCount;
    }

	public GameObject InstantiateNewOrgan(GameObject _organ, GameObject _organism) {
		return Instantiate(_organ, _organism.transform);
	}

	public void OrganismDealth() {
		organismCount--;
    }
}
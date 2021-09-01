using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public abstract class BasicSpeciesScript : MonoBehaviour {
	internal EarthScript earth;
	internal SunScript sun;
	public string speciesName;
	public string speciesDisplayName;
	public Color speciesColor;
	[SerializeField] internal int startingPopulation;
	internal List<int> populationOverTime = new List<int>();
	[SerializeField] List<BasicOrganismScript> organisms = new List<BasicOrganismScript>();

    #region SimulationStart
	public void SetupSimulation(EarthScript _earth, SunScript _sun) {
		earth = _earth;
		sun = _sun;
		gameObject.name = speciesName;
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.SetBasicSpeciesScript(this);
        }
		SetupSpecificSimulation();
    }

	internal abstract void SetupSpecificSimulation();

    public void StartBasicSimulation() {
		StartSimulation();
		StartSpecificSimulation();
	}

	internal abstract void StartSimulation();

	internal abstract void StartSpecificSimulation();

	public abstract void Populate();
    #endregion

    #region SpawnOrganisms
    public abstract void SpawnSpecificRandomOrganism();

	public abstract BasicOrganismScript SpawnSpecificOrganism(BasicOrganismScript _parent);


	internal BasicOrganismScript SpawnOrganism(GameObject organismPrefab) {
		BasicOrganismScript basicOrganism = Instantiate(organismPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null).GetComponent<BasicOrganismScript>();
		basicOrganism.GetMeshRenderer().material.color = speciesColor;
		basicOrganism.GetMeshRenderer().enabled = User.Instance.GetRenderWorldUserPref();
		basicOrganism.species = this;

		return basicOrganism;
	}

	internal void SetupRandomOrganism(BasicOrganismScript organism) {
		organism.GetOrganismMotor().SetupOrganism(earth,organism);
		new SpawnRandomizer().SpawnRandom(organism.GetOrganismMotor());
	}

	internal void SetupChildOrganism(BasicOrganismScript organism, BasicOrganismScript parent, float range = 2) {
		organism.GetOrganismMotor().SetupChildOrganism(earth, parent.GetOrganismMotor().GetRotationTransform().position, parent.GetOrganismMotor().GetRotationTransform().eulerAngles,organism);
		new SpawnRandomizer().SpawnFromParent(organism.GetOrganismMotor(), range);
	}

	public GameObject InstantiateNewOrgan(GameObject _organ, BasicOrganismScript _organism) {
		return Instantiate(_organ, _organism.transform);
	}
	#endregion


    #region PopulationCountGraph
    public List<int> ReturnPopulationList() {
		return populationOverTime;
	}

	public void RefreshPopulationList() {
		populationOverTime.Add(GetCurrentPopulation());
    }

	public int GetCurrentPopulation() {
		return organisms.Count;
    }
	#endregion

	#region OrganismControls
	public abstract void UpdateOrganismsBehavior();

	public abstract void UpdateOrganisms();

    public void AddOrganism(BasicOrganismScript newOrganism) {
		organisms.Add(newOrganism);
		earth.AddObject(newOrganism,this);
		AddSpecificOrganism(newOrganism);
	}

	internal abstract void AddSpecificOrganism(BasicOrganismScript newOrganism);

	/// <summary>
	/// Called right when an organism detects it should be dead.
	/// </summary>
	public void OrganismDeath(BasicOrganismScript deadOrganism) {
		organisms.Remove(deadOrganism);
		SpecificOrganismDeath(deadOrganism);
    }

	internal abstract void SpecificOrganismDeath(BasicOrganismScript deadOrganism);
	
	/// <summary>
	/// Called after the frame to remove the organism from the EarthScript while it is not iterating through it's list of organisms.
	/// </summary>
	public void RemoveOrganism(BasicOrganismScript removeOrganism) {
		earth.RemoveObject(removeOrganism,this);
	}
    #endregion

    #region GetMethods
    public List<BasicOrganismScript> GetOrganisms() {
		return organisms;
    }
	
	public SpeciesMotor GetSpeciesMotor() {
		return SpeciesManager.Instance.GetSpeciesMotor();
    }

	public string GetFoodType() {
		return speciesName;
    }
    public EarthScript GetEarthScript() {
		return earth;
	}

	public SunScript GetSunScript() {
		return sun;
	}

	public BasicJobController GetBasicJobController() {
		return GetComponent<BasicJobController>();
    }
    #endregion
}
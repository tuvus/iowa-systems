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
	public int speciesIndex;
	public int specificSpeciesIndex;
	internal List<int> populationOverTime = new List<int>();
	[SerializeField] List<BasicOrganismScript> organisms = new List<BasicOrganismScript>();
	public int populationCount { internal set; get; }

	BasicJobController jobController;

	#region SimulationStart

	public void SetupSimulation(EarthScript earth, SunScript sun) {
		this.earth = earth;
		this.sun = sun;
		gameObject.name = speciesName;
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.SetBasicSpeciesScript(this);
		}
		SetupSpecificSimulation();
		jobController = GetComponent<BasicJobController>();
	}

	internal abstract void SetupSpecificSimulation();

	public abstract void SetupSpeciesFoodType();

	public void StartBasicSimulation() {
		StartSimulation();
	}

	internal abstract void StartSimulation();

	public abstract void Populate();
	#endregion

	#region SpawnOrganisms
	public abstract void PreSpawn(int spawnNumber);

	public abstract void SpawnRandomOrganism();

	internal GameObject SpawnOrganism(GameObject organismPrefab) {
		return Instantiate(organismPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
	}

	internal void RandomiseOrganismPosition(BasicOrganismScript organism) {
		SpawnRandomizer.SpawnRandom(organism.GetOrganismMotor());
	}

	internal void RandomiseOrganismChildPosition(BasicOrganismScript organism, BasicOrganismScript parent, float range = 2) {
		SpawnRandomizer.SpawnFromParent(organism.GetOrganismMotor(), parent.GetOrganismMotor().GetRotationTransform().position, parent.GetOrganismMotor().GetRotationTransform().eulerAngles, range);
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
		return populationCount;
	}
	#endregion

	#region OrganismControls
	public abstract void UpdateOrganismData();

	public abstract void UpdateOrganismsBehavior();

	public abstract void UpdateOrganisms();

	public abstract void UpdateOrganismLists();

	public void AddOrganism(BasicOrganismScript newOrganism) {
		organisms.Add(newOrganism);
		newOrganism.organismIndex = organisms.Count - 1;
		AddSpecificOrganism(newOrganism);
	}

	internal abstract void AddSpecificOrganism(BasicOrganismScript newOrganism);

	/// <summary>
	/// Called right when an organism detects it should be dead.
	/// </summary>
	public void OrganismDeath() {
		populationCount--;
	}


	#endregion

    #region GetMethods
    public List<BasicOrganismScript> GetOrganisms() {
		return organisms;
    }
	
	public abstract List<string> GetOrganismFoodTypes();

    public EarthScript GetEarthScript() {
		return earth;
	}

	public SunScript GetSunScript() {
		return sun;
	}

	public BasicJobController GetBasicJobController() {
		return jobController;
    }
    #endregion
}
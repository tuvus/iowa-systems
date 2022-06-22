using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public abstract class Species : MonoBehaviour {
    private Earth earth;
    public string speciesName;
    public string speciesDisplayName;
    public Color32 speciesColor;
    [SerializeField] internal int startingPopulation;
    public int speciesIndex;
    public int specificSpeciesIndex;
    internal List<int> populationOverTime = new List<int>();
    [SerializeField] List<Organism> organisms = new List<Organism>();
    public int populationCount { internal set; get; }

    JobController jobController;

    #region SimulationStart

    public virtual void SetupSimulation(Earth earth) {
        this.earth = earth;
        gameObject.name = speciesName;
        foreach (var organ in GetComponents<SpeciesOrgan>()) {
            organ.SetSpeciesScript(this);
        }
        jobController = GetComponent<JobController>();
    }

    public abstract void SetupSpeciesFoodType();

    public abstract void StartSimulation();

    public abstract void Populate();
    #endregion

    #region SpawnOrganisms
    public abstract void PreSpawn(int spawnNumber);

    public abstract void SpawnRandomOrganism();

    internal GameObject SpawnOrganism(GameObject organismPrefab) {
        return Instantiate(organismPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
    }

    internal void RandomiseOrganismPosition(Organism organism) {
        SpawnRandomizer.SpawnRandom(organism.GetOrganismMotor());
    }

    internal void RandomiseOrganismChildPosition(Organism organism, Organism parent, float range = 2) {
        SpawnRandomizer.SpawnFromParent(organism.GetOrganismMotor(), range);
    }

    public GameObject InstantiateNewOrgan(GameObject organ, Organism organism) {
        return Instantiate(organ, organism.transform);
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
    public abstract void OnSettingsChanged(bool renderOrganisms);

    public abstract void UpdateOrganismData();

    public abstract void UpdateOrganismsBehavior();

    public abstract void UpdateOrganisms();

    public abstract void UpdateOrganismLists();

    public void AddOrganism(Organism newOrganism) {
        organisms.Add(newOrganism);
        newOrganism.organismIndex = organisms.Count - 1;
    }

    /// <summary>
    /// Called right when an organism detects it should be dead.
    /// </summary>
    public void OrganismDeath() {
        populationCount--;
        if (populationCount == 0)
            User.Instance.PrintState("Species has died out after " + (int)(earth.worldTime / 24) + " days.", speciesDisplayName,3);
    }


    #endregion

    #region GetMethods
    public List<Organism> GetOrganisms() {
        return organisms;
    }

    public abstract List<string> GetOrganismFoodTypes();

    public Earth GetEarth() {
        return earth;
    }

    public JobController GetBasicJobController() {
        return jobController;
    }
    #endregion
}
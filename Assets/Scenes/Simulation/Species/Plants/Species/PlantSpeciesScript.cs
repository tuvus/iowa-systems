using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesScript : BasicSpeciesScript {
	public GameObject plantType;

	public int seedCount;

	public float maxGrowth;

	internal override void StartSimulation() {
	}

	internal override void StartSpecificSimulation() {
		populationOverTime.Add(organismCount);
		gameObject.name = speciesName;
		history = GetComponentInParent<SpeciesMotor>();

		Populate();
	}

	public override void Populate() {
		int organismsToSpawn = organismCount;
		organismCount = 0;
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnSpecificRandomOrganism();
		}
		if (GetComponent<PlantSpeciesSeeds>() != null) {
			GetComponent<PlantSpeciesSeeds>().Populate(earth);
		}
	}

    public override void SpawnSpecificRandomOrganism() {
		GameObject newPlant = SpawnRandomOrganism(plantType).gameObject;
		PlantScript plantScipt = newPlant.GetComponent<PlantScript>();
		plantScipt.maxGrowth = maxGrowth;
		plantScipt.Grow(Random.Range(0.1f, maxGrowth),1);
		plantScipt.species = this;
		plantScipt.SetUpOrganism(this);
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(newPlant);
		}
	}

    public PlantScript SpawnOrganismFromSeed (GameObject _seed) {
		GameObject newOrganism = Instantiate(plantType, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
		newOrganism.transform.SetParent(earth.GetOrganismsTransform());
		newOrganism.GetComponent<Renderer>().material.color = speciesColor;
		newOrganism.GetComponent<Renderer>().enabled = User.Instance.GetRenderWorldUserPref();
		BasicOrganismScript basicOrganism = newOrganism.GetComponent<BasicOrganismScript>();
		newOrganism.transform.position = _seed.transform.position;
		//new SpawnRandomizer().SpawnFromParent(newOrganism.transform, _seed, 0, earth);
		basicOrganism.species = this;
		organismCount++;
		seedCount--;
		PlantScript plantScipt = newOrganism.GetComponent<PlantScript>();
		plantScipt.maxGrowth = maxGrowth;
		plantScipt.species = this;
		plantScipt.growth = 0.1f;
		plantScipt.health = plantScipt.growth;
		plantScipt.SetUpOrganism(this);
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(newOrganism);
		}
		return plantScipt;
	}

	public Seed SpawnRandomSeed(GameObject _seed) {
		GameObject newSeed = InstantiateNewSeed(_seed).gameObject;
		new SpawnRandomizer().SpawnRandom(newSeed.transform, earth);
		Seed seedScript = newSeed.GetComponent<Seed>();
		seedScript.species = this;
		seedCount++;
		return seedScript;
	}

	public Seed SpawnSeed(GameObject _parent, GameObject _seed, float range) {
		GameObject newSeed = InstantiateNewSeed(_seed).gameObject;
		new SpawnRandomizer().SpawnFromParent(newSeed.transform, _parent, range, earth);
		newSeed.transform.SetParent(earth.GetOrganismsTransform());
		Seed seedScript = newSeed.GetComponent<Seed>();
		seedScript.species = this;
		seedCount++;
		return seedScript;
	}

	public GameObject InstantiateNewSeed(GameObject _seed) {
		GameObject newSeed = Instantiate(_seed, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
		newSeed.transform.SetParent(earth.GetOrganismsTransform());

		newSeed.GetComponent<Renderer>().material.color = speciesColor;
		newSeed.GetComponent<Renderer>().enabled = User.Instance.GetRenderWorldUserPref();
		return newSeed;
	}

	public override GameObject SpawnSpecificOrganism(GameObject _parent) {
		GameObject newPlant = SpawnRandomOrganism(plantType).gameObject;
		PlantScript plantScipt = newPlant.GetComponent<PlantScript>();
		plantScipt.maxGrowth = maxGrowth;
		plantScipt.species = this;
		plantScipt.SetUpOrganism(this);
		foreach (var organ in GetComponents<BasicSpeciesOrganScript>()) {
			organ.MakeOrganism(newPlant);
		}
		return newPlant;
	}

	public void SeedDeath() {
		seedCount--;
	}
}

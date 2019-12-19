using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesSeeds : MonoBehaviour {
	public GameObject seed;

	private PlantSpeciesScript plantSpecies;
	public float humidityRequirement;
	public float tempetureRequirement;
	public float seedDispertionRange;

	void Start() {
		plantSpecies = GetComponent<PlantSpeciesScript>();
	}

	public void makeOrganism(GameObject _newOrganism) {
		Seeds seeds = _newOrganism.AddComponent<Seeds>();
		seeds.humidityRequirement = humidityRequirement;
		seeds.seedDispertionRange = seedDispertionRange;
		seeds.seed = seed;
	}
	public void MakePlant (GameObject _plantToGrow, float growth) {
		plantSpecies.SpawnSeedOrganism(_plantToGrow).gameObject.GetComponent<BasicPlantScript>().storedGrowth = growth;

	}

	public void Populate (int _populateCount, GameObject _earth) {
		for (int i = 0; i < _populateCount; i++) {
			GameObject newSeed = Instantiate(seed, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
			newSeed.transform.parent = _earth.transform;
			Seed seedScript = newSeed.GetComponent<Seed>();
			seedScript.humidityRequirement = humidityRequirement;
			seedScript.species = gameObject;
			seedScript.earth = _earth;
			seedScript.tempetureRequirement = tempetureRequirement;
		}
	}
}

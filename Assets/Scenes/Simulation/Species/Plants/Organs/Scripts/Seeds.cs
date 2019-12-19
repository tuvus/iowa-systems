using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seeds : MonoBehaviour {

	public GameObject seed;

	private BasicPlantScript basicPlant;
	public float seedDispertionRange;
	public float humidityRequirement;

	void Start() {
		basicPlant = GetComponent<BasicPlantScript>();
	}

	public void MakeSeed (GameObject _parent) {
		GameObject newSeed = Instantiate(seed, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1), null);
		newSeed.GetComponent<Seed>().earth = basicPlant.earth;
		newSeed.GetComponent<Seed>().humidityRequirement = humidityRequirement;
		newSeed.GetComponent<Seed>().species = basicPlant.plantSpecies;
		newSeed.GetComponent<SpawnRandomizer>().parent = _parent;
		newSeed.GetComponent<SpawnRandomizer>().range = seedDispertionRange;
	}
}

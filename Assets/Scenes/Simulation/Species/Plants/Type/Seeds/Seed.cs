using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour {

	public PlantSpeciesScript species;
	public PlantSpeciesSeeds speciesSeeds;

	public float humidityRequirement;
	public float tempetureRequirement;
	public float timeRequirement;
	public float maxTime;

	public void SetupSeed(float _humidity, float _tempature, float _timeRequirement, float _maxTime, PlantSpeciesSeeds _speciesSeeds) {
		humidityRequirement = _humidity;
		tempetureRequirement = _tempature;
		timeRequirement = _timeRequirement;
		speciesSeeds = _speciesSeeds;
		maxTime = _maxTime;
	}

	void FixedUpdate() {
		if (timeRequirement <= 0) {
			if (humidityRequirement <= species.GetEarthScript().GetComponent<EarthScript>().humidity && tempetureRequirement <= species.GetEarthScript().GetComponent<EarthScript>().tempeture) {
				speciesSeeds.MakePlant(gameObject, Random.Range(2.3f, 4.6f));
				//Multiply this by humidity
				Destroy(gameObject);
			}
		}
		if (timeRequirement < -maxTime) {
			print("SeedTimedOut");
			species.SeedDeath();
			Destroy(gameObject);
		} else {
			timeRequirement -= Time.fixedDeltaTime;
		}
	}
}
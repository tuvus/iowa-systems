using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour {

	public GameObject species;
	public GameObject earth;

	public float humidityRequirement;
	public float tempetureRequirement;

	void Start() {
		humidityRequirement = humidityRequirement * Random.Range(.8f, 1.2f);
		transform.localScale = (new Vector3(0.001f, 0.001f, 0.001f));
		transform.parent = earth.transform;
	}

	void FixedUpdate() {
		if (humidityRequirement <= earth.GetComponent<EarthScript>().humidity && tempetureRequirement <= earth.GetComponent<EarthScript>().tempeture) {
			if (Random.Range(0, 3) == 0) {
				species.GetComponent<PlantSpeciesSeeds>().MakePlant(gameObject, Random.Range(2.3f, 4.6f));
				//Multiply this by humidit
				Destroy(gameObject);
			}
		}
	}
}
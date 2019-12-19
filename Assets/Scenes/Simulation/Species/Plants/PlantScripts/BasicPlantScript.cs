using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlantScript : MonoBehaviour {
	public GameObject earth;
	private GameObject sun;
	public GameObject plantSpecies;
	//Plants Stats
	public float age;
	public string type;
	public string species;
	public float storedGrowth;
	public float fertility;
	public float fertilityConsumption;
	public int organismCount;
	public bool refreshed;
	public bool inSun;

	/*
	public float reproductionChance;
	public float spreadRadius;
	//Plants berry growth Stats
	public float minFoodGrothTime;
	public float maxFoodGrothTime;
	public float foodGrowthAge;
	public float grothTime;
	public float growRadius;
	//Berry's stats
	public string foodType;
	public float foodCount;
	public float foodGain;
	public float eatNoiseRange;
	public float deteriationTime;

	void Start() {

		FindStartPos();

		earth = GameObject.Find("Earth");
		transform.SetParent (earth.transform);
	}

	public void FindStartPos () {
	}*/

	void Start() {
		refreshed = false;
		earth = GameObject.Find("Earth");
		sun = GameObject.Find("Sun");
		transform.SetParent(earth.transform);
		fertilityConsumption = fertilityConsumption * Random.Range(0.8f, 1.2f);
	}

	void FixedUpdate() {
		age += 0.0001f;
		if (earth.GetComponent<EarthScript>().time == 0) {
			CheckSun();
			refreshed = true;
			CheckFertility();
		} else {
			refreshed = false;
		}
	}
	public void CheckSun () {
		//Debug.Log(transform.position + " " + sun.transform.position);
		//if (Physics.Linecast(transform.position, sun.transform.position, 0)) {
		//	inSun = false;
		//} else {
			inSun = true;
		//}
	}

	public void CheckFertility() {
		/*fertility -= fertilityConsumption * organismCount;
		if (fertility > .3f * organismCount) {
			growth += fertility + .5f / organismCount;
		} else if (fertility < - .3f * organismCount) {
			growth += fertility - .5f / organismCount;
		}
		fertility = 0;

		if (growth <= 0) {
			growth = 0;
			organismCount -= Mathf.RoundToInt (fertility / 2);
		}
		if (organismCount <= 0) {
			Destroy(gameObject);
		}*/
		fertility -= fertilityConsumption * age;
		if (fertility > 0) {
			storedGrowth += fertility * earth.GetComponent<EarthScript>().humidity / 100;
		} else {
			storedGrowth -= fertility / earth.GetComponent<EarthScript>().humidity / 100;
		}
		fertility = 0;
		if (storedGrowth <= 0) {
			storedGrowth = 0;
			if (Random.Range(0, 4) == 0) {
				plantSpecies.GetComponent<PlantSpeciesScript>().plantCount--;
				organismCount--;
				storedGrowth += .5f;
			}
			if (organismCount == 0) {
				Destroy(gameObject);
			}
		}
	}
}
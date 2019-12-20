using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushScript : MonoBehaviour {


	void Start () {
		
	}
	
	void Update () {
		
	}

	void MakeFLower () {
		GetComponent<BasicPlantScript>().plantSpecies.gameObject.GetComponent<PlantSpeciesFlowerSeed>();
	}
}

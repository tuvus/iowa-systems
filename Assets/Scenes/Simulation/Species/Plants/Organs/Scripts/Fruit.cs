using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour {
	public GameObject parent;

	private GameObject earth;

	public int growTimeMax;
	public float dispersionRange;

	private void Start() {
		//earth = GameObject.Find("earth");
	}
	//void FixedUpdate() {
	//	if ((earth.GetComponent<EarthScript>().time == 0) && ( growTimeMax != 0)) {
	//		growTimeMax--;
	//	}
	//	if (growTimeMax == 0) {
	//		if (Random.Range (0,4) == 0) {
	//			MakeNewPlant();
	//		}
	//	}
	//}

	public void MakeNewPlant () {
		//GameObject.Find(parent.GetComponent<BasicPlantScript>().species).GetComponent<PlantSpeciesScript>().SpawnOrganism(parent, dispersionRange);
	}
}

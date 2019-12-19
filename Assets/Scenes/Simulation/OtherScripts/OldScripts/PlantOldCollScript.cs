using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantCollScript : MonoBehaviour {

	void OnTriggerStay(Collider other) {
		if (other.gameObject.layer == 11) {
			//GetComponentInParent<BasicPlantScript>().FindStartPos();
			Debug.Log("reset");
		}
	}
}

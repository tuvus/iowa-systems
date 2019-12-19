using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesMouth : MonoBehaviour {

	public GameObject mouth;

	public float biteSize;
	public float eatTime;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newMouth = Instantiate(mouth, _newOrganism.transform);
		MouthScript plantMouth = newMouth.GetComponent<MouthScript>();
		plantMouth.biteSize = biteSize;
		plantMouth.eatTime = eatTime;
	}
}

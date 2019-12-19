using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomizer : MonoBehaviour {

	public float distance;
	public float range;
	public GameObject parent;

	void Start () {
		transform.position = new Vector3(0, 0, 0);
		if (parent != null) {
			transform.LookAt(parent.transform);
		}
		//Debug.Log(range);
		if (range == 0f) {
			transform.Rotate(new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)));
		} else {
			transform.Rotate(new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)));

		}
		if (distance == 0f) {
			transform.Translate(transform.forward * 249.2f);
		} else {
			transform.position = new Vector3(0, 0, 0);
			transform.Translate(transform.forward * distance);
		}
		Destroy(this);
	}
}
    
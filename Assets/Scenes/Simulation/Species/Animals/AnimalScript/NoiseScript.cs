using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseScript : MonoBehaviour {

	public float range;
	public string type;
	public float time;
	void Start() {

	}

	void Update () {
		if (time < 0) {
			Destroy(gameObject);
		} else {
			time -= Time.fixedDeltaTime;
		}
	}
}
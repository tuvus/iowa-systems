using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomizer {

	public void SpawnRandom (Transform organism, EarthScript earth) {
		float distance = earth.transform.localScale.x / 2;
		organism.position = new Vector3(0, 0, 0);
		organism.Rotate(new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)));
			organism.Translate(organism.forward * distance);
	}
	public void SpawnFromParent(Transform organism,GameObject parent, float range, EarthScript earth) {
		float distance = earth.transform.localScale.x / 2;

		organism.position = new Vector3(0, 0, 0);
		organism.LookAt(parent.transform);
		organism.Rotate(organism.right * 90);
		organism.Rotate(new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)));
		organism.Translate(organism.forward * distance);
	}
}
    
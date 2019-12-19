using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantFood : MonoBehaviour {

	public string foodType;
	public float foodCount;
	public float foodGain;
	public float eatNoiseRange;
	public float deteriationTime;
	private void Start() {
		deteriationTime = 100;
	}

	void Update () {
		deteriationTime -= Time.deltaTime;
		if (deteriationTime < 0) {
			Destroy(gameObject);
		}
		if (foodCount <= 0) {
			Destroy(gameObject);
		}
	}
}

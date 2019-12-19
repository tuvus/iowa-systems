using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthScript : MonoBehaviour {

	public float worldTime;
	public int maxTime;
	public int time;
	public float tempeture;
	public float humidity;

	void FixedUpdate() {
		worldTime++;
		time --;
		if (time == -1) {
			time = maxTime;
		}
		if (Random.Range (0,2) == 0) {
			humidity += Random.Range(-2.4444f, 2.4444f);
			if (humidity < 0) {
				humidity = 0;
			}
			if (humidity > 100) {
				humidity = 100;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour {

	public float sunSpeed;

	void Start () {
		
	}
	
	void FixedUpdate () {
		transform.Rotate(0, sunSpeed, 0);
		transform.position = new Vector3(0, 0, 0);
		transform.Translate(transform.forward * -1000, Space.World);
	}
}

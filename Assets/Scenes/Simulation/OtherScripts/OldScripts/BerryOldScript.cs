using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryScript : MonoBehaviour {

	public float growRange;
	

	void Start () {
		SetPosition();
	}
	
	void Update () {
		
	}
	void SetPosition() {
		transform.Rotate(new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)));
		transform.Translate(transform.forward * growRange, Space.World);
	}
	private void OnTriggerStay(Collider coll) {
		if (coll.gameObject.layer == LayerMask.GetMask("Earth")){
			SetPosition();
		}
	}
}

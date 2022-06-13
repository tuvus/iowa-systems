using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTestScript : MonoBehaviour {
    public Vector3 transformation = new Vector3();
    public float radius;

    public void Start() {

        transform.position = new Vector3(Random.Range(-1,1), Random.Range(-1, 1), Random.Range(-1, 1)) * radius;
    }

    public void Update() {
        //transform.rotation = Quaternion.(-transform.position, transformation);
    }
}

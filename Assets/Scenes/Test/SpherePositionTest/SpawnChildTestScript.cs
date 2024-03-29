using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChildTestScript : MonoBehaviour {
    public void SpawnChild() {
        Transform newChild = Instantiate(GetTestScript1().GetRotationTransform().gameObject, null).transform;
        newChild.GetChild(0).GetComponent<MovementTestScript>().child = true;
        newChild.GetChild(0).GetComponent<MovementTestScript>().SetupChildLocation(GetTestScript1().GetRotationTransform().localPosition, 10);

    }

    MovementTestScript GetTestScript1() {
        return GetComponent<MovementTestScript>();
    }
}

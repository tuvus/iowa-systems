using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A simple script to test the movetoPoint function on the old movement system.
/// </summary>
public class OldPositionTestScript : MonoBehaviour {
    public Transform target;
    public float radius;
    public float moveSpeed;
    public float moveRange;

    private void Start() {
        GetRotationTransform().localPosition = new Vector3(0, .5f, 0);
    }

    public void RotateFromCenter(Vector3 rotationAmmounts) {
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, rotationAmmounts.x);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().up, rotationAmmounts.y);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().forward, rotationAmmounts.z);
    }

    public void Update() {
        GoToPoint(target.position);
    }

    public void MoveForward(float arcDistance) {
        float degreesToRotate = Mathf.Rad2Deg * (arcDistance / radius);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, degreesToRotate);
    }

    public void GoToPoint(Vector3 position) {
        LookAtPoint(position);
        float dist = Vector3.Distance(GetModelTransform().position, position);
        float arcDist = 2 * radius * math.asin(dist / (2 * radius)) - (moveRange / 2);
        MoveForward(math.max(math.min(arcDist, moveSpeed * Time.deltaTime), - moveSpeed * Time.deltaTime));

    }

    public void LookAwayFromPoint(Vector3 point) {
        if (Vector3.Distance(point, GetModelTransform().position) > 0) {
            GetModelTransform().rotation = Quaternion.LookRotation(GetModelTransform().position - point);
        }
        AlignOrganism();
    }

    public void LookAtPoint(Vector3 point) {
        if (Vector3.Distance(point, GetModelTransform().position) > 0) {
            GetModelTransform().rotation = Quaternion.LookRotation(point - GetModelTransform().position);
        }
        AlignOrganism();
    }

    void AlignOrganism() {
        GetModelTransform().localEulerAngles = new Vector3(0, GetModelTransform().localEulerAngles.y, 0);
    }


    public void SetLookRotation(float degrees) {
        GetModelTransform().localEulerAngles = new Vector3(0, degrees, 0);
    }

    public Transform GetModelTransform() {
        return transform;
    }

    public Transform GetRotationTransform() {
        return transform.parent;
    }
}

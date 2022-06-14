using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A test on the new movment system. A new movement system is nessesary for position calculations to work on multiple threads.
/// </summary>
public class NewPositionTestScript : MonoBehaviour {
    public Transform target;
    public float radius;
    public bool moving;
    public float moveSpeed;

    public void Start() {
        GetRotationTransform().localPosition = new Vector3(0, .5f, 0);
        //RotateFromCenter(new Vector3(GetRandomNumber(), GetRandomNumber(), GetRandomNumber()));
        SetLookRotation(GetRandomNumber());
    }

    public void Update() {
        if (moving)
        MovetoPoint(target.transform.position);
        //transform.rotation = Quaternion.(-transform.position, transformation);
    }

    public void MoveOrganism() {
        if (moving) {
            GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, moveSpeed * Time.deltaTime);
        }
    }

    public void MovetoPoint(Vector3 point) {
        float distance = Vector3.Distance(transform.position, point);
        float arcDistance = 2 * radius * math.asin(distance / (2 * radius));
        Vector3 middlePoint = Vector3.MoveTowards(transform.position, point, math.min(arcDistance, moveSpeed * Time.deltaTime));
        //float radius = Vector3.Distance(Vector3.zero, middlePoint);
        //Vector3 targetPoint = middlePoint * (radius / this.radius);
        Vector3 targetPointNorm = middlePoint.normalized;
        targetPointNorm /= 2;
        GetRotationTransform().localPosition = targetPointNorm;
        LookAtPoint(point);
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

    public void RotateFromCenter(Vector3 rotationAmmounts) {
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, rotationAmmounts.x);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().up, rotationAmmounts.y);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().forward, rotationAmmounts.z);
    }

    public void SetLookRotation(float degrees) {
        GetModelTransform().localEulerAngles = new Vector3(0, degrees, 0);
    }

    public void TurnReletive(float turn) {
        GetModelTransform().Rotate(Vector3.up * turn);
    }

    void AlignOrganism() {
        GetModelTransform().localEulerAngles = new Vector3(0, GetModelTransform().localEulerAngles.y, 0);
    }

    static float GetRandomNumber(float range = 360) {
        return UnityEngine.Random.Range(-range * 1000, range * 1000) / 1000;
    }

    public Transform GetModelTransform() {
        return transform;
    }

    public Transform GetRotationTransform() {
        return transform.parent;
    }

}

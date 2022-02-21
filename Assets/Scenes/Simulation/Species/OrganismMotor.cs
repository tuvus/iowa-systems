using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganismMotor : MonoBehaviour {
    internal EarthScript earth;
    internal BasicOrganismScript organismScript;

    public virtual void SetupOrganismMotor (EarthScript earth, BasicOrganismScript organismScript) {
        this.earth = earth;
        this.organismScript = organismScript;
        transform.parent.SetParent(earth.GetOrganismsTransform());
        Vector3 previousSize = transform.parent.localScale;
        transform.parent.localScale = new Vector3(1, 1, 1);
        transform.parent.localPosition = new Vector3(0, .5f, 0);
        transform.localScale = new Vector3(transform.localScale.x * previousSize.x, transform.localScale.y * previousSize.y, transform.localScale.z * previousSize.z);
    }

    public void SetPositionToParent(Vector3 parentPosition, Vector3 parentRotation) {
        GetRotationTransform().position = parentPosition;
        GetRotationTransform().eulerAngles = parentRotation;
    }


    public void RotateFromCenter(Vector3 rotationAmmounts) {
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, rotationAmmounts.x);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().up, rotationAmmounts.y);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().forward, rotationAmmounts.z);
        organismScript.position = GetModelTransform().position;
        organismScript.RefreshOrganism();
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

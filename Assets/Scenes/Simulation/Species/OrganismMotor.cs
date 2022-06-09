using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganismMotor : MonoBehaviour {
    private Organism organism;

    public void SetupOrganismMotor (Organism organism) {
        this.organism = organism;
        transform.parent.SetParent(organism.GetEarthScript().GetOrganismsTransform());
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
        organism.position = GetModelTransform().position;
        organism.RefreshOrganism();
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

    public Organism GetOrganism() {
        return organism;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganismMotor : MonoBehaviour {
    private Organism organism;

    public void SetupOrganismMotor (Organism organism) {
        this.organism = organism;
        transform.parent.SetParent(organism.GetEarthScript().GetOrganismsTransform());
        Vector3 previousSize = transform.parent.localScale;
        GetRotationTransform().localScale = new Vector3(1, 1, 1);
        GetRotationTransform().localPosition = new Vector3(0, .5f, 0);
        GetModelTransform().localScale = new Vector3(transform.localScale.x * previousSize.x, transform.localScale.y * previousSize.y, transform.localScale.z * previousSize.z);
    }

    /// <summary>
    /// Sets the position and rotation of this organism to the parents position and rotation
    /// </summary>
    /// <param name="organismMotor">The organismMotor attached to the parent</param>
    public void SetPositionToParent(OrganismMotor organismMotor) {
        GetRotationTransform().position = organismMotor.GetRotationTransform().position;
        GetRotationTransform().eulerAngles = organismMotor.GetRotationTransform().eulerAngles;
    }

    /// <summary>
    /// Rotates the organism around the origin on all three axis.
    /// Refreshes the organism afterwards.
    /// </summary>
    /// <param name="rotationAmmounts">The Vector3 ammount that should be rotated.</param>
    public void RotateFromCenter(Vector3 rotationAmmounts) {
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, rotationAmmounts.x);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().up, rotationAmmounts.y);
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().forward, rotationAmmounts.z);
        organism.position = GetModelTransform().position;
        organism.RefreshOrganism();
    }

    /// <summary>
    /// Sets the Rotation of the organism to the value degrees.
    /// Look Rotation is the axis tangential to the earth at all times.
    /// </summary>
    /// <param name="degrees"></param>
    public void SetModelRotation(float degrees) {
        GetModelTransform().localEulerAngles = new Vector3(0, degrees, 0);
    }

    /// <summary>
    /// Aligns model of the organism so that it's Y rotation is isolated to allow it to be pointed.
    /// Does not actually poin the organism with it's belly towards the center. That should have already been the case with rotating around the center.
    /// </summary>
    public void AlignOrganismModel() {
        GetModelTransform().localEulerAngles = new Vector3(0, GetModelTransform().localEulerAngles.y, 0);
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

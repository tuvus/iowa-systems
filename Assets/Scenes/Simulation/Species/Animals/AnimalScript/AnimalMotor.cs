using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMotor : OrganismMotor {
    AnimalScript animal;

    bool moving;
    float frameMoveSpeed;

    public override void SetupOrganismMotor(EarthScript earth, BasicOrganismScript organismScript) {
        this.earth = earth;
        this.organismScript = organismScript;
        this.animal = organismScript.GetComponent<AnimalScript>();
        transform.parent.SetParent(earth.GetOrganismsTransform());
        Vector3 previousSize = transform.parent.localScale;
        transform.parent.localScale = new Vector3(1, 1, 1);
        transform.parent.localPosition = new Vector3(0, .5f, 0);
        transform.localScale = new Vector3(transform.localScale.x * previousSize.x, transform.localScale.y * previousSize.y, transform.localScale.z * previousSize.z);
    }

    public void SetSpeed(float _speed) {
        frameMoveSpeed = _speed;
    }

    public void SetMoving(bool _moving) {
        moving = _moving;
    }

    public bool GetMoving() {
        return moving;
    }

    public void MoveOrganism() {
        if (moving) {
            GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, frameMoveSpeed * earth.simulationDeltaTime / 12);
            animal.RefreshOrganism();
            animal.animalSpecies.AddToFindZone(animal, animal.zone, frameMoveSpeed * earth.simulationDeltaTime / 12);
        }
    }

    #region RotationControls
    public void LookAwayFromPoint(Vector3 _point) {
        if (Vector3.Distance(_point, GetModelTransform().position) > 0) {
            GetModelTransform().rotation = Quaternion.LookRotation(GetModelTransform().position - _point);
        }
        AlignOrganism();
    }

    public void LookAtPoint(Vector3 _point) {
        if (Vector3.Distance(_point, GetModelTransform().position) > 0) {
            GetModelTransform().rotation = Quaternion.LookRotation(_point - GetModelTransform().position);
        }
        AlignOrganism();
    }

    public void TurnReletive(float _turn) {
        GetModelTransform().Rotate(Vector3.up * _turn);
    }

    void AlignOrganism() {
        GetModelTransform().localEulerAngles = new Vector3(0, GetModelTransform().localEulerAngles.y, 0);
    }
    #endregion

}

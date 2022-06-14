using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AnimalMotor : OrganismMotor {
    float frameMoveSpeed;

    public void SetSpeed(float speed) {
        frameMoveSpeed = speed;
    }

    /// <summary>
    /// Moves the organism foward by the value of arcDistance.
    /// </summary>
    /// <param name="arcDistance">The distance to travel.</param>
    public void MoveForward(float arcDistance) {
        float degreesToRotate = Mathf.Rad2Deg * (arcDistance / GetAnimal().GetEarthScript().GetRadius());
        GetRotationTransform().RotateAround(new Vector3(0, 0, 0), GetModelTransform().right, degreesToRotate);
        GetAnimal().RefreshOrganism();
        GetAnimal().animalSpecies.AddToFindZone(GetAnimal(), GetAnimal().zone, frameMoveSpeed * GetOrganism().GetEarthScript().simulationDeltaTime);
    }

    /// <summary>
    /// Moves the animal towards the position at a minimum of distanceFromPosition units from it.
    /// The movement will not be greater than the speed parameter.
    /// If the animal is within the minimum distance it will move back to the minimum distance 
    /// </summary>
    /// <param name="position">The position to move to</param>
    /// <param name="maxArcDistance">The maximum ammount to move</param>
    /// <param name="distanceFromPosition">The minimum distance from the point</param>
    public void GoToPoint(Vector3 position, float maxArcDistance, float distanceFromPosition = 0f) {
        LookAtPoint(position);
        float radius = GetOrganism().GetEarthScript().GetRadius();
        float directDistance = Vector3.Distance(GetModelTransform().position, position);
        float arcDistDistance = 2 * radius * math.asin(directDistance / (2 * radius) - distanceFromPosition);
        MoveForward(math.max(math.min(arcDistDistance, maxArcDistance), -maxArcDistance));
    }

    /// <summary>
    /// Moves the animal away from the position by arcDistance.
    /// </summary>
    /// <param name="position">The position to move from</param>
    /// <param name="arcDistance">The distance to move away</param>
    public void MoveAwayFromPoint(Vector3 position, float arcDistance) {
        LookAwayFromPoint(position);
        MoveForward(arcDistance);
    }

    #region RotationControls
    /// <summary>
    /// Points the organism's Model towards the target Point while still flat to the earth.
    /// </summary>
    /// <param name="point">The point to look at</param>
    public void LookAtPoint(Vector3 point) {
        if (Vector3.Distance(point, GetModelTransform().position) > 0) {
            GetModelTransform().rotation = Quaternion.LookRotation(point - GetModelTransform().position);
        }
        AlignOrganismModel();
    }

    /// <summary>
    /// Points the organism's Model away from the target Point while still flat to the earth.
    /// </summary>
    /// <param name="point">The point to look away from</param>
    public void LookAwayFromPoint(Vector3 point) {
        if (Vector3.Distance(point, GetModelTransform().position) > 0) {
            GetModelTransform().rotation = Quaternion.LookRotation(GetModelTransform().position - point);
        }
        AlignOrganismModel();
    }

    /// <summary>
    /// Turns the model of the organism.
    /// </summary>
    /// <param name="turnDegrees"></param>
    public void TurnModel(float turnDegrees) {
        GetModelTransform().Rotate(Vector3.up * turnDegrees);
    }
    #endregion

    public Animal GetAnimal() {
        return (Animal)GetOrganism();
    }
}

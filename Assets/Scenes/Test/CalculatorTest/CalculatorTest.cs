using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CalculatorTest : MonoBehaviour {
    public Transform earth;
    public Transform controlOrganism;
    public Transform newOrganism;

    public void Awake() {
        Vector3 previousSize = controlOrganism.parent.localScale;
        controlOrganism.parent.localScale = Vector3.one;
        controlOrganism.parent.localPosition = new Vector3(0, 0.5f, 0);
        controlOrganism.localScale = previousSize;
        //Testing calculator
        //print(GetArcDistanceBetweenTwoPositions(new float3(1,1,1), new float3(-1,-1,-1)));
        //print(math.distance(new float3(1, 1, 1), new float3(-1, -1, -1)));
        float3 rotation = float3.zero;
        SetRotationControlAroundCenter(rotation);
        newOrganism.position = GetPositionFromRotation(rotation);
        print(controlOrganism.transform.position.Equals(newOrganism.position));
        print(controlOrganism.transform.position + " " + newOrganism.position);

        rotation = new float3(0, 1 * math.PI, 0);
        SetRotationControlAroundCenter(rotation);
        newOrganism.position = GetPositionFromRotation(rotation);
        print(controlOrganism.transform.position.Equals(newOrganism.position));
        print(controlOrganism.transform.position + " " + newOrganism.position);

        rotation = new float3(0, 0, 1 * math.PI);
        SetRotationControlAroundCenter(rotation);
        newOrganism.position = GetPositionFromRotation(rotation);
        print(controlOrganism.transform.position.Equals(newOrganism.position));
        print(controlOrganism.transform.position + " " + newOrganism.position);
    }

    public Vector3 rotation = Vector3.zero;

    private void Update() {
        SetRotationControlAroundCenter(rotation);
        newOrganism.position = GetPositionFromRotation(rotation);
    }

    public void SetRotationControlAroundCenter(float3 rotationAmmount) {
        controlOrganism.parent.localPosition = new Vector3(0, 0.5f, 0);
        controlOrganism.parent.eulerAngles = Vector3.zero;
        controlOrganism.parent.RotateAround(new Vector3(0, 0, 0), controlOrganism.right, rotationAmmount.x);
        controlOrganism.parent.RotateAround(new Vector3(0, 0, 0), controlOrganism.up, rotationAmmount.y);
        controlOrganism.parent.RotateAround(new Vector3(0, 0, 0), controlOrganism.forward, rotationAmmount.z);
        //controlOrganism.parent.RotateAround(new Vector3(0, 0, 0), controlOrganism.right, math.degrees(rotationAmmount.x));
        //controlOrganism.parent.RotateAround(new Vector3(0, 0, 0), controlOrganism.up, math.degrees(rotationAmmount.y));
        //controlOrganism.parent.RotateAround(new Vector3(0, 0, 0), controlOrganism.forward, math.degrees(rotationAmmount.z));
        controlOrganism.parent.LookAt(Vector3.zero);
    }

    public float GetRadius() {
        return earth.localScale.x / 2;
    }

    public float GetCircumference() {
        return 2 * math.PI * GetRadius();
    }


    public float GetSurfaceArea() {
        return 4 * math.PI * math.pow(GetRadius(), 2);
    }


    public float3 GetPositionFromRotation(float3 rPosition) {
        return new float3(GetRadius() * math.sin(rPosition.x) * math.cos(rPosition.x), GetRadius() * math.sin(rPosition.y) * math.sin(rPosition.y), GetRadius() * math.cos(rPosition.z));
    }

    public float GetArcDistanceBetweenTwoPositions(float3 position, float3 target) {
        return (1 / math.sin(math.distance(position, target) / (2 * GetRadius()))) * GetRadius();
    }

    public float GetSurfaceAreaWithinDistance(float distance) {
        return 0;
    }

    public float3 GetRandomPosition() {
        return 0;
    }

    public float3 GetFlatRotation(float3 position, float rotation) {
        return 0;
    }

    public float3x2 GetMovePosition(float3 position, float rotaiton, float distance) {
        return 0;
    }

    public float GetLookFlatRotation(float3 position, float3 lookPosition) {
        return 0;
    }

    public float3 GetOffsetPosition(float3 postion, float rotaiton, float3 localPosition) {
        return 0;
    }
}
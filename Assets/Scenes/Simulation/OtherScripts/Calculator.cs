using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Calculator : MonoBehaviour {

    public void Awake() {
        //Testing calculator
        //print(GetArcDistanceBetweenTwoPositions(new float3(1,1,1), new float3(-1,-1,-1)));
        //print(math.distance(new float3(1, 1, 1), new float3(-1, -1, -1)));
    }

    public static float GetRadius() {
        return 1;
    }

    public static float GetCircumference() {
        return 2 * math.PI * GetRadius();
    }


    public static float GetSurfaceArea() {
        return 4 * math.PI * math.pow(GetRadius(), 2);
    }

    public static float3 GetPositionFromRotation(float3 rPosition) {
        return 0;
    }

    public static float GetArcDistanceBetweenTwoPositions(float3 position, float3 target) {
        return (1 / math.sin(math.distance(position, target) / (2 * GetRadius()))) * GetRadius();
    }

    public static float GetSurfaceAreaWithinDistance(float distance) {
        return 0;
    }

    public static float3 GetRandomPosition() {
        return 0;
    }

    public static float3 GetFlatRotation(float3 position, float rotation) {
        return 0;
    }

    public static float3x2 GetMovePosition(float3 position, float rotaiton, float distance) {
        return 0;
    }

    public static float GetLookFlatRotation(float3 position, float3 lookPosition) {
        return 0;
    }

    public static float3 GetOffsetPosition(float3 postion, float rotaiton, float3 localPosition) {
        return 0;
    }
}
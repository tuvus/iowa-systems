using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;


public class ZoneControllerTestScript : MonoBehaviour {
    public GameObject zonePrefab;

    public int numberOfZones;
    NativeArray<float3> zonePositions;
    NativeParallelMultiHashMap<int, int> neiboringZones;
    public int maxNeibroingZones;

    internal JobHandle job;

    void Start() {
        SpawnZones(GetRadius());
    }

    void SpawnZones(float radius) {
        Allocate();
        float phi = Mathf.PI * (3 - Mathf.Sqrt(5));
        for (int i = 0; i < numberOfZones; i++) {
            float yPosition = 1 - (i / (float)(numberOfZones - 1)) * 2;
            float tempRadius = Mathf.Sqrt(1 - yPosition * yPosition);

            float theta = phi * i;
            float xPosition = Mathf.Cos(theta) * tempRadius;
            float zPosition = Mathf.Sin(theta) * tempRadius;
            float3 newZonePosition = new float3(new Vector3(xPosition, yPosition, zPosition) * radius);
            zonePositions[i] = newZonePosition;
        }
        //float distance = 4 * Mathf.PI * Mathf.Pow(radius, 2) / (numberOfZones * 10);
        float distance = 350 / Mathf.Sqrt(numberOfZones);
        JobHandle neiboringZonesJob = FindNeiboringZones(distance);
        neiboringZonesJob.Complete();
        for (int i = 0; i < zonePositions.Length; i++) {
            CreateZonePrefab(zonePositions[i]);
        }
    }

    void Allocate() {
        zonePositions = new NativeArray<float3>(numberOfZones, Allocator.Persistent);
        neiboringZones = new NativeParallelMultiHashMap<int, int>(numberOfZones * maxNeibroingZones, Allocator.Persistent);
    }

    public JobHandle FindNeiboringZones(float distance) {
        job = ZoneUpdateJobTest.BeginJob(zonePositions,neiboringZones.AsParallelWriter(), maxNeibroingZones, distance);
        return job;
    }

    public void CreateZonePrefab(Vector3 position) {
        GameObject newZonePrefab = Instantiate(zonePrefab, position, quaternion.identity, transform);
    }

    private void OnDestroy() {
        neiboringZones.Dispose();
        zonePositions.Dispose();
    }

    float GetRadius() {
        return transform.parent.lossyScale.x / 2;
    }
}

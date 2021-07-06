using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomizer {

	public void SpawnRandom (OrganismMotor organismMotor) {
		organismMotor.RotateFromCenter(new Vector3(GetRandomNumber(),GetRandomNumber(),GetRandomNumber()));
		organismMotor.SetLookRotation(GetRandomNumber());
	}

	public void SpawnFromParent(OrganismMotor organismMotor, float range) {
		organismMotor.RotateFromCenter(new Vector3(GetRandomNumber(range), GetRandomNumber(range), GetRandomNumber(range)));
		organismMotor.SetLookRotation(GetRandomNumber());
	}

	float GetRandomNumber(float range = 360) {
		return Random.Range(-range * 1000, range * 1000) / 1000;
	}
}
    
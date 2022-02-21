using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnRandomizer {

	public static void SpawnRandom (OrganismMotor organismMotor) {
		organismMotor.RotateFromCenter(new Vector3(GetRandomNumber(),GetRandomNumber(),GetRandomNumber()));
		organismMotor.SetLookRotation(GetRandomNumber());
	}

	public static void SpawnFromParent(OrganismMotor organismMotor, Vector3 parentPosition, Vector3 parentRotaiton, float range) {
		organismMotor.SetPositionToParent(parentPosition, parentRotaiton);
		organismMotor.RotateFromCenter(new Vector3(GetRandomNumber(range), GetRandomNumber(range), GetRandomNumber(range)));
		organismMotor.SetLookRotation(GetRandomNumber());
	}

	static float GetRandomNumber(float range = 360) {
		return Random.Range(-range * 1000, range * 1000) / 1000;
	}
}
    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnRandomizer {

	public static void SpawnRandom (Species.Organism organismData) {
		organismMotor.RotateFromCenter(new Vector3(GetRandomNumber(),GetRandomNumber(),GetRandomNumber()));
		organismMotor.SetModelRotation(GetRandomNumber());
	}

	public static void SpawnFromParent(OrganismMotor organismMotor, OrganismMotor parentOrganismMotor, float range) {
		organismMotor.SetPositionToParent(parentOrganismMotor);
		organismMotor.RotateFromCenter(new Vector3(GetRandomNumber(range), GetRandomNumber(range), GetRandomNumber(range)));
		organismMotor.SetModelRotation(GetRandomNumber());
	}

	static float GetRandomNumber(float range = 360) {
		return Simulation.randomGenerator.NextFloat(-range,range);
	}
}
    
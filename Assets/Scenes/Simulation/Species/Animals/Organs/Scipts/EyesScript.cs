using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesScript : BasicAnimalOrganScript {
	public AnimalSpeciesEyes speciesEyes;
	public enum EyeTypes {
		Foward = 0,
		Side = 1,
    }


	public float sightRange;

	internal override void SetUpSpecificOrgan() {
		SetUpEyes();
	}

	void FixedUpdate() {
	}
	void OnTriggerEnter(Collider trigg) {
		if (trigg.gameObject.layer != 8 && trigg.gameObject.layer != 10) {
			basicAnimalScript.nearbyObjects.Add(trigg.gameObject);
		}
	}
	void OnTriggerExit(Collider trigg) {
		if (trigg.gameObject.layer != 8 && trigg.gameObject.layer != 10) {
			basicAnimalScript.nearbyObjects.Remove(trigg.gameObject);
		}
	}

	public void SetUpEyes() {
		if (speciesEyes.eyeType == EyeTypes.Foward) {
			SphereCollider eye = gameObject.AddComponent<SphereCollider>();
			eye.isTrigger = true;
			eye.radius = sightRange / 2;
			eye.center = new Vector3(sightRange / 2, 0, 0);
			return;
		}
		if (speciesEyes.eyeType == EyeTypes.Side) {
			SphereCollider leftEye = gameObject.AddComponent<SphereCollider>();
			SphereCollider rightEye = gameObject.AddComponent<SphereCollider>();
			leftEye.isTrigger = true;
			rightEye.isTrigger = true;
			leftEye.radius = sightRange / 2;
			leftEye.center = new Vector3(0, sightRange / 1.2f, 0);
			rightEye.radius = sightRange / 2;
			rightEye.center = new Vector3(0, -sightRange / 1.2f, 0);
		}
	}
}
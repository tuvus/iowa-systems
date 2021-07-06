using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthScript : BasicAnimalOrganScript {

	public bool eating;
	public float biteSize;
	public float eatRange;
	public float eatTime;

	public Transform mouth;

	internal override void SetUpSpecificOrgan() {
		basicAnimalScript.SetMouth(this);
		SetUpMouth();
	}

	public void SetUpMouth() {
		mouth = new GameObject("Mouth").transform;
		mouth.SetParent(basicAnimalScript.GetAnimalMotor().GetModelTransform());
		mouth.localScale = Vector3.one;
		mouth.localEulerAngles = Vector3.zero;
		mouth.localPosition = new Vector3(0, 0, eatRange / 2f);
	}


    public override void UpdateOrgan() {

	}

	public List<Eddible> GetEddibleObjectsInRange(List<Eddible> eddibleObjects) {
		List<Eddible> eddibleObjectsInRange = new List<Eddible>();
		foreach (var eddibleObject in eddibleObjects) {
			if (WithinRange(eddibleObject.GetPosition(), eatRange))
                eddibleObjectsInRange.Add(eddibleObject);
        }
		return eddibleObjectsInRange;
	}

	bool WithinRange(Vector3 position, float _range) {
        if (Vector3.Distance(position, mouth.position) <= _range / 2)
            return true;
        return false;
	}
}

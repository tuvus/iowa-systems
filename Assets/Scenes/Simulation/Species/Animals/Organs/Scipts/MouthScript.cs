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
		SetUpMouth();
	}

	public void SetUpMouth() {
		mouth = new GameObject("Mouth").transform;
		mouth.SetParent(animalScript.GetAnimalMotor().GetModelTransform());
		mouth.localScale = Vector3.one;
		mouth.localEulerAngles = Vector3.zero;
		mouth.localPosition = new Vector3(0, 0, eatRange / 2f / animalScript.GetAnimalMotor().GetModelTransform().lossyScale.z);
	}


    public override void UpdateOrgan() {

	}

    public override void ResetOrgan() {
    }
}

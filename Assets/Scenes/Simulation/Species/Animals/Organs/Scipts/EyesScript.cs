using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesScript : BasicAnimalOrganScript {
	public AnimalSpeciesEyes speciesEyes;

	public List<Transform> eyes = new List<Transform>();

	public enum EyeTypes {
		Foward = 0,
		Side = 1,
    }

	internal override void SetUpSpecificOrgan() {
		SetUpEyes();
	}

	public void SetUpEyes() {
		if (speciesEyes.eyeType == EyeTypes.Foward) {
			Transform newEye = new GameObject("Eyes").transform;
			newEye.SetParent(animalScript.GetAnimalMotor().GetModelTransform());
			newEye.localScale = Vector3.one;
			newEye.localEulerAngles = Vector3.zero; 
			newEye.localPosition = new Vector3(0, 0, speciesEyes.sightRange / 2 / animalScript.GetAnimalMotor().GetModelTransform().lossyScale.z);
			eyes.Add(newEye);
			return;
		}
		if (speciesEyes.eyeType == EyeTypes.Side) {
			Transform newLeftEye = new GameObject("LeftEye").transform;
			newLeftEye.SetParent(animalScript.GetAnimalMotor().GetModelTransform());
			newLeftEye.localScale = Vector3.one;
			newLeftEye.localEulerAngles = Vector3.zero; 
			newLeftEye.localPosition = new Vector3(-speciesEyes.sightRange / 2 / animalScript.GetAnimalMotor().GetModelTransform().lossyScale.x, 0, 0);
			eyes.Add(newLeftEye);


			Transform newRightEye = new GameObject("RightEye").transform;
			newRightEye.SetParent(animalScript.GetAnimalMotor().GetModelTransform());
			newRightEye.localScale = Vector3.one;
			newRightEye.localEulerAngles = Vector3.zero; 
			newRightEye.localPosition = new Vector3(speciesEyes.sightRange / 2 / animalScript.GetAnimalMotor().GetModelTransform().lossyScale.x, 0, 0);
			eyes.Add(newRightEye);
		}
	}

    public override void UpdateOrgan() {
    }

	public EyeTypes GetEyeType() {
		return speciesEyes.eyeType;
    }

    public override void ResetOrgan() {
    }
}
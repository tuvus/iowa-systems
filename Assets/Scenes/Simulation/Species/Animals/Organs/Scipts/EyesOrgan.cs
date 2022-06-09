using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesOrgan : AnimalOrgan {
	public List<Transform> eyes { private set; get; }

	public enum EyeTypes {
		Foward = 0,
		Side = 1,
    }

	public void SetupOrgan(AnimalSpeciesEyes speciesEyes, Animal animal) {
		base.SetupOrgan(speciesEyes, animal);
		eyes = new List<Transform>();

		if (speciesEyes.eyeType == EyeTypes.Foward) {
			Transform newEye = new GameObject("Eyes").transform;
			newEye.SetParent(GetAnimal().GetAnimalMotor().GetModelTransform());
			newEye.localScale = Vector3.one;
			newEye.localEulerAngles = Vector3.zero;
			newEye.localPosition = new Vector3(0, 0, speciesEyes.sightRange / 2 / GetAnimal().GetAnimalMotor().GetModelTransform().lossyScale.z);
			eyes.Add(newEye);
			return;
		}
		if (speciesEyes.eyeType == EyeTypes.Side) {
			Transform newLeftEye = new GameObject("LeftEye").transform;
			newLeftEye.SetParent(GetAnimal().GetAnimalMotor().GetModelTransform());
			newLeftEye.localScale = Vector3.one;
			newLeftEye.localEulerAngles = Vector3.zero;
			newLeftEye.localPosition = new Vector3(-speciesEyes.sightRange / 2 / GetAnimal().GetAnimalMotor().GetModelTransform().lossyScale.x, 0, 0);
			eyes.Add(newLeftEye);


			Transform newRightEye = new GameObject("RightEye").transform;
			newRightEye.SetParent(GetAnimal().GetAnimalMotor().GetModelTransform());
			newRightEye.localScale = Vector3.one;
			newRightEye.localEulerAngles = Vector3.zero;
			newRightEye.localPosition = new Vector3(speciesEyes.sightRange / 2 / GetAnimal().GetAnimalMotor().GetModelTransform().lossyScale.x, 0, 0);
			eyes.Add(newRightEye);
		}
	}

    public override void UpdateOrgan() {
    }

	public EyeTypes GetEyeType() {
		return GetAnimalSpeciesEyes().eyeType;
    }

    public override void ResetOrgan() {
    }

	public List<Transform> GetEyes() {
		return eyes;
    }

	public AnimalSpeciesEyes GetAnimalSpeciesEyes() {
		return (AnimalSpeciesEyes)GetAnimalSpeciesOrgan();
    }
}
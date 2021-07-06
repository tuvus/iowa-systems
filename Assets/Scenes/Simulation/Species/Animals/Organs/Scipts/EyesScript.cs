using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 
	EyesScript : BasicAnimalOrganScript {
	public AnimalSpeciesEyes speciesEyes;

	public List<Transform> eyes = new List<Transform>();

	public enum EyeTypes {
		Foward = 0,
		Side = 1,
    }

	public float sightRange;

	internal override void SetUpSpecificOrgan() {
		basicAnimalScript.SetEyes(this);
		SetUpEyes();
	}

	public void SetUpEyes() {
		if (speciesEyes.eyeType == EyeTypes.Foward) {
			Transform newEye = new GameObject("Eyes").transform;
			newEye.SetParent(basicAnimalScript.GetAnimalMotor().GetModelTransform());
			newEye.localScale = Vector3.one;
			newEye.localEulerAngles = Vector3.zero; 
			newEye.localPosition = new Vector3(0, 0, sightRange / 2);
			eyes.Add(newEye);
			return;
		}
		if (speciesEyes.eyeType == EyeTypes.Side) {
			Transform newLeftEye = new GameObject("LeftEye").transform;
			newLeftEye.SetParent(basicAnimalScript.GetAnimalMotor().GetModelTransform());
			newLeftEye.localScale = Vector3.one;
			newLeftEye.localEulerAngles = Vector3.zero; 
			newLeftEye.localPosition = new Vector3(-sightRange / 2, 0, 0);
			eyes.Add(newLeftEye);


			Transform newRightEye = new GameObject("RightEye").transform;
			newRightEye.SetParent(basicAnimalScript.GetAnimalMotor().GetModelTransform());
			newRightEye.localScale = Vector3.one;
			newRightEye.localEulerAngles = Vector3.zero; 
			newRightEye.localPosition = new Vector3(sightRange / 2, 0, 0);
			eyes.Add(newRightEye);
		}
	}

    public override void UpdateOrgan() {
    }

    public List<BasicOrganismScript> GetOrganismsInRange(List<BasicOrganismScript> organisms) {
		List<BasicOrganismScript> organismsInRange = new List<BasicOrganismScript>();
		foreach (var organism in organisms) {
			if (WithinRange(organism.GetOrganismMotor().GetModelTransform().position, sightRange))
				organismsInRange.Add(organism);
		}
		return organismsInRange;
	}

	public List<BasicAnimalScript> GetAnimalsInRange(List<BasicAnimalScript> animals) {
		List<BasicAnimalScript> animalsInRange = new List<BasicAnimalScript>();
		foreach (var animal in animals) {
			if (WithinRange(animal.position, sightRange))
				animalsInRange.Add(animal);
		}
		return animalsInRange;
	}

	public List<Eddible> GetEddiblesInRange(List<Eddible> eddibles) {
		List<Eddible> eddiblesInRange = new List<Eddible>();
		foreach (var eddibe in eddibles) {
			if (WithinRange(eddibe.GetPosition(), sightRange))
				eddiblesInRange.Add(eddibe);
		}
		return eddiblesInRange;
	}

	bool WithinRange(Vector3 position, float _range) {
         foreach (var eye in eyes) {
  			if (Vector3.Distance(position, eye.position) <= _range / 2)
				return true;
        }
		return false;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthOrgan : AnimalOrgan {

	public Transform mouth { private set; get; }

	public void SetupOrgan(AnimalSpeciesMouth animalSpeciesMouth, Animal animal) {
		base.SetupOrgan(animalSpeciesMouth, animal);
		mouth = new GameObject("Mouth").transform;
		mouth.SetParent(GetAnimal().GetAnimalMotor().GetModelTransform());
		mouth.localScale = Vector3.one;
		mouth.localEulerAngles = Vector3.zero;
		mouth.localPosition = new Vector3(0, 0, animalSpeciesMouth.eatRange / 2f / GetAnimal().GetAnimalMotor().GetModelTransform().lossyScale.z);

	}

    public override void UpdateOrgan() {

	}

    public override void ResetOrgan() {
    }

	public float GetEatTime() {
		return GetSpeciesMouth().eatTime;
	}

	public float GetBiteSize() {
		return GetSpeciesMouth().biteSize;
    }


	public float GetEatRange() {
		return GetSpeciesMouth().eatRange;
	}

	public AnimalSpeciesMouth GetSpeciesMouth() {
		return (AnimalSpeciesMouth)GetSpeciesOrgan();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthOrgan : AnimalOrgan {

	private AnimalSpeciesMouth animalSpeciesMouth;
	public Transform mouth { private set; get; }

	public void SetupOrgan(AnimalSpeciesMouth animalSpeciesMouth, Animal animal) {
		base.SetupOrgan(animalSpeciesMouth, animal);
		this.animalSpeciesMouth = animalSpeciesMouth;
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
		return animalSpeciesMouth.eatTime;
	}

	public float GetBiteSize() {
		return animalSpeciesMouth.biteSize;
    }

}

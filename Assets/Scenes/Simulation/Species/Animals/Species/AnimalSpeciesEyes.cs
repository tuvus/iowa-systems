using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesEyes : AnimalSpeciesOrgan {
	public GameObject eyes;
	public float sightRange;
	public EyesOrgan.EyeTypes eyeType;

	public override void MakeOrganism(Animal animal) {
		EyesOrgan eyesScript = GetAnimalSpecies().InstantiateNewOrgan(eyes, animal).GetComponent<EyesOrgan>();
		eyesScript.SetupOrgan(this, animal);
	}
}

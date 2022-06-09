using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpeciesMouth : AnimalSpeciesOrgan {
	public GameObject mouth;

	public float biteSize;
	public float eatRange;
	public float eatTime;

	public override void MakeOrganism(Animal animal) {
		MouthOrgan mouthScript = GetAnimalSpecies().InstantiateNewOrgan(mouth, animal).GetComponent<MouthOrgan>();
		mouthScript.SetupOrgan(this, animal);
	}
}

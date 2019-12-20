using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesRoots : MonoBehaviour {

	public GameObject roots;

	public float rootAgeRequirement;
	public float rootGrowthRate;
	public float rootGrowthMax;

	public float rootHealthBonus;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newRoot = Instantiate(roots, _newOrganism.transform);
		Roots newRootRoot = newRoot.GetComponent<Roots>();
		newRootRoot.rootAgeRequirement = rootAgeRequirement;
		newRootRoot.rootGrowthRate = rootGrowthRate;
		newRootRoot.rootGrowthMax = rootGrowthMax;

		newRootRoot.rootHealthBonus = rootHealthBonus;

	}
	public void MakeNewGrownOrganism(GameObject _newOrganism) {

	}

}

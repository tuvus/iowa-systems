using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpeciesRoots : MonoBehaviour {

	public GameObject roots;

	public float rootGrowthRate;
	public float rootGrowthMax;

	public float rootFertilityBonus;

	public void makeOrganism(GameObject _newOrganism) {
		GameObject newRoot = Instantiate(roots, _newOrganism.transform);
		Roots newRootRoot = newRoot.GetComponent<Roots>();
		newRootRoot.rootGrowthRate = rootGrowthRate;
		newRootRoot.rootGrowthMax = rootGrowthMax;

		newRootRoot.rootFertilityBonus = rootFertilityBonus;

	}
}

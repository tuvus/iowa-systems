using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour {

	private BasicPlantScript plantScript;

	public float rootGrowth;
	public float rootGrowthRate;
	public float rootGrowthMax;

	public float rootFertilityBonus;

	void Start () {
		plantScript = GetComponentInParent<BasicPlantScript>();
		rootGrowth = 0;
		rootFertilityBonus = rootFertilityBonus * Random.Range(0.8f, 1.2f);
		rootGrowthRate = rootGrowthRate * Random.Range(0.8f, 1.2f);
	}

	void FixedUpdate() {
		if (plantScript.refreshed == true) {
			if (plantScript.storedGrowth >= rootGrowthRate && rootGrowth < rootGrowthMax) {
				plantScript.storedGrowth -= rootGrowthRate;
				rootGrowthRate -= rootGrowthRate / 1000;
				rootGrowth += rootGrowthRate;
				if (rootGrowth > rootGrowthMax) {
					rootGrowth = rootGrowthMax;
				}
			}
			if (rootGrowth == rootGrowthMax) {
				plantScript.fertility += (rootFertilityBonus * plantScript.organismCount);
			}
		}
	}
}
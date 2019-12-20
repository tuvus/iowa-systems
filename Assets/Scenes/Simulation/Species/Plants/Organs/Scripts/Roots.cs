using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour {

	private BasicPlantScript plantScript;

	public float rootAgeRequirement;
	public float rootGrowth;
	public float rootGrowthRate;
	public float rootGrowthMax;

	public float rootHealthBonus;

	void Start () {
		plantScript = GetComponentInParent<BasicPlantScript>();
		rootGrowth = 0;
		rootHealthBonus = rootHealthBonus * Random.Range(0.8f, 1.2f);
		rootGrowthRate = rootGrowthRate * Random.Range(0.8f, 1.2f);
	}

	void FixedUpdate() {
		if (plantScript.refreshed == true) {
			if (plantScript.age >= rootAgeRequirement && rootGrowth < rootGrowthMax) {
				Grow();
			}
			if (rootGrowth > 0) {
				if (plantScript.inSun) {
					if (plantScript.health <= plantScript.maxHealth) {
						plantScript.health += rootHealthBonus * (rootGrowth / rootGrowthMax);
					}
				}
			}
			if (rootGrowth >= rootGrowthMax) {
				if (transform.parent.GetComponentInChildren<FlowerSeed>() != null && transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowth < transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowthMax) {
					transform.parent.GetComponentInChildren<FlowerSeed>().flowerGrowth += rootGrowthRate;
					rootGrowthRate -= rootGrowthRate / 1000;
				} else if (transform.parent.GetComponentInChildren<FlowerFruit>() != null && transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowth < transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowthMax) {
					transform.parent.GetComponentInChildren<FlowerFruit>().flowerGrowth += rootGrowthRate;
					rootGrowthRate -= rootGrowthRate / 1000;
				} else if (GetComponentInParent<VegetativePropagation>() != null && transform.parent.GetComponentInChildren<VegetativePropagation>().growth < transform.parent.GetComponentInChildren<VegetativePropagation>().growthMax) {
					GetComponentInParent<VegetativePropagation>().growth += rootGrowthRate;
					rootGrowthRate -= rootGrowthRate / 1000;
				}
			}

		}
	}
	void Grow() {
		rootGrowthRate -= rootGrowthRate / 1000;
		rootGrowth += rootGrowthRate;
		if (rootGrowth > rootGrowthMax) {
			rootGrowth = rootGrowthMax;
		}
	}

}
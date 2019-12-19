using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatFoodScript : MonoBehaviour {

	public string foodType;
	public float floatFoodCount;
	public float deterationTime;
	public float foodGain;
	public float eatNoiseRange;

	void Start() {
		transform.localScale = new Vector3(0.004f, 0.002f, 0.002f);
	}
	void FixedUpdate() {
		deterationTime -= 0.1f;
		if (deterationTime <= 0) {
			Destroy(gameObject);
		}
	}

	public float eaten(float _BiteSize) {
		if ((floatFoodCount != 0)) {
			if (floatFoodCount > 0) {
				if (floatFoodCount >= _BiteSize) {
					floatFoodCount -= _BiteSize;
					return (foodGain * _BiteSize);
				} else if ((floatFoodCount < _BiteSize) && (floatFoodCount != 0)) {
					floatFoodCount -= _BiteSize;
					return (foodGain * floatFoodCount);
				}
			}
		}
		if (floatFoodCount <= 0) {
			Destroy(gameObject);
		}
		return (0);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantFoodScript : MonoBehaviour {

	public string foodType;
	public float floatFoodCount;
	public int intFoodCount;
	public float foodGain;
	public float eatNoiseRange;

	public bool checkFood () {
		if ((floatFoodCount != 0) || (intFoodCount != 0)) {
			return true;
		}
		return false;
	}

	public float eaten (float _BiteSize) {
		if ((floatFoodCount != 0) || (intFoodCount != 0)) {
			if (floatFoodCount > 0) {
				if (floatFoodCount >= _BiteSize) {
					floatFoodCount -= _BiteSize;
					return (foodGain * _BiteSize);
				} else if ((floatFoodCount < _BiteSize) && (floatFoodCount != 0)) {
					floatFoodCount -= _BiteSize;
					return (foodGain * floatFoodCount);
				}
			}
			if (intFoodCount > 0) {
				if (intFoodCount >= _BiteSize) {
					intFoodCount -= Mathf.RoundToInt(_BiteSize);
					intFoodCount -= Mathf.RoundToInt(_BiteSize);
					return (foodGain * Mathf.RoundToInt(_BiteSize));
				} else if ((intFoodCount < Mathf.RoundToInt(_BiteSize)) && (intFoodCount != 0)) {
					intFoodCount -= Mathf.RoundToInt(_BiteSize);
					intFoodCount -= Mathf.RoundToInt(_BiteSize);
					return (foodGain * intFoodCount);
				}
			}
		}
		if (intFoodCount <= 0 && floatFoodCount <= 0) {
			Destroy(gameObject);
		}
		return (0);
	}
}
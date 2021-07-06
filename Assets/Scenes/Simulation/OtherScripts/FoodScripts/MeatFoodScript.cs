using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatFoodScript : BasicFoodScript {

	public float deterationTime;
	public float foodCount;

	public override float Eat(float _BiteSize) {
		if (foodCount >= _BiteSize) {
			foodCount -= _BiteSize;
			return foodGain * _BiteSize;
		}
		if (foodCount < _BiteSize) {
			float foodTaken = foodCount;
			foodCount = 0;
			earth.OnEndFrame += OnDestroyFood;
			return foodGain * foodTaken;
		}
		return 0;
	}

	public override void UpdateFood() {
		deterationTime -= Time.fixedDeltaTime * 0.1f;
		if (deterationTime <= 0) {
			earth.OnEndFrame += OnDestroyFood;
		}
	}

    public override bool HasFood() {
		if (foodCount > 0)
			return true;
		return false;
    }
}

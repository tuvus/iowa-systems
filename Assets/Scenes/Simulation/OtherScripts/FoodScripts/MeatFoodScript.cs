using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatFoodScript : BasicFoodScript {

	public float deterationTime;
	public float foodCount;

	internal override void TakeDamage(float _damage) {
	}

	public override float Eaten(float _BiteSize) {
		if (HasFood()) {
			if (foodCount >= _BiteSize) {
				foodCount -= _BiteSize;
				TakeDamage(_BiteSize);
				return foodGain * _BiteSize;
			} else if (foodCount < _BiteSize) {
				float foodTaken = foodCount;
				foodCount = 0;
				TakeDamage(foodTaken);
				return foodGain * foodTaken;
			}
		}
		Destroy(gameObject);
		return 0;
	}

	void FixedUpdate() {
		deterationTime -= Time.fixedDeltaTime * 0.1f;
		if (deterationTime <= 0) {
			Destroy(gameObject);
		}
	}

    public override bool HasFood() {
		if (foodCount > 0)
			return true;
		return false;
    }
}

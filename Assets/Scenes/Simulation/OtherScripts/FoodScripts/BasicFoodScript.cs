using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicFoodScript : MonoBehaviour {
	public string foodType;
	public float foodGain;
	public float eatNoiseRange;

	public void SetupFoodType(string _foodType, float _foodGain, float _eatNoiseRange) {
		foodType = _foodType;
		foodGain = _foodGain;
		eatNoiseRange = _eatNoiseRange;
    }

	public abstract bool HasFood();

	public abstract float Eaten(float _BiteSize);

	internal abstract void TakeDamage(float _damage);
}

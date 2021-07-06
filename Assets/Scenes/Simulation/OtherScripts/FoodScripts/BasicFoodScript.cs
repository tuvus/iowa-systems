using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicFoodScript : MonoBehaviour {
	internal EarthScript earth;
	internal string foodType;
	public float foodGain;
	public float eatNoiseRange;

	public void SetupFoodType(string foodType, float foodGain, float eatNoiseRange, EarthScript earth) {
		this.earth = earth;
		this.foodType = foodType;
		this.foodGain = foodGain;
		this.eatNoiseRange = eatNoiseRange;
		GetComponent<Eddible>().postion = transform.position;
		earth.OnEndFrame += OnAddFood;
	}

	public abstract void UpdateFood();

	public void OnAddFood(object sender, System.EventArgs info) {
		earth.OnEndFrame -= OnAddFood;
		earth.AddObject(this);
	}

	internal void OnDestroyFood(object sender, System.EventArgs info) {
		earth.OnEndFrame -= OnDestroyFood;
		earth.RemoveObject(this);
		DestroyFood();
	}

	void DestroyFood() {
		Destroy(gameObject);
    }
	public abstract float Eat(float _BiteSize);

	public abstract bool HasFood();


	public string GetFoodType() {
		return foodType;
    }


	public Eddible GetEddible() {
		return GetComponent<Eddible>();
    }
}

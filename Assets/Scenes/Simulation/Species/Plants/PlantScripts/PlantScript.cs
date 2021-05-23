using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : BasicOrganismScript {
	//Plants Stats
	public string type;
	public float health;
	public float growth;
	public float maxGrowth;
	public int organismCount;

	public override void SetUpSpecificOrganism() {
		maxGrowth = maxGrowth * Random.Range(0.8f, 1.2f);
		growth = Random.Range(0.1f, maxGrowth);
		health = growth;
	}

	void FixedUpdate() {
		Grow(Time.fixedDeltaTime, CalculateSun());
	}

	public void Grow(float _time, float _sun) {
		age += _time;
		float newGrowth = _time * _sun / 10;
		if (health < newGrowth) {
			newGrowth = AddHealth(newGrowth);
		}
		newGrowth = AddGrowth(newGrowth);
		if (GetSeedOrgan() != null)
			GetSeedOrgan().Grow(newGrowth, _time);
	}

	public float AddHealth(float _value) {
		float extraGrowth = 0;
		health += _value;
		if (health > maxGrowth) {
			extraGrowth = health - maxGrowth;
			health = maxGrowth;
		}
		return extraGrowth;
	}

	public float AddGrowth(float _value) {
		float extraGrowth = 0;
		growth += _value;
		if (growth > maxGrowth) {
			extraGrowth = growth - maxGrowth;
			growth = maxGrowth;
			AddHealth(extraGrowth);
		}
		return extraGrowth;
	}

	public float Eat(float _biteSize) {
		return TakeHealth(_biteSize);
    }

	public float TakeHealth(float _health) {
		health -= _health;
		if (health < 0) {
			KillOrganism();
			return _health - health;
        }
		return _health;
    }

	public float CalculateSun () {
		return species.GetEarthScript().FindSunValue(transform.position);
	}

	public PlantSpeciesScript GetPlantSpecies() {
		return (PlantSpeciesScript)species;
	}

    internal override void OrganismDied() {
    }

    public SeedOrgan GetSeedOrgan() {
		return GetComponentInChildren<SeedOrgan>();
	}

	public float GetEatNoiseRange() {
		return 10;
    }

	public float CheckFood() {
		return health;
    }
}
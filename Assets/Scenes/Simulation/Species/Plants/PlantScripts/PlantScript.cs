using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : BasicOrganismScript {
	public PlantSpeciesScript plantSpecies;
	PlantScript plantParent;

	//Plants Stats
	public string type;
	public float health;
	public float growth;
	public float maxGrowth;
	public int organismCount;

	public override void SetUpSpecificOrganism(BasicOrganismScript parent) {
		position = GetOrganismMotor().GetModelTransform().position;
		GetEddible().postion = position;
		if (parent != null) {
			plantParent = parent.GetComponent<PlantScript>();
			maxGrowth = plantParent.maxGrowth * Random.Range(0.95f, 1.05f);
			growth = maxGrowth / 100;
			health = growth / 3;
			return;
        }
		maxGrowth = plantSpecies.maxGrowth * Random.Range(0.8f, 1.2f);
		growth = maxGrowth / 100;
		health = growth / 3;
	}

	#region PlantUpdate
	public override void RefreshOrganism() {
	}

	public override void UpdateOrganism() {
		Grow(GetEarthScript().simulationDeltaTime, CalculateSun());
	}
    #endregion

    #region PlantControls
    public void Grow(float _time, float _sun) {
		age += _time;
		float newGrowth = _time * _sun / 50;
		if (health < growth / 3) {
			newGrowth = AddHealth(newGrowth);
		}
		newGrowth = AddGrowth(newGrowth);
		if (GetSeedOrgan() != null)
			GetSeedOrgan().Grow(newGrowth, _time);
	}

	public float AddHealth(float _value) {
		float extraGrowth = 0;
		health += _value / 3;
		if (health > maxGrowth / 3) {
			extraGrowth = health - maxGrowth / 3;
			health = maxGrowth / 3;
		}
		return extraGrowth * 3;
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
		return TakeHealth(_biteSize * 3 * 10);
    }

	public float TakeHealth(float _health) {
		health -= _health;
		if (health < 0) {
			KillOrganism();
			return _health - health;
        }
		return _health;
    }
    
	internal override void OrganismDied() {
    }
    #endregion

    #region GetMethods
    public float CalculateSun () {
		return species.GetEarthScript().GetSunValue(transform.position);
	}

    public SeedOrgan GetSeedOrgan() {
		return GetComponentInChildren<SeedOrgan>();
	}

	public float GetEatNoiseRange() {
		return 10;
    }

	public bool HasFood() {
		if (health > 0)
			return true;
		return false;
    }

    public override string GetFoodType() {
		return species.GetFoodType();
    }
    #endregion
}
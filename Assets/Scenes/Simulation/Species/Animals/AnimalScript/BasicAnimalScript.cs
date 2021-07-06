using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimalScript : BasicOrganismScript {

	public BasicAnimalSpecies animalSpecies;
	public BasicBehaviorScript behavior;
	[SerializeField] internal BasicAnimalScript animalParent;

	public float speed;
	public float maxAge;

	public float waitTime;
	public float food;
	public float fullFood; //Amount of food required until eating is not prioritized
	public float maxFood;
	public float bodyFoodConsumption;

	public BasicAnimalScript mate;
	public float health;
	private float maxHealth;

	MouthScript mouth;
	EyesScript eyes;

    readonly List<BasicAnimalOrganScript> organs = new List<BasicAnimalOrganScript>();
	
	public override void SetUpSpecificOrganism(BasicOrganismScript parent) {
		gameObject.name = animalSpecies + "Organism";
		bodyFoodConsumption = animalSpecies.GetFoodConsumption();
		if (parent != null) {
			animalParent = parent.GetComponent<BasicAnimalScript>();
			maxAge = animalParent.maxAge * Random.Range(0.95f, 1.05f);

			age = 0;

			maxHealth = animalParent.maxHealth * Random.Range(0.95f, 1.05f);
			health = maxHealth;

			speed = animalParent.speed + Random.Range(0.95f, 1.05f);

			maxFood = animalParent.maxFood + Random.Range(0.95f, 1.05f);
			fullFood = maxFood * Random.Range(.6f, .8f);
			food = Random.Range(fullFood / 2, fullFood);
			return;
        }
		maxAge = animalSpecies.maxAge * Random.Range(0.8f, 1.2f);

		age = Random.Range(0, maxAge / 1.2f);

		maxHealth = animalSpecies.maxHealth * Random.Range(0.8f, 1.2f);
		health = maxHealth;

		speed = animalSpecies.speed + Random.Range(0.8f, 1.2f);

		maxFood = animalSpecies.maxFood + Random.Range(0.8f, 1.2f);
		fullFood = maxFood * Random.Range(.6f, .8f);
		food = Random.Range(fullFood / 2, maxFood);
	}

	#region AnimalUpdate
	public override void RefreshOrganism() {
		position = GetOrganismMotor().GetModelTransform().position;
		GetEddible().postion = position;
	}

	public override void UpdateOrganism() {
		UpdateOrgans();
		ManageAge();
		ManageWaitTime();
		ManageFood();
		ManageMovement();
		behavior.UpdateBehavior();
	}

	void UpdateOrgans() {
        foreach (var organ in organs) {
			organ.UpdateOrgan();
        }
    }

	void ManageAge() {
		bool pastReproductioveAge = false;
		if (GetReproductive().PastReproductiveAge())
			pastReproductioveAge = true;
		age += Time.fixedDeltaTime * 0.001f;
		if (!pastReproductioveAge && GetReproductive().PastReproductiveAge()) {
			GetEarthScript().OnEndFrame += AddAvailableMate;
        }
		if (age >= maxAge) {
			age = maxAge;
			health = 0;
			CheckIfDead("Age");
		}
	}

	void ManageFood () {
		if (food > maxFood) {
			food = maxFood;
		} else if (food > 0) {
			if (GetMoving()) {
				food -= bodyFoodConsumption * Time.fixedDeltaTime * 0.03f;

			} else {
				float restingFoodReduction = 0.8f;
				food -= bodyFoodConsumption * Time.fixedDeltaTime * 0.03f * restingFoodReduction;
			}
		} else if (food <= 0) {
			food = 0;
			health -= 0.1f;
			CheckIfDead("Starvation");
		}
	}

	void ManageWaitTime () {
		if (waitTime > 0) {
			waitTime -= Time.fixedDeltaTime;
			SetMoving(false);
		}
		if (waitTime < 0) {
			waitTime = 0;
		}
	}

	public void ManageMovement() {
		if (GetMoving())
			GetAnimalMotor().SetSpeed(speed);
	}
    #endregion

    #region AnimalControls
	public void SetMoving(bool _moving) {
		GetAnimalMotor().SetMoving(_moving);
    }
	
	public void Explore() {
		float random = Random.Range(0, 10f);
		GetAnimalMotor().TurnReletive(Random.Range(-random, random));
		SetMoving(true);
	}

	public void FollowOrganism(BasicOrganismScript organism) {
		GoToPoint(organism.position);
	}

	public void GoToPoint(Vector3 position) {
		GetAnimalMotor().LookAtPoint(position);
		SetMoving(true);
	}

	public void RunFromOrganism(BasicOrganismScript organism) {
		GetAnimalMotor().LookAwayFromPoint(organism.position);
		SetMoving(true);
	}

	public void Bite(BasicAnimalScript _animalToBite) {
		Debug.Log("Bite: " + _animalToBite);
		_animalToBite.Eaten(GetMouth().biteSize, this);
	}

	public bool Eat() {
		if (Full()) {
			return false;
		}
		foreach (var eddibleOrganism in GetFoodsInRange()) {
			if (eddibleOrganism.HasFood()) {
				food += eddibleOrganism.Eat(GetMouth().biteSize, this);
				CreateNewNoise();
				waitTime = GetMouth().eatTime;
				return true;
			}
		}
		return false;
	}

    public void Eaten(float _BiteSize, BasicAnimalScript _biter) {
		health -= _BiteSize;
		speed *= health / maxHealth;
		if (speed < 0) {
			speed = 0;
		}
		CheckIfDead("KilledBy:" + _biter.species.speciesDisplayName);
	}
	
	public void CreateNewNoise() {
		//NoiseScript newNoise = Instantiate(animalSpecies.noise, gameObject.transform).GetComponent<NoiseScript>();
		//newNoise.time = Random.Range(1.2f, 0.3f);
		//newNoise.range = Random.Range(GetMouth().biteSize, GetMouth().biteSize * 3);
		//newNoise.type = "eatNoise";
		//newNoise.transform.localPosition = new Vector3(0, 0, 0);
	}
	
	internal override void OrganismDied() {
		MeatFoodScript newDeadBody = animalSpecies.SpawnDeadAnimal(gameObject);
		newDeadBody.SetupFoodType(animalSpecies.speciesName, 1.2f, 10,GetEarthScript());
		newDeadBody.foodCount = (maxFood / food + 1) * animalSpecies.bodySize;
		if (animalSpecies.IsMateAvalible(this, GetReproductive().GetSex())) {
			GetEarthScript().OnEndFrame += RemoveAvailableMate;
        }
		if (mate != null) {
			mate.mate = null;
			GetEarthScript().OnEndFrame += mate.AddAvailableMate;
		}
	}

	public void AddAvailableMate(object sender, System.EventArgs info) {
		GetEarthScript().OnEndFrame -= AddAvailableMate;
		animalSpecies.AddAvalibleMate(this, behavior.reproductive.GetSex());
    }

	public void RemoveAvailableMate(object sender, System.EventArgs info) {
		GetEarthScript().OnEndFrame -= RemoveAvailableMate;
		animalSpecies.RemoveAvalibleMate(this, behavior.reproductive.GetSex());
	}
	#endregion

	#region AnimalState
	public bool Full() {
		if (food > maxFood * .9f)
			return true;
		return false;
    }

	public bool Hungry() {
		if (food < fullFood)
			return true;
		return false;
    }

	public bool GetMoving() {
		return GetAnimalMotor().GetMoving();
    }

	bool CheckIfDead(string _causeOfDeath) {
		if (health <= 0) {
			behavior.PrintState("Death:" + _causeOfDeath, 3);
            KillOrganism();
        }
		return false;
    }
    #endregion

    #region OrganControlls
	public void AddOrgan(BasicAnimalOrganScript organ) {
		organs.Add(organ);
    } 

	public void SetMouth(MouthScript _mouthScript) {
		mouth = _mouthScript;
    }

	public void SetEyes(EyesScript _eyeScript) {
		eyes = _eyeScript;
    }
	
	public MouthScript GetMouth() {
		return mouth;
    }

	public EyesScript GetEyes() {
		return eyes;
    }
    #endregion

    #region GetMethods
    public BasicAnimalSpecies GetAnimalSpecies() {
		return animalSpecies;
	}
    
	public bool CheckIfEddible(GameObject _object) {
		if (animalSpecies.GetDiet().IsEddible(_object)) {
			return true;
		}
		return false;
	}

	public AnimalMotor GetAnimalMotor() {
		return GetOrganismMotor().GetComponent<AnimalMotor>();
    }

	public List<Eddible> GetFoodsInRange() {
        return GetMouth().GetEddibleObjectsInRange(animalSpecies.eddibleFood);
	}

	public override string GetFoodType() {
		return species.GetFoodType();
	}

	public ReproductiveSystem GetReproductive() {
		return behavior.reproductive;
    }
	#endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class BasicAnimalScript : BasicOrganismScript {

	public BasicAnimalSpecies animalSpecies;
	public BasicBehaviorScript behavior;
	[SerializeField] internal BasicAnimalScript animalParent;


	public struct AnimalData {
		public float3 animalPosition;
		public float3x2 animalEyePosition;
		public float3 animalMouthPosition;
		public float animalFood;
		public float animalAge;
		public bool animalSex;
		public bool animalHasMate;
		public bool2 animalReproductionReady;

        public AnimalData(BasicAnimalScript animal) {
			animalPosition = animal.position;
			animalEyePosition = GetAnimalEyePositions(animal,animal.GetEyes());
			animalMouthPosition = GetMouthPosition(animal, animal.GetMouth());
			animalFood = animal.food;
			animalAge = animal.age;
			animalSex = animal.GetReproductive().GetSex();
			animalHasMate = AnimalHasMate(animal);
			animalReproductionReady = ReadyToReproduce(animal);
		}

		public AnimalData SetupData(BasicAnimalScript animal) {
			animalPosition = animal.position;
			animalEyePosition = GetAnimalEyePositions(animal,animal.GetEyes());
			animalMouthPosition = GetMouthPosition(animal, animal.GetMouth());
			animalFood = animal.food;
			animalAge = animal.age;
			animalSex = animal.GetReproductive().GetSex();
			animalHasMate = AnimalHasMate(animal);
			animalReproductionReady = ReadyToReproduce(animal);
			return this;
		}
		public static float3x2 GetAnimalEyePositions(BasicAnimalScript animal, EyesScript eyes) {
			if (eyes != null) {
				float3x2 eyePostions;
				if (eyes.GetEyeType() == EyesScript.EyeTypes.Foward) {
					eyePostions = new float3x2(eyes.eyes[0].position, eyes.eyes[0].position);
				} else {
					eyePostions = new float3x2(eyes.eyes[0].position, eyes.eyes[1].position);
				}
				return eyePostions;
			}
			return new float3x2(animal.position, animal.position);
		}

		public static float3 GetMouthPosition(BasicAnimalScript animal,MouthScript mouth) {
			if (mouth != null) {
				return mouth.mouth.position;
			}
			return animal.position;
        }

		public static bool AnimalHasMate(BasicAnimalScript animal) {
			if (animal.mate != null) {
				return true;
			}
			return false;
		}

		public static bool2 ReadyToReproduce(BasicAnimalScript animal) {
			if (AnimalHasMate(animal)) {
				return new bool2(animal.GetReproductive().ReadyToAttemptReproduction(),
						animal.mate.GetReproductive().ReadyToAttemptReproduction());
			}
			return new bool2(false, false);
		}
	}

	public struct PredatorData {
		public float3 animalPosition;

		public BasicAnimalScript.PredatorData SetupData(BasicAnimalScript animal) {
			animalPosition = animal.position;
			return this;
		}
	}

	public float speed;
	public float maxAge;

	public float waitTime;
	public float food;
	public float bodyFoodConsumption;

	public BasicAnimalScript mate;
    public float health;
	private float maxHealth;

    readonly List<BasicAnimalOrganScript> organs = new List<BasicAnimalOrganScript>();
	
	public override void SetUpSpecificOrganism(BasicOrganismScript parent) {
		gameObject.name = animalSpecies + "Organism";
		bodyFoodConsumption = animalSpecies.GetFoodConsumption();
		if (parent != null) {
			animalParent = parent.GetComponent<BasicAnimalScript>();
			maxAge = animalParent.maxAge * UnityEngine.Random.Range(0.95f, 1.05f);

			age = 0;

			maxHealth = animalParent.maxHealth * UnityEngine.Random.Range(0.95f, 1.05f);
			health = maxHealth;

			speed = animalParent.speed + UnityEngine.Random.Range(0.95f, 1.05f);

			food = UnityEngine.Random.Range(animalSpecies.fullFood / 2, animalSpecies.fullFood);
			return;
        }
		maxAge = animalSpecies.maxAge * UnityEngine.Random.Range(0.8f, 1.2f);

		age = UnityEngine.Random.Range(0, maxAge / 1.2f);

		maxHealth = animalSpecies.maxHealth * UnityEngine.Random.Range(0.8f, 1.2f);
		health = maxHealth;

		speed = animalSpecies.speed + UnityEngine.Random.Range(0.8f, 1.2f);

		food = UnityEngine.Random.Range(animalSpecies.fullFood / 2, animalSpecies.maxFood);
	}

	#region AnimalUpdate
	public override void RefreshOrganism() {
		position = GetOrganismMotor().GetModelTransform().position;
		GetEddible().postion = position;
	}
	
	public void UpdateAnimalBehavior(AnimalActions.ActionType actionType, BasicAnimalScript animalTarget, Eddible eddibleTarget) {
		behavior.UpdateBehavior(actionType,animalTarget,eddibleTarget);
	}

	public override void UpdateOrganism() {
		UpdateOrgans();
		ManageAge();
		ManageWaitTime();
		ManageFood();
		ManageMovement();
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
		age += GetEarthScript().simulationDeltaTime * 0.001f;
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
		if (food > animalSpecies.maxFood) {
			food = animalSpecies.maxFood;
		} else if (food > 0) {
			if (GetMoving()) {
				food -= bodyFoodConsumption * GetEarthScript().simulationDeltaTime * 0.03f;

			} else {
				float restingFoodReduction = 0.8f;
				food -= bodyFoodConsumption * GetEarthScript().simulationDeltaTime * 0.03f * restingFoodReduction;
			}
		} else if (food <= 0) {
			food = 0;
			health -= 0.1f;
			CheckIfDead("Starvation");
		}
	}

	void ManageWaitTime () {
		if (waitTime > 0) {
			waitTime -= GetEarthScript().simulationDeltaTime;
			SetMoving(false);
		}
		if (waitTime < 0) {
			waitTime = 0;
		}
	}

	public void ManageMovement() {
		if (GetMoving())
			GetAnimalMotor().SetSpeed(speed);
		GetAnimalMotor().MoveOrganism();
	}
    #endregion

    #region AnimalControls
	public void SetMoving(bool _moving) {
		GetAnimalMotor().SetMoving(_moving);
    }
	
	public void Explore() {
		float random = UnityEngine.Random.Range(0, 100f);
		GetAnimalMotor().TurnReletive(UnityEngine.Random.Range(-random * GetEarthScript().simulationDeltaTime, random * GetEarthScript().simulationDeltaTime));
		SetMoving(true);
	}

	public void FollowOrganism(BasicOrganismScript organism) {
		GoToPoint(organism.position);
	}

	public void LookAtPoint(Vector3 position) {
		GetAnimalMotor().LookAtPoint(position);
	}

	public void GoToPoint(Vector3 position) {
		LookAtPoint(position);
		SetMoving(true);
	}

	public void RunFromOrganism(BasicOrganismScript organism) {
		GetAnimalMotor().LookAwayFromPoint(organism.position);
		SetMoving(true);
	}

	public void Bite(BasicAnimalScript _animalToBite) {
		_animalToBite.Eaten(GetMouth().biteSize, this);
	}

	public bool Eat(Eddible eddible) {
		if (Full()) {
			return false;
		}
		if (eddible.HasFood()) {
			food += eddible.Eat(GetMouth().biteSize, this);
			CreateNewNoise();
			waitTime = GetMouth().eatTime;
			return true;
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
		newDeadBody.foodCount = (animalSpecies.maxFood / food + 1) * animalSpecies.bodySize;
		if (animalSpecies.IsMateAvalible(this, GetReproductive().GetSex())) {
			GetEarthScript().OnEndFrame += RemoveAvailableMate;
        }
		if (mate != null && !mate.organismDead) {
			mate.mate = null;
			GetEarthScript().OnEndFrame += mate.AddAvailableMate;
		}
	}

	public bool AttemptToMate(BasicAnimalScript potentialMate) {
		if (mate == null && potentialMate.mate == null && GetReproductive().CheckMate(potentialMate)) {
			MateWithAnimal(potentialMate);
			return true;
        }
		return false;
	}

	public void MateWithAnimal(BasicAnimalScript newMate) {
		mate = newMate;
		newMate.mate = this;
		GetEarthScript().OnEndFrame += RemoveAvailableMate;
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
		if (food > animalSpecies.maxFood * .9f)
			return true;
		return false;
    }

	public bool Hungry() {
		if (food < animalSpecies.fullFood)
			return true;
		return false;
    }

	public bool GetMoving() {
		return GetAnimalMotor().GetMoving();
    }

	bool CheckIfDead(string _causeOfDeath) {
		if (health <= 0) {
			User.Instance.PrintState("Death:" + _causeOfDeath,animalSpecies.speciesDisplayName, 3);
            KillOrganism();
        }
		return false;
    }
    #endregion

    #region OrganControlls
	public void AddOrgan(BasicAnimalOrganScript organ) {
		organs.Add(organ);
    } 

	public MouthScript GetMouth() {
        for (int i = 0; i < organs.Count; i++) {
			if (organs[i].GetType() == typeof(MouthScript))
				return (MouthScript)organs[i];
        }
		return null;
    }

	public EyesScript GetEyes() {
		for (int i = 0; i < organs.Count; i++) {
			if (organs[i].GetType() == typeof(EyesScript))
				return (EyesScript)organs[i];
		}
		return null;
	}

	public NoseScript GetNose() {
		for (int i = 0; i < organs.Count; i++) {
			if (organs[i].GetType() == typeof(NoseScript))
				return (NoseScript)organs[i];
		}
		return null;
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

	public override string GetFoodType() {
		return species.GetFoodType();
	}

	public ReproductiveSystem GetReproductive() {
		return behavior.reproductive;
    }

	#endregion
}
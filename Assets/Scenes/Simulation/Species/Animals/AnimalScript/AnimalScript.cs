﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class AnimalScript : BasicOrganismScript {

	public enum GrowthStage {
		Dead = -2,
		Corpse = -1,
		Juvinile = 0,
		Adult = 1,
	}

	public AnimalSpecies animalSpecies;
	public ReproductiveSystem reproductive;
	[SerializeField] internal AnimalScript animalParent;

	public struct AnimalData {
		public float age;
		public int speciesIndex;
		public int specificSpeciesIndex;
		public int animalIndex;
		public int zone;
		public float3 position;
		public float3x2 animalEyePosition;
		public float3 animalMouthPosition;
		public float animalFood;
		public bool animalSex;
		public bool animalHasMate;
		public bool2 animalReproductionReady;
		public GrowthStage stage;

        public AnimalData(AnimalScript animal) {
			age = animal.age;
			speciesIndex = animal.species.speciesIndex;
			specificSpeciesIndex = animal.species.specificSpeciesIndex;
			animalIndex = animal.specificOrganismIndex;
			position = animal.position;
			zone = animal.zone;
			animalEyePosition = GetAnimalEyePositions(animal,animal.GetEyes());
			animalMouthPosition = GetMouthPosition(animal, animal.GetMouth());
			animalFood = animal.food;
			animalSex = animal.GetReproductive().GetSex();
			animalHasMate = AnimalHasMate(animal);
			animalReproductionReady = ReadyToReproduce(animal);
			stage = animal.stage;
		}

		public static float3x2 GetAnimalEyePositions(AnimalScript animal, EyesScript eyes) {
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

		public static float3 GetMouthPosition(AnimalScript animal,MouthScript mouth) {
			if (mouth != null) {
				return mouth.mouth.position;
			}
			return animal.position;
        }

		public static bool AnimalHasMate(AnimalScript animal) {
			if (animal.mate != null) {
				return true;
			}
			return false;
		}

		public static bool2 ReadyToReproduce(AnimalScript animal) {
			if (AnimalHasMate(animal)) {
				return new bool2(animal.GetReproductive().ReadyToAttemptReproduction(),
						animal.mate.GetReproductive().ReadyToAttemptReproduction());
			}
			return new bool2(false, false);
		}
	}

	public struct AnimalActions {
		public enum ActionType {
			RunFromPredator = 1,
			EatFood = 2,
			GoToFood = 3,
			AttemptReproduction = 4,
			AttemptToMate = 5,
			Explore = 6,
			Idle = 7,
		}

		public ActionType actionType;
		public ZoneController.DataLocation dataLocation;

		public AnimalActions(ActionType actionType) : this(actionType, new ZoneController.DataLocation(ZoneController.DataLocation.DataType.None, -1)) {
        }
		
		public AnimalActions(ActionType actionType, ZoneController.DataLocation dataLocation) {
			this.actionType = actionType;
			this.dataLocation = dataLocation;
		}
	}

	public int animalDataIndex;

	public float waitTime;
	public float food;

	public AnimalScript mate;
    public float health;
	public GrowthStage stage;

    List<BasicAnimalOrganScript> organs = new List<BasicAnimalOrganScript>();
	
	public void SetupAnimalOrganism(AnimalSpecies animalSpecies) {
		SetUpOrganism(animalSpecies);
		this.animalSpecies = animalSpecies;
		gameObject.name = animalSpecies + "Organism";
	}

	public void SpawnAnimalRandom() {
		age = UnityEngine.Random.Range(reproductive.animalSpeciesReproductive.reproductionAge / 2, animalSpecies.maxAge / 1.2f);
		stage = GrowthStage.Juvinile;
		ManageAge();
		health = animalSpecies.maxHealth;
		food = UnityEngine.Random.Range(animalSpecies.fullFood, animalSpecies.maxFood);
		reproductive.SpawnReproductive();
	}

	public void SpawnAnimal(AnimalScript parent) {
		this.parent = parent;
		age = 0;
		stage = GrowthStage.Juvinile;
		zone = parent.zone;
		health = animalSpecies.maxHealth;
		food = UnityEngine.Random.Range(animalSpecies.fullFood / 2, animalSpecies.fullFood);
		reproductive.SpawnReproductive();
	}

	#region AnimalUpdate
	public override void RefreshOrganism() {
		position = GetOrganismMotor().GetModelTransform().position;
	}
	
	public override void UpdateOrganism() {
		if (!spawned || stage == GrowthStage.Corpse)
			return;
		UpdateOrgans();
		if (ManageAge())
			return;
		ManageWaitTime();
		if (ManageFood())
			return;
		ManageMovement();
	}

	public void UpdateCorpse() {
		if (!spawned || stage == GrowthStage.Dead)
			return;
		ManageCorpse();
	}

	void UpdateOrgans() {
        foreach (var organ in organs) {
			organ.UpdateOrgan();
        }
    }

	bool ManageAge() {
		age += GetEarthScript().simulationDeltaTime;
		if (stage == GrowthStage.Juvinile && reproductive.IsMature()) {
			stage = GrowthStage.Adult;
		}
		if (age >= animalSpecies.maxAge) {
			health = 0;
			if (CheckIfDead("Age"))
				return true;
		}
		return false;
	}

	void ManageWaitTime() {
		if (waitTime > 0) {
			waitTime -= GetEarthScript().simulationDeltaTime;
			SetMoving(false);
		}
		if (waitTime < 0) {
			waitTime = 0;
		}
	}

	bool ManageFood () {
		if (food > animalSpecies.maxFood) {
			food = animalSpecies.maxFood;
			return false;
		}
		if (food > 0) {
			if (GetMoving()) {
				food -= animalSpecies.GetFoodConsumption() * GetEarthScript().simulationDeltaTime / 24;

			} else {
				float restingFoodReduction = 0.6f;
				food -= animalSpecies.GetFoodConsumption() * GetEarthScript().simulationDeltaTime * restingFoodReduction / 24;
			}
			return false;
		}
		if (food <= 0) {
			food = 0;
			health -= animalSpecies.GetFoodConsumption() * GetEarthScript().simulationDeltaTime / 24 / 5;
			if (CheckIfDead("Starvation")) {
				return true;
			}
		}
		return false;
	}

	void ManageMovement() {
		if (GetMoving())
			GetAnimalMotor().SetSpeed(animalSpecies.speed * (health / animalSpecies.maxHealth / 2) + 0.5f);
		GetAnimalMotor().MoveOrganism();
	}

	void ManageCorpse() {
		age -= GetEarthScript().simulationDeltaTime;
		if (age <= 0) {
			animalSpecies.DespawnCorpse(this);
			RemoveFromZone();
        }
	}
	#endregion

	#region AnimalControls
	public override void AddToZone(int zoneIndex) {
		GetZoneController().AddFoodTypeToZone(zoneIndex, animalSpecies.GetFoodIndex(), new ZoneController.DataLocation(this));
	}

    public override void RemoveFromZone() {
		GetZoneController().RemoveFoodTypeFromZone(zone, animalSpecies.GetFoodIndex(), new ZoneController.DataLocation(this));
		base.RemoveFromZone();
	}

    public void SetMoving(bool _moving) {
		GetAnimalMotor().SetMoving(_moving);
    }
	
	public void Idle() {
		SetMoving(false);
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

	public bool Eat(PlantScript plant) {
		if (Full()) {
			return false;
		}
		AddFood(plant.EatPlant(this,math.min(GetMouth().biteSize * GetBiteAmmount(),animalSpecies.maxFood - food)));
		return true;
	}


	public bool Eat(AnimalScript animal) {
		if (Full()) {
			return false;
		}
		AddFood(animal.Eaten(this, math.min(GetMouth().biteSize * GetBiteAmmount(), animalSpecies.maxFood - food)));
		return true;
	}

	private float GetBiteAmmount() {
		return species.GetEarthScript().simulationDeltaTime / GetMouth().eatTime;
	}

	public void AddFood(float food) {
		this.food += food;
		if (this.food > animalSpecies.maxFood) {
			this.food = animalSpecies.maxFood;
		}
	}

    public float Eaten(AnimalScript biter, float biteSize) {
		if (!spawned)
			Debug.LogError("Trying to eat an organism that is not spawned.");
		if (stage == GrowthStage.Juvinile || stage == GrowthStage.Adult) {
			if (health > biteSize) { 
				health -= biteSize;
			} else {
				biteSize -= health;
				health = 0;
				CheckIfDead("KilledBy:" + biter.species.speciesDisplayName);
            }
		}
		if (stage == GrowthStage.Corpse) {
			if (food > biteSize) {
				food -= biteSize;
				return biteSize;
			} else {
				float returnFood = food;
				food = 0;
				animalSpecies.DespawnCorpse(this);
				return returnFood;
			}
		} else {
			return 0;
        }
	}
	
	internal override void OrganismDied() {
		animalSpecies.SpawnCorpse(this);
		stage = GrowthStage.Corpse;
		food = animalSpecies.bodyWeight * (food / animalSpecies.fullFood + 1);
		age = animalSpecies.deteriationTime;
        if (mate != null) {
			mate.mate = null;
			mate = null;
		}
		GetMeshRenderer().material.color = animalSpecies.GetCorpseColor();
	}

	public bool AttemptToMate(AnimalScript potentialMate) {
		if (mate == null && potentialMate.mate == null && GetReproductive().CheckMate(potentialMate)) {
			MateWithAnimal(potentialMate);
			return true;
        }
		return false;
	}

	public void MateWithAnimal(AnimalScript newMate) {
		mate = newMate;
		newMate.mate = this;
	}

	public void ResetAnimal() {
		stage = GrowthStage.Dead;
		SetOrganismZone(-1);
		age = 0;
		food = 0;
		health = 0;
        for (int i = 0; i < organs.Count; i++) {
			organs[i].ResetOrgan();
        }
		GetEarthScript().GetZoneController().allAnimals[animalDataIndex] = new AnimalData(this);
		GetMeshRenderer().material.color = species.speciesColor;
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

	bool CheckIfDead(string causeOfDeath) {
		if (health <= 0) {
			User.Instance.PrintState("Death:" + causeOfDeath, animalSpecies.speciesDisplayName, 3);
            KillOrganism();
			return true;
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
	public AnimalSpecies GetAnimalSpecies() {
		return animalSpecies;
	}

	public AnimalMotor GetAnimalMotor() {
		return GetOrganismMotor().GetComponent<AnimalMotor>();
    }

	public ReproductiveSystem GetReproductive() {
		return reproductive;
    }
	#endregion
}
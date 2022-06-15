using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class AnimalSpecies : Species {
	public GameObject basicOrganism;

	public Color corpseColor;
	//AnimalStats
	[Tooltip("Body weight is wheight in kilograms")]
	public float bodyWeight;
	public float maxHealth;
	public float speed;
	[Tooltip("foodConsumption is daily food eaten in kilograms")]
    public float foodConsumption;
	public float fullFood;
	[Tooltip("maxFood is food stored in kilograms")]
	public float maxFood;
	public int maxAge;

	public float deteriationTime;

	int foodIndex;
	[SerializeField] List<string> eddibleFoodTypesInput = new List<string>();

	public enum ListType {
		unlisted = -1,
		inactiveAnimals = 0,
		activeCorpses = 1,
		activeAnimals = 2,
	}

	[SerializeField] List<Animal> animals;
	[SerializeField] List<int> activeAnimals;
	[SerializeField] List<int> activeCorpses;
	[SerializeField] List<int> inactiveAnimals;
	[SerializeField] List<ChangeAnimalList> changeAnimalList;

    struct ChangeAnimalList {
		public ListType toList;
		public ListType fromList;
		public int animalIndex;

        public ChangeAnimalList(ListType toList, ListType fromList, int animalIndex) {
			this.toList = toList;
			this.fromList = fromList;
			this.animalIndex = animalIndex;
        }
    }

	AnimalJobController animalJobController;
	public NativeArray<int> eddibleFoodTypes;
	public NativeArray<int> predatorFoodTypes;

    #region StartSimulation
    public override void SetupSimulation(Earth earth) {
		animalJobController = gameObject.AddComponent<AnimalJobController>();
		animalJobController.SetUpJobController(this);
		base.SetupSimulation(earth);
		fullFood = maxFood * .7f;
    }

    public override void SetupSpeciesFoodType() {
		foodIndex = GetEarth().GetIndexOfFoodType(speciesName);
		List<int> tempEddibleFoodTypes = new List<int>();
		for (int i = 0; i < eddibleFoodTypesInput.Count; i++) {
			if (GetEarth().GetIndexOfFoodType(eddibleFoodTypesInput[i]) != -1) {
				tempEddibleFoodTypes.Add(GetEarth().GetIndexOfFoodType(eddibleFoodTypesInput[i]));
			}
		}
		eddibleFoodTypes = new NativeArray<int>(tempEddibleFoodTypes.Count, Allocator.Persistent);
		for (int i = 0; i < tempEddibleFoodTypes.Count; i++) {
			eddibleFoodTypes[i] = tempEddibleFoodTypes[i];
		}
	}

	public void SetupAnimalPredatorSpeciesFoodType() {
		List<int> tempPredatorFoodTypes = new List<int>();
		for (int i = 0; i < SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies().Count; i++) {
			if (i != specificSpeciesIndex && SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies()[i].eddibleFoodTypes.Contains(GetFoodIndex())) {
				if (!tempPredatorFoodTypes.Contains(SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies()[i].GetFoodIndex()))
					tempPredatorFoodTypes.Add(SpeciesManager.Instance.GetSpeciesMotor().GetAllAnimalSpecies()[i].GetFoodIndex());
			}
		}
		predatorFoodTypes = new NativeArray<int>(tempPredatorFoodTypes.Count, Allocator.Persistent);
		for (int i = 0; i < tempPredatorFoodTypes.Count; i++) {
			predatorFoodTypes[i] = tempPredatorFoodTypes[i];
		}
	}

	public override void StartSimulation() {
		Populate();
		UpdateOrganismData();
	}
    #endregion

    #region SpawnOrganisms
    public override void PreSpawn(int spawnNumber) {
		Animal animal = SpawnOrganism(basicOrganism).GetComponent<Animal>();
        AddOrganism(animal);
		GetEarth().GetZoneController().AddAnimal(animal);
		foreach (var animalSpeciesOrgan in GetComponents<AnimalSpeciesOrgan>()) {
			animalSpeciesOrgan.MakeOrganism(animal);
		}
		animal.SetupOrganism(this);
		DeactivateAnimal(animal, ListType.unlisted, true);
	}

    #region OnSimulationStart

    public override void Populate() {
		int organismsToSpawn = startingPopulation;
		animals = new List<Animal>(startingPopulation * 2);
		activeAnimals = new List<int>(startingPopulation * 2);
		activeCorpses = new List<int>(startingPopulation * 2);
		inactiveAnimals = new List<int>(startingPopulation * 2);
		changeAnimalList = new List<ChangeAnimalList>(startingPopulation * 2);
		PreSpawn(organismsToSpawn * 2);
		for (int i = 0; i < organismsToSpawn; i++) {
			SpawnRandomOrganism();
		}
		UpdateOrganismLists();
		GetEarth().StartFindZoneJobs();
		GetEarth().CompleteFindZoneJobs();
	}

	public override void SpawnRandomOrganism() {
		Animal animal = GetInactiveAnimal();
		ActivateAnimal(animal, ListType.inactiveAnimals, true);
		populationCount++;
		RandomiseOrganismPosition(animal);
		animal.SpawnAnimalRandom();
		AddToFindZone(animal);
	}
    #endregion

    public Animal SpawnOrganism(Animal parent) {
		Animal animal = GetInactiveAnimal();
		RemoveAnimalFromList(ListType.inactiveAnimals, animal.specificOrganismIndex);
		ActivateAnimal(animal, ListType.unlisted);
		populationCount++;
		RandomiseOrganismChildPosition(animal, parent, 2);
		animal.SpawnAnimal(parent);
		AddToFindZone(animal,parent.zone,2);
		return animal;
	}

	Animal GetInactiveAnimal() {
		if (inactiveAnimals.Count == 0) {
			PreSpawn(1);
		}
		return animals[inactiveAnimals[0]];
    }
    #endregion

    #region AnimalControls
    public override void UpdateOrganismData() {
		for (int i = 0; i < activeAnimals.Count; i++) {
			GetEarth().GetZoneController().allAnimals[animals[activeAnimals[i]].animalDataIndex] = new Animal.AnimalData(animals[activeAnimals[i]]);
		}
	}

	public override void UpdateOrganismsBehavior() {
		for (int i = 0; i < activeAnimals.Count; i++) {
			Animal.AnimalActions animalAction = GetAnimalJobController().animalActions[i];
			Animal animal = animals[activeAnimals[i]];
			switch (animalAction.actionType) {
				case Animal.AnimalActions.ActionType.Idle:
					animal.Idle();
					User.Instance.PrintState("SittingStill", speciesDisplayName, 1);
					break;
				case Animal.AnimalActions.ActionType.RunFromPredator:
					Organism targetPredatorOrganism = GetEarth().GetZoneController().GetOrganismFromDataLocation(animalAction.dataLocation);
					if (targetPredatorOrganism.spawned) {
						animal.MoveAwayFromPoint(targetPredatorOrganism.position);
						animal.SetMoving();
						User.Instance.PrintState("PredatorFound", speciesDisplayName, 2);
						break;
					}
					animal.Explore();
					User.Instance.PrintState("Exploring", speciesDisplayName, 1);
					break;
				case Animal.AnimalActions.ActionType.EatFood:
					if (animalAction.dataLocation.dataType == ZoneController.DataLocation.DataType.Animal) {
						Animal targetAnimal = GetEarth().GetZoneController().GetAnimalFromDataLocation(animalAction.dataLocation);
						if (targetAnimal.spawned && animal.Eat(targetAnimal)) {
							animal.GetAnimalMotor().LookAtPoint(targetAnimal.position);
							animal.Idle();
							User.Instance.PrintState("Eating", speciesDisplayName, 2);
							break;
						}
					}
					if (animalAction.dataLocation.dataType == ZoneController.DataLocation.DataType.Plant) {
						Plant targetPlant = GetEarth().GetZoneController().GetPlantFromDataLocation(animalAction.dataLocation);
						if (targetPlant.spawned && animal.Eat(targetPlant)) {
							animal.GetAnimalMotor().LookAtPoint(targetPlant.position);
							animal.Idle();
							User.Instance.PrintState("Eating", speciesDisplayName, 2);
							break;
						}
					}
					animal.Explore();
					User.Instance.PrintState("Exploring", speciesDisplayName, 1);
					break;
				case Animal.AnimalActions.ActionType.GoToFood:
					Organism targetGoToOrganism = GetEarth().GetZoneController().GetOrganismFromDataLocation(animalAction.dataLocation);
					if (targetGoToOrganism.spawned) {
						animal.GoToPoint(targetGoToOrganism.position, animal.GetMouth().GetEatRange() / 2);
						animal.SetMoving();
						User.Instance.PrintState("GoingToFood", speciesDisplayName, 1);
					} else {
						animal.Explore();
						User.Instance.PrintState("Exploring", speciesDisplayName, 1);
					}
					break;
				case Animal.AnimalActions.ActionType.AttemptReproduction:
					Animal targetMate = GetEarth().GetZoneController().GetAnimalFromDataLocation(animalAction.dataLocation);
					if (targetMate.spawned && animal.GetReproductive().AttemptReproduction(targetMate)) {
						animal.GoToPoint(targetMate.position, animal.GetMouth().GetEatRange() / 2);
						animal.SetMoving();
						User.Instance.PrintState("AttemptReproduction", speciesDisplayName, 2);
					} else {
						animal.Idle();
						User.Instance.PrintState("SittingStill", speciesDisplayName, 1);
					}
					break;
				case Animal.AnimalActions.ActionType.Explore:
					animal.Explore();
					User.Instance.PrintState("Exploring", speciesDisplayName, 1);
					break;
			}
		}
	}

	public override void UpdateOrganisms() {
		for (int i = 0; i < activeAnimals.Count; i++) {
			animals[activeAnimals[i]].UpdateOrganism();
		}
        for (int i = 0; i < activeCorpses.Count; i++) {
			animals[activeCorpses[i]].UpdateCorpse();
        }
	}

    public override void UpdateOrganismLists() {
        while(changeAnimalList.Count > 0) {
			AddAndRemoveAnimalToList(changeAnimalList[0]);
			changeAnimalList.RemoveAt(0);
		}
    }

	void AddAndRemoveAnimalToList(ChangeAnimalList changeAnimal) {
		RemoveAnimalFromList(changeAnimal.fromList, changeAnimal.animalIndex);
		AddAnimalToList(changeAnimal.toList, changeAnimal.animalIndex,changeAnimal.fromList);
	}

	void RemoveAnimalFromList(ListType fromList, int animalIndex) {
		switch (fromList) {
			case ListType.activeAnimals:
				activeAnimals.Remove(animalIndex);
				break;
			case ListType.activeCorpses:
				activeCorpses.Remove(animalIndex);
				break;
			case ListType.inactiveAnimals:
				inactiveAnimals.Remove(animalIndex);
				break;
		}
	}

	void AddAnimalToList(ListType toList, int animalIndex, ListType fromList) {
		switch (toList) {
			case ListType.activeAnimals:
				activeAnimals.Add(animalIndex);
				break;
			case ListType.activeCorpses:
				activeCorpses.Add(animalIndex);
				break;
			case ListType.inactiveAnimals:
				inactiveAnimals.Add(animalIndex);
				break;
		}
	}

	public float GetFoodConsumption() {
		return foodConsumption / 24;
	}

	public void AddToFindZone(Animal animal, int zone = -1, float range = 0) {
		GetEarth().GetZoneController().FindZoneController.AddFindZoneData(new FindZoneController.FindZoneData(new ZoneController.DataLocation(animal), zone, animal.position, range));
	}

	public void SpawnCorpse(Animal animal) {
		ActivateCorpse(animal, ListType.activeAnimals);
    }

	public void DespawnCorpse(Animal animal) {
		DeactivateAnimal(animal, ListType.activeCorpses);
    }

    public override void OnSettingsChanged(bool renderOrganisms) {
        for (int i = 0; i < activeAnimals.Count; i++) {
			animals[activeAnimals[i]].GetMeshRenderer().enabled = renderOrganisms;
        }
		for (int i = 0; i < activeCorpses.Count; i++) {
			animals[activeCorpses[i]].GetMeshRenderer().enabled = renderOrganisms;
		}
	}
    #endregion

    #region AnimalListControls
    public void AddOrganism(Animal newOrganism) {
		base.AddOrganism(newOrganism);
		Animal newAnimal = newOrganism;
		animals.Add(newAnimal);
		newAnimal.specificOrganismIndex = animals.Count - 1;
    }
	
	public void ActivateAnimal(Animal animal, ListType fromList, bool imediatly = false) {
		if (imediatly) {
			AddAndRemoveAnimalToList(new ChangeAnimalList(ListType.activeAnimals, fromList, animal.specificOrganismIndex));
		} else {
			changeAnimalList.Add(new ChangeAnimalList(ListType.activeAnimals, fromList, animal.specificOrganismIndex));
		}
		animal.spawned = true;
		animal.GetMeshRenderer().enabled = User.Instance.GetRenderWorldUserPref();
	}

	public void ActivateCorpse(Animal animal, ListType fromList, bool imediatly = false) {
		if (imediatly) {
			AddAndRemoveAnimalToList(new ChangeAnimalList(ListType.activeCorpses, fromList, animal.specificOrganismIndex));
		} else { 
			changeAnimalList.Add(new ChangeAnimalList(ListType.activeCorpses, fromList, animal.specificOrganismIndex));
		}
		animal.spawned = true;
		animal.GetMeshRenderer().enabled = User.Instance.GetRenderWorldUserPref();
	}

	public void DeactivateAnimal(Animal animal, ListType fromList, bool imediatly = false) {
		animal.ResetAnimal();
		if (imediatly) {
			AddAndRemoveAnimalToList(new ChangeAnimalList(ListType.inactiveAnimals, fromList, animal.specificOrganismIndex));
		} else {
			changeAnimalList.Add(new ChangeAnimalList(ListType.inactiveAnimals, fromList, animal.specificOrganismIndex));
		}
		animal.spawned = false;
		//GetEarth().GetZoneController().allAnimals[animal.animalDataIndex] = new AnimalScript.AnimalData(animal);
		animal.GetMeshRenderer().enabled = false;
	}
	#endregion

	#region GetMethods
	public int GetAnimalsCount() {
		return animals.Count;
	}

	public Animal GetAnimal(int animalIndex) {
		return animals[animalIndex];
	}

	public int GetActiveAnimalsCount() {
		return activeAnimals.Count;
    }

	public int GetActiveAnimal(int activeAnimalIndex) {
		return activeAnimals[activeAnimalIndex];
	}

	public float GetSightRange() {
		return GetComponent<AnimalSpeciesEyes>().sightRange;
    }

	public EyesOrgan.EyeTypes GetEyeType() {
		return GetComponent<AnimalSpeciesEyes>().eyeType;
	}

	public float GetEatRange() {
		return GetComponent<AnimalSpeciesMouth>().eatRange;
    }

	public float GetSmellRange() {
		return GetComponent<AnimalSpeciesNose>().smellRange;
    }

	public float GetReproductiveAge() {
		return GetComponent<AnimalSpeciesReproductiveSystem>().reproductionAge;
    }

	public AnimalJobController GetAnimalJobController() {
		return animalJobController;
    }

    public override List<string> GetOrganismFoodTypes() {
		List<string> foodTypes = new List<string>();
		foodTypes.Add(speciesName);
		return foodTypes;
    }

	public int GetFoodIndex() {
		return foodIndex;
    }

	public Color GetCorpseColor() {
		return corpseColor;
    }
	#endregion

	public void OnDestroy() {
		if (eddibleFoodTypes.IsCreated)
			eddibleFoodTypes.Dispose();
		if (predatorFoodTypes.IsCreated)
			predatorFoodTypes.Dispose();
	}
}
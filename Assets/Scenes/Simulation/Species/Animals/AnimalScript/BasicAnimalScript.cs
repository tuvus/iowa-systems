using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimalScript : BasicOrganismScript {

	public BasicAnimalSpecies animalSpecies;
	public BasicBehaviorScript behavior;

	//This Animal's Stats
	public float speed;
	public float maxAge;

	//This Animal's food Status and stats
	public float waitTime;
	public float food;
	public float fullFood; //Amount of food required until eating is not prioritized
	public float maxFood;
	public float bodyFoodConsumption;

	//This Animal's Status
	public BasicAnimalScript mate;
	public float health;
	private float maxHealth;

	public bool moving;
	public bool touchingEarth;

	public List<GameObject> nearbyObjects = new List<GameObject>();

	public override void SetUpSpecificOrganism() {
		gameObject.name = animalSpecies + "Organism";
		waitTime = 1f;
		rb.mass = animalSpecies.bodySize;

		maxAge = animalSpecies.maxAge * Random.Range(0.8f, 1.2f);
		age = Random.Range(0, maxAge / 1.2f);

		maxHealth = animalSpecies.maxHealth * Random.Range(0.8f, 1.2f);
		health = maxHealth;

		speed = animalSpecies.speed + Random.Range(0.8f, 1.2f);

		maxFood = animalSpecies.maxFood + Random.Range(0.8f, 1.2f);
		fullFood = maxFood * Random.Range(.5f, .7f);
		food = Random.Range(fullFood / 2, maxFood);
	}

	void FixedUpdate() {
		ManageNearbyObjects();
		ManagePhysics();
		ManageAge();
		ManageWaitTime();
		ManageFood();
		ManageMovement();
	}


	void ManageNearbyObjects() {
        for (int i = 0; i < nearbyObjects.Count; i++) {
            if (nearbyObjects[i] == null) {
                nearbyObjects.RemoveAt(i);
				i--;
            }
        }
    }

	void ManagePhysics() {
		rb.velocity = new Vector3(0, 0, 0);

		if (touchingEarth) {
			rb.AddForce((-transform.position - species.GetEarthScript().transform.position) / 100f);
		} else {
			rb.AddForce((-transform.position - species.GetEarthScript().transform.position));
		}
	}

	void ManageAge() {
		age += Time.fixedDeltaTime * 0.001f;
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
			if (moving) {
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
			moving = false;
		}
		if (waitTime < 0) {
			waitTime = 0;
		}
	}

	public void ManageMovement() {
		if (moving) {
			transform.Translate(transform.right * speed / 10, Space.World);
			moving = false;
		}
	}

	public void LookAtPoint (Transform _trans) {
		transform.LookAt(_trans.position);
	}

	public void OnCollisionEnter(Collision coll) {
		if (coll.gameObject.name == "Earth") {
			touchingEarth = true;
		}
	}

	public void OnCollisionExit(Collision coll) {
		if (coll.gameObject.name == "Earth") {
			touchingEarth = false;
		}
	}

	public void Eaten(float _BiteSize, BasicAnimalScript _biter) {
		health -= _BiteSize;
		speed = speed * (health / maxHealth);
		if (speed < 0) {
			speed = 0;
		}
		CheckIfDead("KilledBy:" + _biter.species.speciesDisplayName);
	}

	public bool CheckIfEddible(GameObject _object) {
		if (animalSpecies.GetDiet().IsEddible(_object)) {
			return true;
		}
		return false;
	}


    public BasicAnimalSpecies GetAnimalSpecies() {
		return animalSpecies;
	}

	bool CheckIfDead(string _causeOfDeath) {
		if (health <= 0) {
			print(species.speciesDisplayName + " Death:" + _causeOfDeath);
			KillOrganism();
        }
		return false;
    }
	internal override void OrganismDied() {
		MeatFoodScript newDeadBody = animalSpecies.SpawnDeadAnimal(gameObject);
		newDeadBody.SetupFoodType(animalSpecies.speciesName, 1, 10);
		newDeadBody.foodCount = food * 2 + animalSpecies.bodySize;
		if (mate != null)
			mate.mate = null;
	}

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
}
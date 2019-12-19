using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimalScript : MonoBehaviour {

	private GameObject earth;
	private Rigidbody rb;
	//This Animal's Stats
	public string species;
	public float speed;
	public float maxAge;
	public float maxWantedSleep;

	public List<string> diet = new List<string>();

	//This Animal's food Status and stats
	public float waitTime;
	public float food;
	public float fullFood; //Amount of food required until eating is not prioritized
	public float maxFood;
	public float bodyFoodConsumption;
	//True is Female false is male

	//This Animal's Status
	public GameObject mate;
	public float age;
	public float health;
	private float maxHealth;

	public bool move;
	public bool touchingEarth;

	public List<GameObject> nearbyObjects = new List<GameObject>();

	void Start () {
		waitTime = 1f;
		gameObject.name = species + "Organism";
		earth = GameObject.Find("Earth");
		rb = gameObject.GetComponent<Rigidbody>();

		maxHealth = health;
		fullFood = maxFood * Random.Range(.38f, 1f);
		food = Random.Range(fullFood / 6, maxFood * 0.8f);
		maxAge = maxAge + Random.Range(-maxAge / 10, maxAge / 10);
		transform.parent = earth.transform;
	}

	void FixedUpdate() {

		for (int i = 0; i < nearbyObjects.Count; i++) {
			if (nearbyObjects[i] == null) {
				nearbyObjects.Remove(null);
			}
		}
		rb.velocity = new Vector3(0, 0, 0);

		age += 0.0001f;
		if (age >= maxAge) {
			age = maxAge;
			health = 0;
		}
		if (touchingEarth) {
			rb.AddForce(-transform.position - earth.transform.position / -10f);
		} else {
			rb.AddForce(-transform.position - earth.transform.position * -10);
		}

		if (waitTime > 0) {
			waitTime -= 0.1f;
			move = false;
		}
		if (waitTime < 0) {
			waitTime = 0;
		}
		ManageFood();
		if (health <= 0) {
			Destroy(gameObject);
			if (food <= .2) {
				Debug.Log("Death:Starvation");
			} else if (age >= maxAge) {
				Debug.Log("Deat:Age");
			} else {
				Debug.Log("Death:Killed");

			}
			if (GetComponent<HerbivoreScript>()) {
				GetComponent<HerbivoreScript>().GetSpecies().GetComponent<HerbivoreSpecies>().organismCount--;
				GameObject newDeadBody = Instantiate(GetComponent<HerbivoreScript>().GetSpecies().GetComponent<HerbivoreSpecies>().deadAnimal, transform.position, transform.rotation, earth.transform);
				newDeadBody.GetComponent<MeatFoodScript>().floatFoodCount = food + (maxFood / 2);
				newDeadBody.GetComponent<MeatFoodScript>().foodType = species;
				newDeadBody.transform.localScale = new Vector3(0.004f, 0.002f, 0.002f);
			}
			if (GetComponent<CarnivoreScript>()) {
				GameObject newDeadBody = Instantiate(GetComponent<CarnivoreScript>().GetSpecies().GetComponent<CarnivoreSpecies>().deadAnimal, transform.position, transform.rotation, earth.transform);
				newDeadBody.GetComponent<MeatFoodScript>().floatFoodCount = food + (maxFood / 2);
				newDeadBody.GetComponent<MeatFoodScript>().foodType = species;
				newDeadBody.transform.localScale = new Vector3(0.004f, 0.002f, 0.002f);
			}
		}

		if (move) {
			//Move
			transform.Translate(transform.right * speed,Space.World);
		}
	}
	public void Reproduce () {
		
	}
	void ManageFood () {
		if (food > maxFood) {
			food = maxFood;
		} else if (food > 0) {
			food -= bodyFoodConsumption * 0.01f;
		} else if (food <= 0) {
			food = 0;
			health -= 0.1f;
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
	public void Eaten(float _BiteSize, GameObject _biter) {
		health -= _BiteSize;
		speed = speed * (health / maxHealth);
		if (speed < 0) {
			speed = 0;
		}
	}
}
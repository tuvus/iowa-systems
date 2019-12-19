using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthScript : MonoBehaviour {

	private HerbivoreScript herbivore;
	private CarnivoreScript carnivore;
	//private OmnivoreScript omnivore;
	private BasicAnimalScript basicAnimal;

	public bool eating;
	public float biteSize;
	public float eatTime;

	void Start() {
		herbivore = gameObject.transform.parent.GetComponent<HerbivoreScript>();
		carnivore = gameObject.transform.parent.GetComponent<CarnivoreScript>();
		//omnivore = gameObject.transform.parent.GetComponent<OmnivoreScriptScript>();

		basicAnimal = GetComponentInParent<BasicAnimalScript>();
	}

	void FixedUpdate() {

	}
	
	void OnTriggerEnter(Collider coll) {
		if ((coll.gameObject.layer == 13) || (coll.gameObject.layer == 14)) {
			if (basicAnimal.waitTime == 0) {
				if (coll.gameObject.GetComponent<PlantFoodScript>() != null) {
					if (basicAnimal.diet.Contains(coll.gameObject.GetComponent<PlantFoodScript>().foodType) == true) {
						if (basicAnimal != null) {
							if (herbivore != null) {
								herbivore.foodsInRange.Add(coll.gameObject);
							} else if (carnivore != null) {
								carnivore.foodsInRange.Add(coll.gameObject);
							} /* else if (carnivore != null) {
						carnivore.foodsInRange.Add(coll.gameObject);
					}
					 */
						}

					}
				}

			}
		}
		if ((coll.gameObject.layer == 9)) {
			if (carnivore) {
				if (coll.gameObject.GetComponent<BasicAnimalScript>() != null) {
					if (basicAnimal.diet.Contains(coll.gameObject.GetComponent<BasicAnimalScript>().species)) {
						if (carnivore != null) {
							carnivore.foodsInRange.Add(coll.gameObject);
						} /* else if (carnivore != null) {
						carnivore.foodsInRange.Add(coll.gameObject);
					}
					 */
					}
				}
			}
		}
	}
	void OnTriggerExit(Collider coll) {
		if ((coll.gameObject.layer == 13) || (coll.gameObject.layer == 15)) {
			if (coll.gameObject.GetComponent<PlantFoodScript>() != null) {
				if (basicAnimal.diet.Contains(coll.gameObject.GetComponent<PlantFoodScript>().foodType) == true) {
					if (basicAnimal != null) {
						if (herbivore != null) {
							herbivore.foodsInRange.Remove(coll.gameObject);
						} else if (carnivore != null) {
							carnivore.foodsInRange.Remove(coll.gameObject);
						} /* else if (carnivore != null) {
						carnivore.foodsInRange.Remove(coll.gameObject);
					}
					 */
					}

				}
			}
		}
		if ((coll.gameObject.layer == 9)) {
			if (carnivore) {
				if (coll.gameObject.GetComponent<BasicAnimalScript>() != null) {
					if (basicAnimal.diet.Contains(coll.gameObject.GetComponent<BasicAnimalScript>().species)) {
						if (carnivore != null) {
							carnivore.foodsInRange.Remove(coll.gameObject);
						} /* else if (carnivore != null) {
						carnivore.foodsInRange.Remove(coll.gameObject);
					}
					 */
					}
				}
			}
		}

	}
	void Eat() {

	}
}

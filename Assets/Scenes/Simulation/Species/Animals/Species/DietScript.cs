using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DietScript : MonoBehaviour {
    public List<string> diet = new List<string>();

    public bool IsEddible(GameObject _gameObject) {
        if (_gameObject.GetComponent<BasicOrganismScript>() != null && IsEddible(_gameObject.GetComponent<BasicOrganismScript>()))
            return true;
        if (_gameObject.GetComponent<PlantFoodScript>() != null && IsEddible(_gameObject.GetComponent<PlantFoodScript>()))
            return true;
        if (_gameObject.GetComponent<MeatFoodScript>() != null && IsEddible(_gameObject.GetComponent<MeatFoodScript>()))
            return true;
        return false;
    }

    public bool IsEddible(BasicSpeciesScript _species) {
        if (diet.Contains(_species.GetFoodType())) {
            return true;
        }
        return false;
    }

    public bool IsEddible(BasicOrganismScript _organism) {
        if (diet.Contains(_organism.GetFoodType())) {
            return true;
        }
        return false;
    }

    public bool IsEddible(BasicFoodScript food) {
        if (diet.Contains(food.GetFoodType())) {
            return true;
        }
        return false;
    }
}

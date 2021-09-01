using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Eddible : MonoBehaviour {

    public Vector3 postion;
    public struct Data {
        public float3 position;
        public int rank;

        public Eddible.Data SetupData(Eddible eddible) {
            position = eddible.GetPosition();
            rank = eddible.GetFoodRank();
            return this;
        }
    }

    public float Eat(float biteSize, BasicAnimalScript eater) {
        if (GetBasicAnimal() != null) {
            return EatAnimal(biteSize, eater);
        }
        if (GetPlant() != null) {
            return EatPlant(biteSize);
        }
        if (GetFood() != null) {
            return EatFood(biteSize);
        }
        return 0;
    }

    float EatAnimal(float biteSize, BasicAnimalScript eater) {
        GetBasicAnimal().Eaten(biteSize, eater);
        return 0;
    }

    float EatPlant(float biteSize) {
        return GetPlant().Eat(biteSize);
    }

    float EatFood(float biteSize) {
        return GetFood().Eat(biteSize);
    }

    public bool HasFood() {
        if (GetBasicAnimal() != null) {
            if (GetBasicAnimal().health > 0)
                return true;
            return false;
        }
        if (GetPlant() != null) {
            return GetPlant().HasFood();
        }
        if (GetFood() != null) {
            return GetFood().HasFood();
        }
        return false;
    }

    public int GetFoodRank() {
        if (HasFood()) {
            if (GetFood() != null)
                return 2;
            if (GetBasicAnimal() != null)
                return 1;
            return 0;

        }
        return -1;
    }

    public string GetFoodType() {
        if (GetBasicAnimal() != null) {
            return GetBasicAnimal().GetFoodType();
        }
        if (GetPlant() != null) {
            return GetPlant().GetFoodType();
        }
        if (GetFood() != null) {
            return GetFood().GetFoodType();
        }
        return "";
    }

    public Vector3 GetPosition() {
        return postion;
    }

    BasicAnimalScript GetBasicAnimal() {
        return GetComponent<BasicAnimalScript>();
    }

    PlantScript GetPlant() {
        return GetComponent<PlantScript>();
    }

    BasicFoodScript GetFood() {
        return GetComponent<BasicFoodScript>();
    }
}

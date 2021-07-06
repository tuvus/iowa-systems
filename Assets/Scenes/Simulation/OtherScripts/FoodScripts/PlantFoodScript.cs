using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantFoodScript : BasicFoodScript {
    public override float Eat(float _BiteSize) {
        return _BiteSize - GetPlantScript().TakeHealth(_BiteSize);
    }

    public override bool HasFood() {
        if (GetPlantScript().health > 0)
            return true;
        return false;
    }

    public override void UpdateFood() {
    }

    public PlantScript GetPlantScript() {
        return GetComponentInParent<PlantScript>();
    }
}
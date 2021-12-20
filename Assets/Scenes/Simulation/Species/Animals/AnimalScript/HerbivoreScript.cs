using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivoreScript : BasicBehaviorScript {
	public HerbivoreSpecies herbivoreSpecies;

    public override void UpdateBehavior(AnimalActions.ActionType actionType, BasicAnimalScript animalTarget) {
		basicAnimal.SetMoving(false);
		if (basicAnimal.waitTime > 0f) {
			return;
		}
        Debug.LogError("not implamented, please fix");
        //switch (actionType) {
        //    case AnimalActions.ActionType.Idle:
        //        User.Instance.PrintState("SittingStill", animalSpecies.speciesDisplayName, 1);
        //        break;
        //    case AnimalActions.ActionType.RunFromPredator:
        //        basicAnimal.RunFromOrganism(animalTarget);
        //        User.Instance.PrintState("PredatorFound", animalSpecies.speciesDisplayName, 2);
        //        break;
        //    case AnimalActions.ActionType.EatFood:
        //        basicAnimal.Eat(eddibleTarget);
        //        basicAnimal.LookAtPoint(eddibleTarget.GetPosition());
        //        User.Instance.PrintState("Eating", animalSpecies.speciesDisplayName, 2);
        //        break;
        //    case AnimalActions.ActionType.GoToFood:
        //        basicAnimal.GoToPoint(eddibleTarget.GetPosition());
        //        User.Instance.PrintState("GoingtoFood", animalSpecies.speciesDisplayName, 1);
        //        break;
        //    case AnimalActions.ActionType.AttemptReproduction:
        //        basicAnimal.GetReproductive().AttemptReproduction();
        //        basicAnimal.LookAtPoint(basicAnimal.mate.position);
        //        User.Instance.PrintState("AttemptReproduction", animalSpecies.speciesDisplayName, 2);
        //        break;
        //    case AnimalActions.ActionType.AttemptToMate:
        //        if (basicAnimal.AttemptToMate(animalTarget)) {
        //            basicAnimal.LookAtPoint(animalTarget.position);
        //            User.Instance.PrintState("FoundMate", animalSpecies.speciesDisplayName, 2);
        //        }
        //        break;
        //    case AnimalActions.ActionType.Explore:
        //        basicAnimal.Explore();
        //        User.Instance.PrintState("Exploring", animalSpecies.speciesDisplayName, 1);
        //        break;
        //}
    }
}
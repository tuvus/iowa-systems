using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivoreScript : BasicBehaviorScript {
	public CarnivoreSpecies carnivoreSpecies;

	public override void UpdateBehavior() {
		basicAnimal.SetMoving(false);
		if (basicAnimal.waitTime > 0f) {
			return;
		}

		if (RunFromPredator()) {
			PrintState("PredatorFound",2);
            return;
		}
		if (basicAnimal.Eat()) {
			PrintState("Eating", 2);
			return;
		}
		if (GoToFood()) {
			PrintState("GoingtoFood", 1);
			return;
        }
		if (reproductive.AttemptReproduction()) {
			PrintState("AttemptReproduction", 2);
			return;
		}
		if (FindMate()) {
			PrintState("FoundMate", 2);
            return;
		}
		if (!basicAnimal.Hungry()) {
			if (reproductive.ReadyToAttemptReproduction()) {
				basicAnimal.FollowOrganism(basicAnimal.mate);
				PrintState("FollingMate", 1);
				return;
			}
			if (basicAnimal.mate != null) {
				PrintState("SittingStill", 1);
				basicAnimal.SetMoving(false);
				return;
            }
		}
		basicAnimal.Explore();
		PrintState("Exploring", 1);
		return;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivoreScript : BasicBehaviorScript {

	public HerbivoreSpecies species;

	void FixedUpdate() {
		basicAnimal.moving = false;
		if (basicAnimal.waitTime == 0f) {
			if (FindPredator() != null) {
				Debug.Log("PredatorFound");
				transform.LookAt(FindPredator().transform.position);
				transform.Rotate(-transform.up * -90);
				basicAnimal.moving = false;
				return;
			}
			if (Eat()) {
				return;
			}
			if (basicAnimal.Hungry() && FindFood() != null) {
				transform.LookAt(FindFood().transform.position);
				transform.Rotate(transform.up * -90);
				basicAnimal.moving = true;
				return;
			}
			if (reproductive.AttemptReproduction()) {
				Debug.Log("AtemptingReproduction");
				return;
			}
			if (FindMate()) {
				Debug.Log("FoundMate");
				return;
			}
			if (basicAnimal.touchingEarth) {
				if (!basicAnimal.Hungry()) {
					if (reproductive.ReadyToAttemptReproduction()) {
						FollowMate();
						return;
					}
				}
				Explore();
				return;
			}
		}
	}
}
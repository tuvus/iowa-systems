using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;

public struct AnimalUpdateJob : IJobParallelFor {

    public NativeArray<AnimalActions> animalActions;

	[ReadOnly] NativeArray<BasicAnimalScript.AnimalData> animals;

	[ReadOnly] public float speciesFullFood;
	[ReadOnly] public float speciesMaxFood;
	[ReadOnly] public float speciesSightRange;
	[ReadOnly] public float speciesEatRange;
	[ReadOnly] public float speciesSmellRange;

	int availableMaleMateCount;
	[ReadOnly] NativeArray<float3> availableMaleMatePositions;
	int availableFemaleMateCount;
	[ReadOnly] NativeArray<float3> availableFemaleMatesPositions;



	int predatorCount;
	[ReadOnly] NativeArray<BasicAnimalScript.PredatorData> predators;

    public static JobHandle BeginJob(NativeArray<AnimalActions> animalActions, NativeArray<BasicAnimalScript.AnimalData> animals,int animalCount, float speciesFullFood, 
		float speciesMaxFood, float speciesSightRange, float speciesEatRange, float speciesSmellRange, int availableMaleMateCount,
		NativeArray<float3> availableMaleMatePositions,int availableFemaleMateCount, NativeArray<float3> availableFemaleMatesPositions, 
		int predatorCount, NativeArray<BasicAnimalScript.PredatorData> predators) {

		AnimalUpdateJob job = new AnimalUpdateJob() {
			animalActions = animalActions, animals = animals, speciesFullFood = speciesFullFood, speciesMaxFood = speciesMaxFood,
			speciesSightRange = speciesSightRange, speciesEatRange = speciesEatRange, speciesSmellRange = speciesSmellRange,
			availableMaleMateCount = availableMaleMateCount, availableMaleMatePositions = availableMaleMatePositions, availableFemaleMateCount = availableFemaleMateCount,
			availableFemaleMatesPositions = availableFemaleMatesPositions, predatorCount = predatorCount, predators = predators
		};

		return IJobParallelForExtensions.Schedule(job, animalCount,1);
    }

	public void Execute(int animalIndex) {
		BasicAnimalScript.AnimalData animal = animals[animalIndex];
        int closestPredator = GetClosestPredator(animal);
        if (closestPredator != -1) {
            animalActions[animalIndex] = new AnimalActions(AnimalActions.ActionType.RunFromPredator, closestPredator);
            return;
        }

        if (!IsAnimalFull(animal)) {
            int closestBestMouthFood = GetClosestBestMouthFood(animal);
            if (closestBestMouthFood != -1) {
				animalActions[animalIndex] = new AnimalActions(AnimalActions.ActionType.EatFood, closestBestMouthFood);
                return;
            }

            if (IsAnimalHungry(animal)) {
                int closestBestSightFood = GetClosestBestSightFood(animal);
                if (closestBestSightFood != -1) {
                    animalActions[animalIndex] = new AnimalActions(AnimalActions.ActionType.GoToFood, closestBestSightFood);
                    return;
                }
            }
        }

        if (!IsAnimalHungry(animal)) {
            if (DoesAnimalHaveMate(animal)) {
                if (AnimalPairReadyToAttemptReproduction(animal)) {
					animalActions[animalIndex] = new AnimalActions(AnimalActions.ActionType.AttemptReproduction);
                    return;
                }

            } else {
                int closestAvailableMate = GetClosestAvailableMate(animal);
                if (closestAvailableMate != -1) {
					animalActions[animalIndex] = new AnimalActions(AnimalActions.ActionType.AttemptToMate, closestAvailableMate);
                    return;
                }
            }
        }

        if (IsAnimalHungry(animal) || !DoesAnimalHaveMate(animal)) {
			animalActions[animalIndex] = new AnimalActions(AnimalActions.ActionType.Explore);
			return;

        } else {
			animalActions[animalIndex] = new AnimalActions(AnimalActions.ActionType.Idle);
			return;
        }
    }

    #region AnimalActions
    int GetClosestPredator(BasicAnimalScript.AnimalData animalData) {
		int closestPredator = -1;
		float predatorDistance = -1;
		for (int i = 0; i < predatorCount; i++) {
			float directDistance = GetDistance(animalData.animalPosition, predators[i].animalPosition);
			if (DistanceInSmellRange(directDistance) || DistanceInSightRange(GetClosestDistanceFromTwoPositions(animalData.animalEyePosition, predators[i].animalPosition))) {
				if (closestPredator != -1) {
					if (directDistance < predatorDistance) {
						closestPredator = i;
						predatorDistance = directDistance;
					}
				} else {
					closestPredator = i;
					predatorDistance = directDistance;
				}
			}
		}
		return closestPredator;
	}

	int GetClosestBestMouthFood(BasicAnimalScript.AnimalData animalData) {
		int closestBestFood = -1;
		float foodDistance = -1;
		int foodRank = -1;
		
  //      for (int i = 0; i < eddibleCount; i++) {
		//	float directDistance = GetDistance(animalData.animalMouthPosition, eddibles[i].position);
		//	if (DistanceInEatRange(directDistance)) {
		//		if (closestBestFood != -1) {
		//			if (eddibles[i].rank > foodRank || (eddibles[i].rank <= foodRank && directDistance < foodDistance)) {
		//				closestBestFood = i;
		//				foodDistance = directDistance;
		//				foodRank = eddibles[i].rank;
		//			}
		//		} else {
		//			closestBestFood = i;
		//			foodDistance = directDistance;
		//			foodRank = eddibles[i].rank;
		//		}
		//	}
		//}

		return closestBestFood;
    }

	int GetClosestBestSightFood(BasicAnimalScript.AnimalData animalData) {
		int closestBestFood = -1;
		float foodDistance = -1;
		int foodRank = -1;

		//for (int i = 0; i < eddibleCount; i++) {
		//	float directDistance = GetDistance(animalData.animalPosition, eddibles[i].position);
		//	if (DistanceInSmellRange(directDistance) || DistanceInSightRange(GetClosestDistanceFromTwoPositions(animalData.animalEyePosition, eddibles[i].position))) {
		//		if (closestBestFood != -1) {
		//			if (eddibles[i].rank > foodRank || (eddibles[i].rank <= foodRank && directDistance < foodDistance)) {
		//				closestBestFood = i;
		//				foodDistance = directDistance;
		//				foodRank = eddibles[i].rank;
		//			}
		//		} else {
		//			closestBestFood = i;
		//			foodDistance = directDistance;
		//			foodRank = eddibles[i].rank;
		//		}
		//	}
		//}
		return closestBestFood;
	}

	bool AnimalPairReadyToAttemptReproduction(BasicAnimalScript.AnimalData animalData) {
		if (animalData.animalReproductionReady.x && animalData.animalReproductionReady.y)
			return true;
		return false;
    }
    #endregion

    #region AnimalState
    bool IsAnimalFull(BasicAnimalScript.AnimalData animalData) {
		if (animalData.animalFood >= speciesMaxFood * .9f)
			return true;
		return false;
    }

	bool IsAnimalHungry(BasicAnimalScript.AnimalData animalData) {
		if (animalData.animalFood < speciesFullFood)
			return true;
		return false;
    }

	bool DoesAnimalHaveMate(BasicAnimalScript.AnimalData animalData) {
		return animalData.animalHasMate;
    }

	int GetClosestAvailableMate(BasicAnimalScript.AnimalData animalData) {
		int closestMate = -1;
		float mateDistance = -1;
		if (animalData.animalSex) {
            for (int i = 0; i < availableFemaleMateCount; i++) {
				float distance = GetClosestDistanceFromTwoPositions(animalData.animalEyePosition, availableFemaleMatesPositions[i]);
				if (!DistanceInSightRange(distance))
					continue;
				if (closestMate != -1) {
					if (distance < mateDistance) {
						closestMate = i;
						mateDistance = distance;
					}
                } else {
					closestMate = i;
					mateDistance = distance;
                }
            }
			return closestMate;
        } else {
			for (int i = 0; i < availableMaleMateCount; i++) {
				float distance = GetClosestDistanceFromTwoPositions(animalData.animalEyePosition, availableMaleMatePositions[i]);
				if (!DistanceInSightRange(distance))
					continue;
				if (closestMate != -1) {
					if (distance < mateDistance) {
						closestMate = i;
						mateDistance = distance;
					}
				} else {
					closestMate = i;
					mateDistance = distance;
				}
			}
			return closestMate;
		}
    }

	float GetDistance(float3 from, float3 to) {
		float distance = math.distance(from, to);
		return distance;
    }

	float GetClosestDistanceFromTwoPositions(float3x2 from,float3 to) {
		float distance = GetDistance(from.c0,to);
		float otherEyeDistance = GetDistance(from.c1,to);
		if (otherEyeDistance < distance)
			return otherEyeDistance;
		return distance;
	}

	bool DistanceInSightRange(float distance) {
		if (distance <= speciesSightRange)
			return true;
		return false;
    }

	bool DistanceInEatRange(float distance) {
		if (distance <= speciesEatRange)
			return true;
		return false;
    }

	bool DistanceInSmellRange(float distance) {
		if (distance <= speciesSmellRange)
			return true;
		return false;
    }
    #endregion
}

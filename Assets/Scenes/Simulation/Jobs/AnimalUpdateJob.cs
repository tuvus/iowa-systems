using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AnimalUpdateJob : IJobParallelFor {

    public NativeArray<AnimalScript.AnimalActions> animalActions;

	[ReadOnly] NativeArray<int> updateAnimals;
	[ReadOnly] float speciesFullFood;
	[ReadOnly] float speciesMaxFood;
	[ReadOnly] float speciesSightRange;
	[ReadOnly] EyesScript.EyeTypes speciesEyeType;
	[ReadOnly] float speciesEatRange;
	[ReadOnly] float speciesSmellRange;
	[ReadOnly] int speciesFoodType;
	[ReadOnly] NativeArray<int> eddibleFoodTypes;
	[ReadOnly] NativeArray<int> predatorFoodTypes;

	[ReadOnly] NativeArray<AnimalScript.AnimalData> allAnimals;
	[ReadOnly] NativeArray<PlantScript.PlantData> allPlants;
	[ReadOnly] NativeArray<ZoneController.ZoneData> zones;
	[ReadOnly] NativeMultiHashMap<int, int> neiboringZones;
	[ReadOnly] NativeMultiHashMap<int, int> animalsInZones;
	[ReadOnly] NativeMultiHashMap<int, int> plantsInZones;
	[ReadOnly] NativeMultiHashMap<int2, ZoneController.DataLocation> organismsByFoodTypeInZones;

	public static JobHandle BeginJob(NativeArray<AnimalScript.AnimalActions> animalActions, NativeArray<int> updateAnimals,
		int animalCount, float speciesFullFood, float speciesMaxFood, float speciesSightRange, EyesScript.EyeTypes speciesEyeType, float speciesEatRange, float speciesSmellRange,
		int speciesFoodType,NativeArray<int> eddibleFoodTypes, NativeArray<int> predatorFoodTypes,
		NativeArray<AnimalScript.AnimalData> allAnimals, NativeArray<PlantScript.PlantData> allPlants,
		NativeArray<ZoneController.ZoneData> zones, NativeMultiHashMap<int, int> neiboringZones, NativeMultiHashMap<int, int> animalsInZones,
		NativeMultiHashMap<int, int> plantsInZones, NativeMultiHashMap<int2, ZoneController.DataLocation> organismsByFoodTypeInZones) {
		AnimalUpdateJob job = new AnimalUpdateJob() {
			animalActions = animalActions, updateAnimals = updateAnimals, speciesFullFood = speciesFullFood, speciesMaxFood = speciesMaxFood,
			speciesSightRange = speciesSightRange, speciesEyeType = speciesEyeType, speciesEatRange = speciesEatRange, speciesSmellRange = speciesSmellRange,
			speciesFoodType = speciesFoodType, eddibleFoodTypes = eddibleFoodTypes, predatorFoodTypes = predatorFoodTypes,
			allAnimals = allAnimals, allPlants = allPlants, zones = zones, neiboringZones = neiboringZones, animalsInZones = animalsInZones,
			plantsInZones = plantsInZones, organismsByFoodTypeInZones = organismsByFoodTypeInZones};
		return IJobParallelForExtensions.Schedule(job, animalCount,1);
    }

	public void Execute(int animalIndex) {
		AnimalScript.AnimalData animal = allAnimals[updateAnimals[animalIndex]];
		if (animal.zone == -1) {
			Debug.LogError("Animal zone was not set. stage: " + animal.stage + " species: " + animal.speciesIndex + " animalIndex: " + animal.animalIndex);
		}
		List<int> zonesInSightRange;
		if (speciesEyeType == EyesScript.EyeTypes.Foward) {
			zonesInSightRange = ZoneCalculator.GetNearbyZones(zones, neiboringZones, animal.zone, animal.animalEyePosition.c0, speciesSightRange);
		} else {
			zonesInSightRange = ZoneCalculator.GetNearbyZonesFromTwoPositions(zones, neiboringZones, animal.zone, animal.animalEyePosition, speciesSightRange);
		}
		ZoneController.DataLocation closestPredator = GetClosestPredator(animal, zonesInSightRange);
		if (closestPredator.dataIndex != -1) {
			animalActions[animalIndex] = new AnimalScript.AnimalActions(AnimalScript.AnimalActions.ActionType.RunFromPredator, closestPredator);
			return;
		}

		if (!IsAnimalFull(animal)) {
			ZoneController.DataLocation closestBestMouthFood = GetClosestBestMouthFood(animal);
			if (closestBestMouthFood.dataType != ZoneController.DataLocation.DataType.None) {
				animalActions[animalIndex] = new AnimalScript.AnimalActions(AnimalScript.AnimalActions.ActionType.EatFood, closestBestMouthFood);
				return;
			}

			if (IsAnimalHungry(animal)) {
				ZoneController.DataLocation closestBestSightFood = GetClosestBestSightFood(animal, zonesInSightRange);
				if (closestBestSightFood.dataType != ZoneController.DataLocation.DataType.None) {
					animalActions[animalIndex] = new AnimalScript.AnimalActions(AnimalScript.AnimalActions.ActionType.GoToFood, closestBestSightFood);
					return;
				}
			}
		}

		if (!IsAnimalHungry(animal)) {
			if (IsAnimalMature(animal) && DoesAnimalHaveMate(animal)) {
				if (AnimalPairReadyToAttemptReproduction(animal)) {
					animalActions[animalIndex] = new AnimalScript.AnimalActions(AnimalScript.AnimalActions.ActionType.AttemptReproduction, new ZoneController.DataLocation());
					return;
				}
			} else {
				ZoneController.DataLocation closestAvailableMate = GetClosestAvailableMate(animal, zonesInSightRange);
				if (closestAvailableMate.dataType != ZoneController.DataLocation.DataType.None) {
					animalActions[animalIndex] = new AnimalScript.AnimalActions(AnimalScript.AnimalActions.ActionType.AttemptToMate, closestAvailableMate);
					return;
				}
			}
		}

		if (IsAnimalHungry(animal) || !DoesAnimalHaveMate(animal)) {
			animalActions[animalIndex] = new AnimalScript.AnimalActions(AnimalScript.AnimalActions.ActionType.Explore);
			return;
		}

		animalActions[animalIndex] = new AnimalScript.AnimalActions(AnimalScript.AnimalActions.ActionType.Idle);
	}


	#region AnimalActions
	ZoneController.DataLocation GetClosestPredator(AnimalScript.AnimalData animalData, List<int> zonesInSightRange) {
		ZoneController.DataLocation closestPredator = new ZoneController.DataLocation(ZoneController.DataLocation.DataType.None, -1);
		float predatorDistance = -1;
        for (int i = 0; i < zonesInSightRange.Count; i++) {
            for (int j = 0; j < predatorFoodTypes.Length; j++) {
				List<ZoneController.DataLocation> animalsInZone = ZoneCalculator.GetOrganismsInZoneByFoodType(organismsByFoodTypeInZones, zonesInSightRange[i], predatorFoodTypes[j]);
                for (int f = 0; f < animalsInZone.Count; f++) {
					if (animalsInZone[f].dataType == ZoneController.DataLocation.DataType.Animal) {
						float directDistance = GetDistance(animalData.position, allAnimals[animalsInZone[f].dataIndex].position);
						if (!(DistanceInSmellRange(directDistance) || DistanceInSightRange(GetClosestDistanceFromTwoPositions(animalData.animalEyePosition, allAnimals[animalsInZone[f].dataIndex].position))))
							continue;
						if (closestPredator.dataType == ZoneController.DataLocation.DataType.None || directDistance < predatorDistance) {
							closestPredator = animalsInZone[f];
							predatorDistance = directDistance;
						}
					}
                }
			}
        }
		return closestPredator;
	}

	ZoneController.DataLocation GetClosestBestMouthFood(AnimalScript.AnimalData animalData) {
		ZoneController.DataLocation closestBestFood = new ZoneController.DataLocation(ZoneController.DataLocation.DataType.None,-1);
		float foodDistance = -1;
		List<int> zonesInMouthRange = ZoneCalculator.GetNearbyZones(zones, neiboringZones, animalData.zone, animalData.animalMouthPosition, speciesEatRange);
		for (int j = 0; j < eddibleFoodTypes.Length; j++) {
			for (int i = 0; i < zonesInMouthRange.Count; i++) {
				List<ZoneController.DataLocation> foodInZone = ZoneCalculator.GetOrganismsInZoneByFoodType(organismsByFoodTypeInZones, zonesInMouthRange[i],eddibleFoodTypes[j]);
                for (int f = 0; f < foodInZone.Count; f++) {
					if (foodInZone[f].dataType == ZoneController.DataLocation.DataType.Plant) {
						float mouthDistance = GetDistance(animalData.animalMouthPosition, allPlants[foodInZone[f].dataIndex].position);
						if (!DistanceInEatRange(mouthDistance))
							continue;
						if (!(closestBestFood.dataType == ZoneController.DataLocation.DataType.None || mouthDistance < foodDistance))
							continue;
						closestBestFood = foodInZone[f];
						foodDistance = mouthDistance;
						continue;
					}
					if (foodInZone[f].dataType == ZoneController.DataLocation.DataType.Animal) {
						float mouthDistance = GetDistance(animalData.animalMouthPosition, allAnimals[foodInZone[f].dataIndex].position);
						if (!DistanceInEatRange(mouthDistance))
							continue;
						if (!(closestBestFood.dataType == ZoneController.DataLocation.DataType.None || mouthDistance < foodDistance))
							continue;
						closestBestFood = foodInZone[f];
						foodDistance = mouthDistance;
						continue;
					}
				}
			}
		}
		return closestBestFood;
    }

	ZoneController.DataLocation GetClosestBestSightFood(AnimalScript.AnimalData animalData, List<int> zonesInSightRange) {
		ZoneController.DataLocation closestBestFood = new ZoneController.DataLocation(ZoneController.DataLocation.DataType.None, -1);
		float foodDistance = -1;
		for (int j = 0; j < eddibleFoodTypes.Length; j++) {
			for (int i = 0; i < zonesInSightRange.Count; i++) {
                List<ZoneController.DataLocation> foodInZone = ZoneCalculator.GetOrganismsInZoneByFoodType(organismsByFoodTypeInZones, zonesInSightRange[i], eddibleFoodTypes[j]);
				for (int f = 0; f < foodInZone.Count; f++) {
					if (foodInZone[f].dataType == ZoneController.DataLocation.DataType.Plant) {
						float directDistance = GetDistance(animalData.position, allPlants[foodInZone[f].dataIndex].position);
						if (!(DistanceInSmellRange(directDistance) || DistanceInSightRange(GetEyeDistance(animalData.animalEyePosition, allPlants[foodInZone[f].dataIndex].position))))
							continue;
						if (!(closestBestFood.dataType == ZoneController.DataLocation.DataType.None || directDistance < foodDistance))
							continue;
						closestBestFood = foodInZone[f];
						foodDistance = directDistance;
						continue;
					}
					if (foodInZone[f].dataType == ZoneController.DataLocation.DataType.Animal) {
						float directDistance = GetDistance(animalData.position, allAnimals[foodInZone[f].dataIndex].position);
						if (!(DistanceInSmellRange(directDistance) || DistanceInSightRange(GetEyeDistance(animalData.animalEyePosition, allAnimals[foodInZone[f].dataIndex].position))))
							continue;
						if (!(closestBestFood.dataType == ZoneController.DataLocation.DataType.None || directDistance < foodDistance))
							continue;
						closestBestFood = foodInZone[f];
						foodDistance = directDistance;
						continue;
					}
				}
			}
		}
		return closestBestFood;
	}

	ZoneController.DataLocation GetClosestAvailableMate(AnimalScript.AnimalData animalData, List<int> zonesInSightRange) {
		ZoneController.DataLocation closestMate = new ZoneController.DataLocation(ZoneController.DataLocation.DataType.None, -1);
		float mateDistance = -1;
		for (int i = 0; i < zonesInSightRange.Count; i++) {
			List<ZoneController.DataLocation> organismsInZone = ZoneCalculator.GetOrganismsInZoneByFoodType(organismsByFoodTypeInZones, zonesInSightRange[i], speciesFoodType);
			for (int f = 0; f < organismsInZone.Count; f++) {
				if (organismsInZone[f].dataType != ZoneController.DataLocation.DataType.Animal)
					continue;
				if (allAnimals[organismsInZone[f].dataIndex].speciesIndex != animalData.speciesIndex)
					continue;
				if (animalData.animalHasMate || allAnimals[organismsInZone[f].dataIndex].animalHasMate)
					continue;
				if (animalData.animalSex == allAnimals[organismsInZone[f].dataIndex].animalSex)
					continue;
				float directDistance = GetDistance(animalData.position, allAnimals[organismsInZone[f].dataIndex].position);
				if (!(DistanceInSmellRange(directDistance) || DistanceInSightRange(GetEyeDistance(animalData.animalEyePosition, allAnimals[organismsInZone[f].dataIndex].position))))
					continue;
				if (!(closestMate.dataType == ZoneController.DataLocation.DataType.None || directDistance < mateDistance))
					continue;
				closestMate = organismsInZone[f];
				mateDistance = directDistance;
			}
		}
		return closestMate;
	}
	#endregion

	#region AnimalState
	bool IsAnimalFull(AnimalScript.AnimalData animalData) {
		if (animalData.animalFood >= speciesMaxFood * .9f)
			return true;
		return false;
    }

	bool IsAnimalHungry(AnimalScript.AnimalData animalData) {
		if (animalData.animalFood < speciesFullFood)
			return true;
		return false;
    }

	bool IsAnimalMature(AnimalScript.AnimalData animalData) {
		if (animalData.stage == AnimalScript.GrowthStage.Adult)
			return true;
		return false;
    }

	bool DoesAnimalHaveMate(AnimalScript.AnimalData animalData) {
		return animalData.animalHasMate;
    }

	bool AnimalPairReadyToAttemptReproduction(AnimalScript.AnimalData animalData) {
		if (animalData.animalReproductionReady.x && animalData.animalReproductionReady.y)
			return true;
		return false;
	}

	float GetEyeDistance(float3x2 eyePositions, float3 to) {
		if (speciesEyeType == EyesScript.EyeTypes.Foward) {
			return GetDistance(eyePositions.c0, to);
		} else {
			return GetClosestDistanceFromTwoPositions(eyePositions, to);
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
		//speciesSightRange needs to be divided by 2 because half of it is already factored in from the eye position
		if (distance <= speciesSightRange / 2)
			return true;
		return false;
    }

	bool DistanceInEatRange(float distance) {
		//speciesEayRange needs to be divided by 2 because half of it is already factored in from the mouth position
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

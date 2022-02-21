using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarsScript : BasicAnimalOrganScript {

	public float hearRange;

	List<GameObject> heardGameobjects = new List<GameObject>();

    internal override void SetUpSpecificOrgan() {
	}

	public override void UpdateOrgan() {
		//List<GameObject> hearableGameObjects = GetAllHearableGameobjects(hearRange / 2);
  //      foreach (var objectToRemove in ListHandler.RemoveElementsNotInListAndReturnThem(heardGameobjects, hearableGameObjects)) {
		//	basicAnimalScript.nearbyObjects.Remove(objectToRemove);
  //      } 
		//ListHandler.AddNewElementsInList(heardGameobjects, hearableGameObjects);

    }

	List<GameObject> GetAllHearableGameobjects(float _range) {
		List<GameObject> hearableGameObjects = new List<GameObject>();
		//foreach (var obj in basicAnimalScript.GetEarthScript().GetAllOrganisms()) {
		//	if (obj.gameObject.layer == 12)
		//		hearableGameObjects.Add(obj.gameObject);
		//}
		return hearableGameObjects;
	}



    //void OnTriggerEnter(Collider trigg) {
    //	if (trigg.gameObject.layer == 12) {
    //		if (basicAnimal.nearbyObjects != null) {
    //			basicAnimal.nearbyObjects.Add(trigg.transform.parent.gameObject);
    //		}
    //	}
    //}
    //void OnTriggerExit(Collider trigg) {
    //	if (trigg.gameObject.layer == 12) {
    //		if (basicAnimal.nearbyObjects != null) {
    //			basicAnimal.nearbyObjects.Remove(trigg.transform.parent.gameObject);
    //		}
    //	}
    //}

    public override void ResetOrgan() {
    }
}

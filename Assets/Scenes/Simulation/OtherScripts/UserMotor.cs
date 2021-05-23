using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserMotor : MonoBehaviour {
	private Transform cameraTransform;
	private SpeciesMotor history;
	private TimeUI timeUI;

	private float scrollModifyer;
	public float scrollFactor;

	public float moveSpeed;
	public float rotateSpeed;


	public void StartSimulation() {
		history = SpeciesManager.Instance.GetComponent<SpeciesMotor>();
		timeUI = GameObject.Find("TimeUI").GetComponent<TimeUI>();
		cameraTransform = Camera.main.transform;
		cameraTransform.localPosition = new Vector3(0, SimulationScript.Instance.earthSize / 1.5f, 0);
		timeUI.Pause();
	}

	void Update () {
		float earthSize = SimulationScript.Instance.earthSize;
		scrollModifyer = Mathf.Pow(cameraTransform.localPosition.y - (earthSize / 4f), 2) / -(earthSize * 20);
		if (scrollModifyer > 0)
			scrollModifyer *= -1;
		cameraTransform.localPosition += new Vector3(0, Input.mouseScrollDelta.y * scrollModifyer * scrollFactor, 0);

		if (cameraTransform.localPosition.y < (earthSize / 2) + 3) {
			cameraTransform.localPosition = new Vector3(0, (earthSize / 2) + 3, 0);
		} else if (cameraTransform.localPosition.y > earthSize * 3) {
			cameraTransform.localPosition = new Vector3(0, earthSize * 3, 0);
		}

		if (Input.GetKeyDown(KeyCode.Tab)) {
			history.ToggleGraph();
		}
		float localMoveSpeed = (((-2000000 / Mathf.Pow(earthSize / 2 - 139 - cameraTransform.localPosition.y, 2)) + 100) / 100 * moveSpeed);
		if (Input.GetKey(KeyCode.W)) {
			transform.Rotate(localMoveSpeed, 0, 0);
		}
		if (Input.GetKey(KeyCode.S)) {
			transform.Rotate(-localMoveSpeed, 0, 0);
		}
		if (Input.GetKey(KeyCode.D)) {
			transform.Rotate(0, 0, -localMoveSpeed);
		}
		if (Input.GetKey(KeyCode.A)) {
			transform.Rotate(0, 0, localMoveSpeed);
		}
		if (Input.GetKey(KeyCode.E)) {
			transform.Rotate(0, rotateSpeed, 0);
		}
		if (Input.GetKey(KeyCode.Q)) {
			transform.Rotate(0, -rotateSpeed, 0);
		}

		if (Input.GetKeyDown(KeyCode.Comma)) {
			timeUI.DecreaceTimeStep();
		}
		if (Input.GetKeyDown(KeyCode.Period)) {
			timeUI.IncreaceTimeStep();
		}
		if (Input.GetKeyDown(KeyCode.Slash)) {
			timeUI.Pause();
		}
		if (Input.GetKey(KeyCode.Escape)) {
			EndSimulation();	
		}
	}

	public void EndSimulation () {
		SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
	}


}
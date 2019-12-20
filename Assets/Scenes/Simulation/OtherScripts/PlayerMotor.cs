using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour {
	private SpeciesMotor history;
	private GameObject timeUI;
	private GameObject moveSpeedUI;

	public float moveSpeed;

	private void Start() {
		history = GameObject.Find("Species").GetComponent<SpeciesMotor>();
		timeUI = GameObject.Find("TimeUI").transform.GetChild(0).gameObject;
		moveSpeedUI = GameObject.Find("CameraMoveSpeed");
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			history.ToggleGraph();
		}

		if (Input.GetKey(KeyCode.W)) {
			transform.Rotate(moveSpeed, 0, 0);
		}
		if (Input.GetKey(KeyCode.S)) {
			transform.Rotate(-moveSpeed, 0, 0);
		}
		if (Input.GetKey(KeyCode.D)) {
			transform.Rotate(0, 0, -moveSpeed);
		}
		if (Input.GetKey(KeyCode.A)) {
			transform.Rotate(0, 0, moveSpeed);
		}
		if (Input.GetKey(KeyCode.E)) {
			transform.Rotate(0, moveSpeed, 0);
		}
		if (Input.GetKey(KeyCode.Q)) {
			transform.Rotate(0, -moveSpeed, 0);
		}
		if (Input.GetKey(KeyCode.R)) {
			transform.GetChild(0).transform.localPosition += new Vector3(0, -moveSpeed * 2, 0);
		}
		if (Input.GetKey(KeyCode.F)) {
			transform.GetChild(0).transform.localPosition += new Vector3(0, moveSpeed * 2, 0);
		}
		if (Input.GetKeyDown(KeyCode.Comma) && Time.timeScale > 0) {
			Time.timeScale -= 1;
			timeUI.GetComponent<Text>().text = "PhysicsTime:" + Time.timeScale;
		}
		if (Input.GetKeyDown(KeyCode.Period) && Time.timeScale < 10) {
			Time.timeScale += 1;
			timeUI.GetComponent<Text>().text = "PhysicsTime:" + Time.timeScale;
		}
		if (Input.GetKeyDown(KeyCode.Slash)) {
			Time.timeScale = 0;
			timeUI.GetComponent<Text>().text = "PhysicsTime:" + Time.timeScale;
		}
		if (Input.GetKey(KeyCode.Escape)) {
			SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
		}
	}
	public void MoveSpeedChange() {
		if (moveSpeedUI.GetComponentInChildren<Slider>().value >= 0) {
			moveSpeed = 1 * moveSpeedUI.GetComponentInChildren<Slider>().value;
		} else {
			moveSpeed = 1 / -moveSpeedUI.GetComponentInChildren<Slider>().value;
		}
		moveSpeedUI.GetComponentInChildren<Text>().text = "CameraSpeed:" + moveSpeed;
	}
}
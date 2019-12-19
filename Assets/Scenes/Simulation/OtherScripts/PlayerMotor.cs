using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMotor : MonoBehaviour {
	private SpeciesMotor history;

	private void Start() {
		history = GameObject.Find("Species").GetComponent<SpeciesMotor>();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			history.ToggleGraph();
		}

		if (Input.GetKey(KeyCode.W)) {
			transform.Rotate(0.1f, 0, 0);
		}
		if (Input.GetKey(KeyCode.S)) {
			transform.Rotate(-0.1f, 0, 0);
		}
		if (Input.GetKey(KeyCode.D)) {
			transform.Rotate(0, 0, -0.1f);
		}
		if (Input.GetKey(KeyCode.A)) {
			transform.Rotate(0, 0, 0.1f);
		}
		if (Input.GetKey(KeyCode.E)) {
			transform.Rotate(0, 0.1f, 0);
		}
		if (Input.GetKey(KeyCode.Q)) {
			transform.Rotate(0, -0.1f, 0);
		}
		if (Input.GetKey(KeyCode.R)) {
			transform.GetChild(0).transform.localPosition += new Vector3(0, -1, 0);
		}
		if (Input.GetKey(KeyCode.F)) {
			transform.GetChild(0).transform.localPosition += new Vector3(0, 1, 0);
		}
		if (Input.GetKeyDown(KeyCode.Comma) && Time.timeScale > 0) {
			Time.timeScale -= 1;
		}
		if (Input.GetKeyDown(KeyCode.Period) && Time.timeScale < 10) {
			Time.timeScale += 1;
		}
		if (Input.GetKeyDown(KeyCode.Slash)) {
			Time.timeScale = 0;
		}
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
	}
}

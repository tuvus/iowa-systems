using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserMotor : MonoBehaviour {
	private Transform cameraTransform;
	private SpeciesMotor speciesMotor;
	private TimeUI timeUI;
	private CameraMovementUI cameraMovementUI;
	private ZoomMovementUI zoomMovementUI;
	private GraphWindow graphWindow;

	private float scrollModifyer;
	public float scrollFactor;

	public float moveSpeed;
	public float rotateSpeed;

	public float graphBufferInput;
	float graphBufferTime;
	int graphLastRefresh;

	public void SetupSimulation(float graphBufferInput) {
		speciesMotor = SpeciesManager.Instance.GetComponent<SpeciesMotor>();
		Transform canvas = GameObject.Find("Canvas").transform;
		graphWindow = canvas.GetChild(0).GetComponent<GraphWindow>();
		timeUI = canvas.GetChild(2).GetComponent<TimeUI>();
		timeUI.SetupSimulation();
		cameraMovementUI = canvas.GetChild(3).GetComponent<CameraMovementUI>();
		zoomMovementUI = canvas.GetChild(3).GetComponent<ZoomMovementUI>();
		cameraTransform = Camera.main.transform;
		cameraTransform.localPosition = new Vector3(0, Simulation.Instance.earthSize / 1.5f, 0);
		this.graphBufferInput = graphBufferInput;
		RefreshSlider();
		timeUI.Pause();
	}

	void LateUpdate () {
		float earthSize = Simulation.Instance.earthSize;
		ManageScroll(earthSize);
		ManageGraph();
		ManageMovement(earthSize);
		ManageTime();
		ManageEndSimulation();
	}

	void ManageScroll(float earthSize) {
		if (speciesMotor.GetGraphStatus())
			return;
		//scrollModifyer = Mathf.Pow(cameraTransform.localPosition.y - (earthSize / 4f), 2) / -(earthSize * 20);
		scrollModifyer = Mathf.Pow(cameraTransform.localPosition.y - (earthSize / 4), 2) / -(earthSize * 20);
		if (scrollModifyer > 0)
			scrollModifyer *= -1;
		cameraTransform.localPosition += new Vector3(0, Input.mouseScrollDelta.y * scrollModifyer * scrollFactor, 0);

		if (cameraTransform.localPosition.y < GetMinSize()) {
			cameraTransform.localPosition = new Vector3(0, GetMinSize(), 0);
		} else if (cameraTransform.localPosition.y > GetMaxSize()) {
			cameraTransform.localPosition = new Vector3(0, GetMaxSize(), 0);
		}
		RefreshSlider();
	}

	void RefreshSlider() {
		zoomMovementUI.SetScroll((cameraTransform.localPosition.y - GetMinSize()) / GetRange());
	}

	public void SetScroll(float scroll) {
		float earthSize = Simulation.Instance.earthSize;
		cameraTransform.localPosition = new Vector3(0, (scroll * GetRange()) + GetMinSize(), 0);
    }
	float GetMaxSize() {
		return Simulation.Instance.earthSize * 3;
    }

	float GetMinSize() {
		return (Simulation.Instance.earthSize / 2) + 3;
    }

	float GetRange() {
		return GetMaxSize() - GetMinSize();
    }

	void ManageGraph() {
		if (Input.GetKeyDown(KeyCode.Tab)) {
			speciesMotor.ToggleGraph();
			graphBufferTime = graphBufferInput;
		}
		if (graphWindow.gameObject.activeSelf && graphLastRefresh < speciesMotor.refreshCount) {
			graphBufferTime -= Time.deltaTime;
			if (graphBufferTime <= 0) {
				graphWindow.RefreshGraphFromTime(graphLastRefresh);
				graphBufferTime = graphBufferInput;
                graphLastRefresh = (int)speciesMotor.refreshCount;
			}
		}
	}

	void ManageMovement(float earthSize) {
		float localMoveSpeed = (((-2000000 / Mathf.Pow(earthSize / 2 - 139 - cameraTransform.localPosition.y, 2)) + 100) / 100 * moveSpeed);

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || cameraMovementUI.GetUpButtonPressed()) {
			transform.Rotate(localMoveSpeed, 0, 0);
		}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || cameraMovementUI.GetDownButtonPressed()) {
			transform.Rotate(-localMoveSpeed, 0, 0);
		}
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || cameraMovementUI.GetLeftButtonPressed()) {
			transform.Rotate(0, 0, localMoveSpeed);
		}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || cameraMovementUI.GetRightButtonPressed()) {
			transform.Rotate(0, 0, -localMoveSpeed);
		}
		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.PageUp) || cameraMovementUI.GetTopLeftButtonPressed()) {
			transform.Rotate(0, -rotateSpeed, 0);
		}
		if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.PageDown) || cameraMovementUI.GetTopRightButtonPressed()) {
			transform.Rotate(0, rotateSpeed, 0);
		}
	}

	void ManageTime() {
		if (Input.GetKeyDown(KeyCode.Comma)) {
			timeUI.DecreaceTimeStep();
		}
		if (Input.GetKeyDown(KeyCode.Period)) {
			timeUI.IncreaceTimeStep();
		}
		if (Input.GetKeyDown(KeyCode.Slash)) {
			timeUI.Pause();
		}
	}

	public void DebugPause() {
		timeUI.Pause();
	}

	void ManageEndSimulation() {
		if (Input.GetKey(KeyCode.Escape)) {
			EndSimulation();
		}
	}

	public void EndSimulation () {
		SpeciesManager.Instance.GetSpeciesMotor().GetEarth().OnDestroy();
		SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
	}
}
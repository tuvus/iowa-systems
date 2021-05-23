using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationScript : MonoBehaviour {
	public int earthSize;
	public bool sunRotationEffect;



	public static SimulationScript Instance { get; private set; }
	EarthScript earth;
	SunScript sun;

    public void Awake() {
		Application.targetFrameRate = 60;
		if (Instance == null) {
			Instance = this;
        } else {
			Destroy(gameObject);
        }
    }

    public void StartNewSimulation() {
		PlayerPrefs.Save();
		StartCoroutine(StartSimulation());
	}

	public IEnumerator StartSimulation() {
		Destroy(GameObject.Find("EventSystem"));
		transform.SetParent(null);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Simulation", LoadSceneMode.Additive);

		while (!asyncLoad.isDone) {
			yield return null;
		}
		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Simulation"));
		SceneManager.MoveGameObjectToScene(User.Instance.gameObject, SceneManager.GetSceneByName("Simulation"));
		SpeciesManager.Instance.transform.SetParent(null);
		SceneManager.MoveGameObjectToScene(SpeciesManager.Instance.gameObject, SceneManager.GetSceneByName("Simulation"));
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

		OnFinishedLoadingAllScene();
	}

	public void OnFinishedLoadingAllScene() {
		earth = GameObject.Find("Earth").GetComponent<EarthScript>();
		sun = GameObject.Find("Sun").GetComponent<SunScript>();

        earth.SetUpEarth(earthSize);
		sun.SetupSun();
		SpeciesManager.Instance.GetSpeciesMotor().enabled = true;
		SpeciesManager.Instance.GetSpeciesMotor().StartSimulation(earth, sun);
        User.Instance.StartSimulation();
    }





    public void QuitGame() {
		PlayerPrefs.Save();
		Application.Quit();
	}
}



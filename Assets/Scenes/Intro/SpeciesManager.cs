using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeciesManager : MonoBehaviour {
	[SerializeField]
	private GameObject speciesHolderPrefab;
	private GameObject speciesMaker;

	public void Start() {
		speciesMaker = GameObject.Find("SpeciesMaker");
		//StartSimulation();
	}

	public void AddNewSpecies () {
		//GameObject newSpecies = Instantiate(speciesHolderPrefab, transform);
		speciesMaker.GetComponent<SpeciesMaker>().DisplaySpecies();
	}
	public void StartNewSimulation () {
		StartCoroutine(StartSimulation());
	}

	public IEnumerator StartSimulation () {
		
		transform.SetParent(null);
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Simulation", LoadSceneMode.Additive);

		while (!asyncLoad.isDone) {
			yield return null;
		}
		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Simulation"));
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

		OnFinishedLoadingAllScene();
	}
	public void OnFinishedLoadingAllScene () {
		GetComponent<SpeciesMotor>().enabled = true;
		GetComponent<SpeciesMotor>().StartSimulation();
	}
	public void QuitGame() {
		Application.Quit();
	}
}

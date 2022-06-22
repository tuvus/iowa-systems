using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simulation : MonoBehaviour {

    public float simulationSpeed;
    public int earthSize;
    public bool sunRotationEffect;
    public int numberOfZones;
    public int maxNeiboringZones;
    public ZoneController.ZoneSetupType zoneSetup;
    public string seed;
    public static Unity.Mathematics.Random randomGenerator;

    public static Simulation Instance { get; private set; }
    Earth earth;
    Sun sun;

    public bool simulationInitialised = false;
    public void Awake() {
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
        earth = GameObject.Find("Earth").GetComponent<Earth>();
        sun = GameObject.Find("Sun").GetComponent<Sun>();

        randomGenerator = new Unity.Mathematics.Random((uint)seed.GetHashCode());
        SpeciesManager.Instance.GetSpeciesMotor().enabled = true;
        SpeciesManager.Instance.GetSpeciesMotor().SetupSimulation(earth, sun);
        earth.SetUpEarth(earthSize, simulationSpeed);
        sun.SetupSun(earth);
        User.Instance.SetupSimulation();
        SpeciesManager.Instance.GetSpeciesMotor().StartSimulation();
        simulationInitialised = true;
        earth.StartSimulation();
    }

    public void SetFrameRate(int frameRate) {
        Application.targetFrameRate = frameRate;
    }

    public void QuitSimulation() {
        PlayerPrefs.Save();
        Application.Quit();
    }

    public Sun GetSun() {
        return sun;
    }

    public Earth GetEarth() {
        return earth;
    }
}



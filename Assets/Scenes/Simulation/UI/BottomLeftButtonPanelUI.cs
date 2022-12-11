using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomLeftButtonPanelUI : MonoBehaviour {
    public void EndSimulation() {
        Simulation.Instance.EndSimulation();
    }

    public void DisplayGraph() {
        SpeciesManager.Instance.GetSpeciesMotor().ToggleGraph();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomLeftButtonPanelUI : MonoBehaviour {
    public void EndSimulation() {
        User.Instance.GetComponent<UserMotor>().EndSimulation();
    }

    public void DisplayGraph() {
        SpeciesManager.Instance.GetComponent<SpeciesMotor>().ToggleGraph();
    }
}

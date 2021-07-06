using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovementUI : MonoBehaviour {
    public bool GetUpButtonPressed() {
        return GetMovementButtonsTransform().GetChild(0).GetComponent<ButtonHandler>().pressed;
    }

    public bool GetDownButtonPressed() {
        return GetMovementButtonsTransform().GetChild(1).GetComponent<ButtonHandler>().pressed;
    }
    public bool GetLeftButtonPressed() {
        return GetMovementButtonsTransform().GetChild(2).GetComponent<ButtonHandler>().pressed;
    }
    public bool GetRightButtonPressed() {
        return GetMovementButtonsTransform().GetChild(3).GetComponent<ButtonHandler>().pressed;
    }

    public bool GetTopLeftButtonPressed() {
        return GetMovementButtonsTransform().GetChild(4).GetComponent<ButtonHandler>().pressed;
    }

    public bool GetTopRightButtonPressed() {
        return GetMovementButtonsTransform().GetChild(5).GetComponent<ButtonHandler>().pressed;
    }

    Transform GetMovementButtonsTransform() {
        return transform.GetChild(0);
    }
}

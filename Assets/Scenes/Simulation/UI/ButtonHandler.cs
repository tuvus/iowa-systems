using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : Selectable {
    public bool pressed;

    private void Update() {
        if (IsPressed()) {
            pressed = true;
        } else {
            pressed = false;
        }
    }
}

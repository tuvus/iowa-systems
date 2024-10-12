using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ZoomMovementUI : MonoBehaviour {
    public void SetScroll(float value) {
        GetSlider().SetValueWithoutNotify(value);
    }

    public void OnSliderChanged() {
        User.Instance.GetUserMotor().SetScroll(math.pow(GetSlider().value, 2f));
    }

    Slider GetSlider() {
        return transform.GetChild(1).GetChild(0).GetComponent<Slider>();
    }
}
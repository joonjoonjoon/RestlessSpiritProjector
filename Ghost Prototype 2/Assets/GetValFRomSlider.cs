using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetValFRomSlider : MonoBehaviour {
    public Slider slider;
    private TMPro.TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update () {
		text.text = slider.value.ToString("0.000");
    }
}

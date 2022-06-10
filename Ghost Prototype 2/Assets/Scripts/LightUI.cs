using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightUI : MonoBehaviour {

    private Image image;
    private Text text;
    public Image topThreshold;
    public Text topThresholdText;
    public Image bottomThreshold;
    public Text bottomThresholdText;

    private bool initialized;
    private bool height;

    void Start()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
    }

    void Init()
    {
        topThreshold.rectTransform.anchoredPosition =
            topThreshold.rectTransform.anchoredPosition.withY(GameManager.instance.lightThresholdUp * topThreshold.transform.parent.GetComponent<RectTransform>().rect.height);
        initialized = true;
    }

    void Update() {
        if (topThreshold.transform.parent.GetComponent<RectTransform>().rect.height == 0)
            return;
        if(!initialized)
        {
            Init();
        }

        image.fillAmount = ArduinoManager.lightSensorValue;
        text.text = Mathf.Floor(ArduinoManager.lightSensorValue * 100) + "%";
        GameManager.instance.lightThresholdUp = topThreshold.rectTransform.anchoredPosition.y / topThreshold.transform.parent.GetComponent<RectTransform>().rect.height;
        topThresholdText.text = Mathf.Floor(GameManager.instance.lightThresholdUp * 100) + "%";
    }
}

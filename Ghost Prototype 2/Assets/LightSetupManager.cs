using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LightSetupManager : MonoBehaviour {

    public GlitchDebugGraph graph;
    public Slider GhostBlinkLength;
    public Slider SensorDelay;
    public Slider TimeOutBetweenDetects;
    public GameObject paused;

    void OnEnable()
    {
        graph.show = true;
        paused.SetActive(false);
        foreach (var item in GameObject.FindObjectsOfType<OrbNumberThing>())
        {
            item.mute = false;
        }
    }

    void Start()
    {
        GhostBlinkLength.value = GameManager.instance.pingSpeed;
        SensorDelay.value = GameManager.instance.signalDelay;
        TimeOutBetweenDetects.value = GameManager.instance.timeOutAfterLoop;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.graphPaused = !GameManager.instance.graphPaused;
            paused.SetActive(GameManager.instance.graphPaused);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            GameManager.instance.StartBlinkLoop();
        }

    }

    void OnDisable()
    {
        graph.show = false;
        foreach (var item in GameObject.FindObjectsOfType<OrbNumberThing>())
        {
            item.mute = true;
        }
    }

    public void GhostBlinkChanged()
    {
        GameManager.instance.pingSpeed = GhostBlinkLength.value;
    }


    public void SensorDelayChanged()
    {
        GameManager.instance.signalDelay= SensorDelay.value;
    }

    public void Timeoutchanged()
    {
        GameManager.instance.timeOutAfterLoop= TimeOutBetweenDetects.value;
    }
}

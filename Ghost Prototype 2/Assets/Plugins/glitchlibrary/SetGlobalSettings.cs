using UnityEngine;
using System.Collections;

public class SetGlobalSettings : MonoBehaviour {
    public bool overrideFps =false;
    public int fps = 60;
    public bool overrideAntiAliasing = false;
    public int antiAliasing = 2;
    public bool overrideSleepTimeout = false;
    public int sleepTimeout = -1;

    void Awake() {
        if (overrideAntiAliasing)
            QualitySettings.antiAliasing = antiAliasing;
        if (overrideFps)
            Application.targetFrameRate = fps;
        if(overrideSleepTimeout)
            Screen.sleepTimeout = sleepTimeout;
    }
}

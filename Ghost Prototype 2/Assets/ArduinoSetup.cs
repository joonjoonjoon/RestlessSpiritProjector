using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoSetup : MonoBehaviour {

    string[] ports;
    bool started;

    public float buttonPosx;
    public float buttonPosy;
    public float buttonSizex;
    public float buttonSizey;
    public float buttonSpacing;

    void OnEnable()
    {
        Refresh();
    }

    void Update()
    {
        if (!started) started = true;
    }

    void Refresh()
    {
        if(started)
        {
            ports = ArduinoManager.instance.GetSerialPortNames();
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(buttonPosx, buttonPosy, buttonSizex, buttonSizey), "[[ REFRESH ]]"))
        {
            Refresh();
        }

        if (ports != null && ports.Length > 0)
        {
            for (int i = 0; i < ports.Length; i++)
            {
                if (GUI.Button(new Rect(buttonPosx, buttonPosy + (i + 1) * buttonSpacing, buttonSizex, buttonSizey), ports[i]))
                {
                    ArduinoManager.instance.port = ports[i];
                    ArduinoManager.instance.Init();
                }
            }

        }
    }
}

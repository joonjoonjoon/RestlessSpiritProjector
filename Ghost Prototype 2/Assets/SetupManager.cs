using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoSingleton<SetupManager> {

    private int debugModeID;

    void Awake()
    {
        instance = this;
        debugModeID = -1;
        ShowNextScreen();
    }

    void Update () {
		if(Input.GetKeyDown(KeyCode.D))
        {
            ShowNextScreen();
        }
	}

    void ShowNextScreen()
    {
        debugModeID++;
        if (debugModeID >= transform.childCount)
        {
            debugModeID = 0;
        }

        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
        transform.GetChild(debugModeID).gameObject.SetActive(true);
    }
}

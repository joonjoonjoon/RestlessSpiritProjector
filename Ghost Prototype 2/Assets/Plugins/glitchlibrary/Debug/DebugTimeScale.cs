using UnityEngine;
using System.Collections;

public class DebugTimeScale : MonoBehaviour {

    [Range(0,20)]
    public float timeScale;
    public bool on;

	void Start () {
	    //Time.timeScale = timeScale;
	}
	
	void Update () {
	    if(on)
        {
            Time.timeScale = timeScale;
        }
	}
}

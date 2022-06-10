using UnityEngine;
using System.Collections;
using Holoville.HOTween;


public class fadeFrom : MonoBehaviour {
    public Color from;
    public float time;
	// Use this for initialization
	void Start () {
        HOTween.From(GetComponent<Renderer>().material, time, "color", from);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

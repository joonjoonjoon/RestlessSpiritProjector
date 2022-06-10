using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbNumberThing : MonoBehaviour {

    private TextMesh text;
    private Vector3 offset;
    private GhostBehaviour ghost;
    public bool mute;
	void Start () {
        text = GetComponent<TextMesh>();
        ghost = GetComponentInParent<GhostBehaviour>();
        offset = transform.parent.position - transform.position;
	}
	
	void Update () {
        transform.position = transform.parent.position + offset;
        transform.rotation = Quaternion.identity;
        if (!mute)
            text.text = ghost.currentID + "";
        else
            text.text = "";
	}
}

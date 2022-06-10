using UnityEngine;
using System.Collections;

public class copyPosition : MonoBehaviour {
    public Transform target;
	
	void Update () {
        transform.position = target.position;
             
	}
}

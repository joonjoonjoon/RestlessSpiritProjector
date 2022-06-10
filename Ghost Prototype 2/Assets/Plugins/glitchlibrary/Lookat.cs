using UnityEngine;
using System.Collections;

public class Lookat : MonoBehaviour {
    public Transform target;
	void Update () {
        transform.LookAt(target);
	
	}
}

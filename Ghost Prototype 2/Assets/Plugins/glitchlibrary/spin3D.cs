using UnityEngine;
using System.Collections;

public class spin3D : MonoBehaviour {
    public Vector3 speed;

	void Update () {
        transform.rotation = Quaternion.Euler(transform.eulerAngles + speed * Time.deltaTime);
	}
}

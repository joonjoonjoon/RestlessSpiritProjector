using UnityEngine;
using System.Collections;

public class fakeGravity : MonoBehaviour {
    public Vector3 gravity;
    private Rigidbody rb;
    private Rigidbody2D rb2d;

    void Start () {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb2d = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
        if (rb != null)
            rb.AddForce(gravity);
        else if (rb2d != null)
            rb2d.AddForce(new Vector2(gravity.x, gravity.y));
	}
}

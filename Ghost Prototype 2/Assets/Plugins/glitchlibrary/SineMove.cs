using UnityEngine;
using System.Collections;

public class SineMove : MonoBehaviour {

    public float frequency;
    public float amplitude;
    public Vector3 axis;
    public bool on;
    private Vector3 startPos;
    private float timer;
    public bool local;
    public bool abs;
    public bool randomOffset;
    float offset = 0;
	void Start () {
        if (randomOffset) offset = Random.value * frequency;
        if (local)
        {
            startPos = transform.localPosition;
        }
        else
        {
            startPos = transform.position;
        }
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
	    if(on)
        {
            timer += Time.deltaTime;
            float val = Mathf.Sin(timer * frequency + offset) * amplitude;
            if (abs) val = Mathf.Abs(val);
            if(local)
            {

                if (axis.x != 0)
                    transform.localPosition = transform.localPosition.withX(startPos.x + val);
                if (axis.y != 0)
                    transform.localPosition = transform.localPosition.withY(startPos.y + val);
                if (axis.z != 0)
                    transform.localPosition = transform.localPosition.withZ(startPos.z + val);
            }
            else 
            {
            
                if (axis.x != 0)
                    transform.position = transform.position.withX(startPos.x + val);
                if (axis.y != 0)
                    transform.position = transform.position.withY(startPos.y + val);
                if (axis.z != 0)
                    transform.position = transform.position.withZ(startPos.z + val);
            }
        }
	}
}

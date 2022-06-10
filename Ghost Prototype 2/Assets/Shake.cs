using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoSingleton<Shake> {

    private FakeTransform startvals;
    public float scale = 1;
    public float shakeAmount =0;

    void Awake()
    {
        instance = this;
    }

    void Start () {
        startvals = transform.ToFakeTransform(true);	
	}
	
	void Update () {
        var offset = Random.insideUnitCircle * scale * shakeAmount;
        transform.localPosition = startvals.position + offset.ToVector3().withZ(0);
	}
}

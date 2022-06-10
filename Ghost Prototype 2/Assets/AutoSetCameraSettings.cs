using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSetCameraSettings : MonoSingleton<AutoSetCameraSettings> {

    [AutoStoredValueManager.StoredValue]
    public float ghostsInTankSizeVal;

    private Camera cam;

    void Awake()
    {
        instance = this;
        cam = GetComponent<Camera>();
    }

	void Start () {
        
    }

    void Update()
    {
        if (ghostsInTankSizeVal <= 0) ghostsInTankSizeVal = 1;
        cam.orthographicSize = ghostsInTankSizeVal;
    }



}

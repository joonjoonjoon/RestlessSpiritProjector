using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

[System.Serializable]
public struct FakeTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public FakeTransform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }

    public FakeTransform(Transform transform, bool local=false)
    {
        if (local) 
        {
            this.position = transform.localPosition;
            this.rotation = transform.localRotation;
            this.scale = transform.localScale;
        }
        else
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
            this.scale = transform.localScale;
        }
    }
}
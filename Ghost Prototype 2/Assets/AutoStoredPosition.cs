using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ScriptOrder(-100)]
public class AutoStoredPosition : MonoBehaviour {

    public static string GetPath(Transform transform)
    {
        var path = ":";
        while(transform != null)
        {
            path = transform.name + ":" + path;
            transform = transform.parent;
        }
        return path;
    }

    private string path;
    public bool position;
    public bool rotation;
    public bool scale;
    private FakeTransform defaultValues;

    void Start () {
        defaultValues = transform.ToFakeTransform(true);
        path = GetPath(transform);
        if (position)
        {
            var posx = ReadValue("posx", transform.position.x);
            var posy = ReadValue("posy", transform.position.y);
            var posz = ReadValue("posz", transform.position.z);
            transform.position = new Vector3(posx, posy, posz);
        }
        if (rotation)
        {
            var rotx = ReadValue("rotx", transform.rotation.x);
            var roty = ReadValue("roty", transform.rotation.y);
            var rotz = ReadValue("rotz", transform.rotation.z);
            var rotw = ReadValue("rotw", transform.rotation.w);
            transform.rotation = new Quaternion(rotx, roty, rotz, rotw);
        }
        if (scale)
        {
            var scalex = ReadValue("scalex", transform.localScale.x);
            var scaley = ReadValue("scaley", transform.localScale.y);
            var scalez = ReadValue("scalez", transform.localScale.z);
            transform.localScale = new Vector3(scalex, scaley, scalez);
        }
    }

    internal void ResetToDefault()
    {
        transform.FromFakeTransform(defaultValues, true);
    }

    float ReadValue(string id, float fallback)
    {
        if (PlayerPrefs.HasKey(path + id)) return PlayerPrefs.GetFloat(path + id);
        return fallback;
    }

    void StoreValue(string id, float value)
    {
        PlayerPrefs.SetFloat(path + id, value);
    }


    void OnDestroy()
    {
        if (position)
        {
            StoreValue("posx", transform.position.x);
            StoreValue("posy", transform.position.y);
            StoreValue("posz", transform.position.z);
        }
        if (rotation)
        {
            StoreValue("rotx", transform.rotation.x);
            StoreValue("roty", transform.rotation.y);
            StoreValue("rotz", transform.rotation.z);
            StoreValue("rotw", transform.rotation.w);
        }
        if (scale)
        {
            StoreValue("scalex", transform.localScale.x);
            StoreValue("scaley", transform.localScale.y);
            StoreValue("scalez", transform.localScale.z);
        }
    }
}

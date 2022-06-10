using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GlitchTweakSliderEntry
{
    public UnityEngine.Object obj;
    public string propertyOrField;
    public float min;
    public float max;
    public float lastval;
    public string name;
    public int size;
    public System.Type type;

    public GlitchTweakSliderEntry(UnityEngine.Object obj, string propertyOrField, float min, float max, int size)
    {
        this.obj = obj;
        this.type = obj.GetType();
        this.propertyOrField = propertyOrField;
        this.min = min;
        this.max = max;
        this.size = size;
        this.name = obj.name + ":" + propertyOrField;  
    }

    public GlitchTweakSliderEntry(System.Type type, string propertyOrField, float min, float max, int size)
    {
        this.type = type;
        this.propertyOrField = propertyOrField;
        this.min = min;
        this.max = max;
        this.size = size;
        this.name = type.Name + ":" + propertyOrField;  
    }
}
public class GlitchTweakSliderController : MonoBehaviour
{
    public static GlitchTweakSliderController instance;

    private static List<GlitchTweakSliderEntry> entries;
    private static float width = 60;
    private static float height = 20;
    private static float spacing = 10;
    private static GUIStyle style;

    private static void Init()
    {
        GameObject go = new GameObject("GlitchTweakSliders");
        instance = go.AddComponent<GlitchTweakSliderController>();
        entries = new List<GlitchTweakSliderEntry>();
        style = new GUIStyle();
        style.alignment = TextAnchor.MiddleLeft;
        style.normal.textColor = Color.black;
    }

	/// <summary>
	/// GlitchTweakSliders.Add(typeof(Time),"timeScale", 0 , 10);
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="propName">Property name.</param>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Max.</param>
	/// <param name="size">Size.</param>
	public static void Add(Type type, string propName, float min, float max, int size = 1)
	{
		if (instance == null)
		{
			Init();
		}
		entries.Add(new GlitchTweakSliderEntry(type, propName, min, max, size));
	}

	/// <summary>
	/// GlitchTweakSlider.Add(segwayTransform.getComponent<rigidbody>(), "drag", 0, 10);
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="propName">Property name.</param>
	/// <param name="min">Minimum.</param>
	/// <param name="max">Max.</param>
	/// <param name="size">Size.</param>
    public static void Add(UnityEngine.Object obj, string propName, float min, float max, int size=1) 
    {
        if (instance == null)
        {
            Init();
        }

        var newEntry = new GlitchTweakSliderEntry(obj, propName, min, max, size);

        //if max is not set
        if(max == -1)
        {
            newEntry.max = (float)GetValueFromEntry(newEntry) * 2;
        }

        entries.Add(newEntry);
    }

    public static void Clear()
    {
        if(entries != null) entries.Clear();
    }



    void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one * Screen.width / 250);


		if (entries == null || !Debug.isDebugBuild) return;
        for (int i = 0; i < entries.Count; i++)
		{
            // get value
            float temp = (float)GetValueFromEntry(entries[i]);
            if(float.IsNaN(temp))
            {
                Debug.Log("couldn't find '" + entries[i].propertyOrField + "', removing.");
                entries.Remove(entries[i]);
                return;
            }

            // Draw slider
            temp = GUI.HorizontalSlider(
                new Rect(
                    spacing, 
                    (spacing + height) * i + spacing, 
                    (width * entries[i].size),
                    height
                ), 
                temp, entries[i].min, entries[i].max);
            //GUI.Label(new Rect(Screen.width * 1/scale - spacing - width, (spacing + height) * i + spacing * 0.5f, width, height), temp + "", style);
            GUI.Label(new Rect(spacing, (spacing + height) * i + spacing * 1.5f, width, height), (entries[i].obj != null ? entries[i].obj.name : entries[i].type.Name) + ":" + entries[i].propertyOrField + " = " + temp , style);
            entries[i].lastval = temp;
            // Set Value
            SetValueFromEntry(entries[i], temp);
		}
    }

    void OnDisable()
    {
        if (entries == null) return;
        Debug.Log("<color=red>GlitchTweakSlider destroyed, dumping values:</color>");
        for (int i = 0; i < entries.Count; i++)
        {
            Debug.Log("+ " + entries[i].name + " = " + entries[i].lastval);
        }
    }

    public static object GetValueFromEntry(GlitchTweakSliderEntry entry)
    {
        try
        {
            return GetPropValue(entry.type, entry.obj, entry.propertyOrField);
        }
        catch (System.NullReferenceException)
        {
            try
            {
                return (float)GetFieldValue(entry.type, entry.obj, entry.propertyOrField);
            }
            catch (System.Exception)
            {
                return float.NaN;
            }
        }
    }

    public static void SetValueFromEntry(GlitchTweakSliderEntry entry, object value)
    {
        try
        {
            SetPropValue(entry.type, entry.obj, entry.propertyOrField, value);
        }
        catch (System.NullReferenceException)
        {
            try
            {
                SetFieldValue(entry.type, entry.obj, entry.propertyOrField, value);
            }
            catch (System.Exception)
            {
            }
        }
    }



    public static object GetPropValue(Type type, object obj, string propName)
    {
		if (Debug.isDebugBuild)
        	return type.GetProperty(propName).GetValue(obj, null);
		return float.NaN;
    }

    public static void SetPropValue(Type type, object obj, string propName, object value)
    {
		if (Debug.isDebugBuild)
        	type.GetProperty(propName).SetValue(obj, value, new object[] { });
    }

    public static object GetFieldValue(Type type, object obj, string propName)
    {
		if (Debug.isDebugBuild)
        	return type.GetField(propName).GetValue(obj);
		return float.NaN;
    }

    public static void SetFieldValue(Type type, object obj, string propName, object value)
    {
		if (Debug.isDebugBuild)
        	type.GetField(propName).SetValue(obj, value);
    }

}


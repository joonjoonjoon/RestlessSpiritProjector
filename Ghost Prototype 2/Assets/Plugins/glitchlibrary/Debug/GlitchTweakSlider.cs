using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using  System.Reflection;

public class GlitchTweakSlider : MonoBehaviour
{
    [SerializeField]
    public List<GlitchTweakSliderDictionaryItem> sliders;


    void Start()
    {
        foreach (var item in sliders)
        {
            foreach (var field in item.fields)
            {
                GlitchTweakSliderController.Add(item.component, field,0,-1,3);
            }
        }
    }

    void OnDisable()
    {
        GlitchTweakSliderController.Clear();
    }

    public void Reset()
    {
        GlitchTweakSliderController.Clear();
        foreach (var item in sliders)
        {
            foreach (var field in item.fields)
            {
                GlitchTweakSliderController.Add(item.component, field, 0, -1, 3);
            }
        }
    }

    public bool Contains(Component component, string field)
    {
        foreach (var item in sliders)
	    {
            if (item.component == component)
             {
                 return item.fields.Contains(field);
             }
	    }
        return false;
    }

    public void Add(Component component,FieldInfo field)
    {
        if (sliders == null) sliders = new List<GlitchTweakSliderDictionaryItem>();
        foreach (var item in sliders)
        {
            if (item.component == component)
            {
                item.fields.Add(field.Name);
                return;
            }
        }

        sliders.Add(new GlitchTweakSliderDictionaryItem(component, new List<string> { field.Name }));
    }

    public void Remove(Component component, System.Reflection.FieldInfo field)
    {
        if (sliders == null) return;
        foreach (var item in sliders)
        {
            if (item.component == component)
            {
                item.fields.Remove(field.Name);
            }
        }
    }
}

[System.Serializable]
public class GlitchTweakSliderDictionaryItem
{
    public Component component;
    public List<string> fields;
    public GlitchTweakSliderDictionaryItem(Component component, List<string> fields)
    {
        this.component = component; 
        this.fields = fields;
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GlitchTweakSlider))]
public class GlitchTweakSliderEditor : Editor {

    public override void OnInspectorGUI()
    {
        GlitchTweakSlider slider = (GlitchTweakSlider)target;
        if (slider.sliders == null) slider.sliders = new List<GlitchTweakSliderDictionaryItem>();

        /*
        // Display active sliders
        foreach (var sliderComponent in slider.sliders)
        {
            foreach (var item in sliderComponent.fields)
            {
                EditorGUILayout.LabelField((sliderComponent.component).gameObject.name + ":" + sliderComponent.component.name + ":" + item);
            }
        }

        
        EditorGUILayout.LabelField("________________________________________");
         */

        // find all components on game object
        foreach (var component in slider.gameObject.GetComponents(typeof(Component)))
        {
            if(component.GetType() == typeof(Transform) || component.GetType() == typeof(GlitchTweakSlider)) continue;

            // show component name
            EditorGUILayout.LabelField(component.GetType().Name, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            // display all floats per component
            foreach (var item in component.GetType().GetFields())
            {
                if (item.FieldType == typeof(float))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(item.Name);

                    if (slider.Contains(component, item.Name))
                    {
                        bool toggle = EditorGUILayout.Toggle(true);
                        if (!toggle)
                        {
                            slider.Remove(component, item);
                            if (Application.isPlaying) slider.Reset();
                        }
                    }
                    else
                    {
                        bool toggle = EditorGUILayout.Toggle(false);
                        if (toggle)
                        {
                            slider.Add(component, item);
                            if (Application.isPlaying) slider.Reset();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(slider.gameObject);
        }
    }

}

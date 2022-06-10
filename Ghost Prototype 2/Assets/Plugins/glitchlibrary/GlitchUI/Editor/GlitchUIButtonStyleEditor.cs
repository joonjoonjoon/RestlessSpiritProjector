using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlitchUIButtonStyle))]
[CanEditMultipleObjects]
public class GlitchUIButtonStyleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        var changed = EditorGUI.EndChangeCheck();

        if (changed)
        {
            var instance = (target as GlitchUIButtonStyle);
            RefreshStyle(instance);
        }
       
    }

    private static void RefreshStyle(GlitchUIButtonStyle instance)
    {
        var actual = instance.GetComponent<GlitchUIButton>();
        var style = instance.buttonStyle;

        actual.darkenBackgroundPercentage = style.darkenBackgroundPercentage;
        actual.darkenWhenPressedPercentage= style.darkenWhenPressedPercentage;
        actual.heightPercentage = style.heightPercentage;
        actual.color = style.color;
        actual.pressPercentage = style.pressPercentage;
        actual.textStyle = style.textStyle;

        var text = instance.GetComponentInChildren<GlitchUITmproStyle>();
        if (actual.textStyle != null)
        {
            text.style = actual.textStyle;
            GlitchUITmproStyleEditor.RefreshAllStyles();
        }

        EditorUtility.SetDirty(instance);
    }

    [MenuItem("GlitchLibrary/GlitchUI/Refresh Button Styles")]
    public static void RefreshAllStyles()
    {
        // Update all Styles
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var style in root.GetComponentsInChildren<GlitchUIButtonStyle>(true))
            {
                RefreshStyle(style);
                style.GetComponent<GlitchUIButton>().RefreshStyle();
                EditorUtility.SetDirty(style);
            }
        }
        GlitchUITmproStyleEditor.RefreshAllStyles();

        DebugExtension.Blip(null,"repainted allegedly");

        
    }
}

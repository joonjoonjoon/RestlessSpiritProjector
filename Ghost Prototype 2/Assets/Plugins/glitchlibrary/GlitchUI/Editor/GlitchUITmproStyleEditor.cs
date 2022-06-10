using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlitchUITmproStyle))]
[CanEditMultipleObjects]
public class GlitchUITmproStyleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        var changed = EditorGUI.EndChangeCheck();

        if (changed)
        {
            var instance = (target as GlitchUITmproStyle);
            RefreshStyle(instance);
        }
    }

    private static void RefreshStyle(GlitchUITmproStyle instance)
    {
        var actual = instance.GetComponent<TMPro.TextMeshProUGUI>();

        if (instance.style == null)
        {
            Debug.LogWarning("Text without style attached found", instance);   
            return;
        }

        actual.font = instance.style.font;
        actual.fontStyle = instance.style.fontStyle;
        actual.color = instance.style.color;
        actual.alpha = instance.style.alpha;
        actual.colorGradient = instance.style.colorGradient;
        actual.overrideColorTags = instance.style.overrideColorTags;
        actual.fontSize = instance.style.fontSize;
        actual.enableAutoSizing = instance.style.enableAutoSizing;
        actual.fontSizeMax = instance.style.fontSizeMax;
        actual.fontSizeMin = instance.style.fontSizeMin;
        actual.characterSpacing = instance.style.characterSpacing;
        actual.lineSpacing = instance.style.lineSpacing;
        actual.paragraphSpacing = instance.style.paragraphSpacing;
        actual.alignment = instance.style.alignment;
        actual.enableWordWrapping = instance.style.enableWordWrapping;
        actual.OverflowMode = instance.style.OverflowMode;
        actual.horizontalMapping = instance.style.horizontalMapping;
        actual.verticalMapping = instance.style.verticalMapping;
        actual.maskOffset = instance.style.maskOffset;
        actual.margin = instance.style.margin;
        actual.enableKerning = instance.style.enableKerning;
        actual.autoSizeTextContainer = instance.style.autoSizeTextContainer;
        actual.characterSpacing = instance.style.characterSpacing;
        actual.characterWidthAdjustment = instance.style.characterWidthAdjustment;
        actual.enableVertexGradient = instance.style.enableVertexGradient;
        actual.extraPadding = instance.style.extraPadding;
        actual.richText = instance.style.richText;
        actual.raycastTarget = instance.style.raycastTarget;
    }

    [MenuItem("GlitchLibrary/GlitchUI/Refresh Text Styles")]
    public static void RefreshAllStyles()
    {
        // Update all Styles
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var style in root.GetComponentsInChildren<GlitchUITmproStyle>(true))
            {
                RefreshStyle(style);
            }
        }
    }
}

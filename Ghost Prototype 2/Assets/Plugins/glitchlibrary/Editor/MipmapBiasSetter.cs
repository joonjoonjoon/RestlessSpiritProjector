using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class MipmapBiasSetter : EditorWindow
{
    Object[] myObjs;
    float bias;
    bool biasMixed;
    System.Action action;
    

    // Add menu named "My Window" to the Window menu
    [MenuItem("GlitchLibrary/Mipmap Bias Setter")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MipmapBiasSetter window = (MipmapBiasSetter)EditorWindow.GetWindow(typeof(MipmapBiasSetter));
        window.titleContent.text = "Mipmap Bias";

        window.Show();

        Selection.selectionChanged += window.SelectionChanged;
        window.action = Selection.selectionChanged;

        window.SelectionChanged();
    }


    void OnGUI()
    {
        if(Selection.selectionChanged != action)
        {
            Selection.selectionChanged += SelectionChanged;
            action = Selection.selectionChanged;
        }


        if (myObjs != null && myObjs.Length > 0)
        {
            biasMixed = false;
            var tempbias = 0f;
            for (int i = 0; i < myObjs.Length; i++)
            {
                if (i == 0) tempbias = ((Texture)myObjs[i]).mipMapBias;
                else if (((Texture)myObjs[i]).mipMapBias != tempbias) biasMixed = true;
            }

            EditorGUI.showMixedValue = biasMixed;
            var biastemp = bias;
            bias = EditorGUILayout.FloatField("Mip Map Bias", bias);
            if (biastemp != bias) biasMixed = false;
            EditorGUI.showMixedValue = false;

            foreach (var item in myObjs)
            {
                if (!biasMixed)
                {
                    SetBias((Texture)item, bias);
                }
            }
        }

        if(GUILayout.Button("APPLY"))
        {
            foreach (var item in myObjs)
            {
                if (!biasMixed)
                {
                    ApplyBias((Texture)item);
                }
            }
        }

        if(myObjs == null || myObjs.Length == 0)
        {
            GUILayout.Label("Set bias: no textures selected", EditorStyles.boldLabel);
        }
        else if(myObjs.Length == 1) 
        {
            GUILayout.Label("Set bias: " + myObjs[0].name, EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(myObjs[0].name, myObjs[0], typeof(Texture), false);
        }
        else
        {
            GUILayout.Label("Set bias: multiple textures (" + myObjs.Length+")", EditorStyles.boldLabel);

            foreach (var obj in myObjs)
	        {
		        EditorGUILayout.ObjectField(obj.name, obj,typeof(Texture), false);
	        }
        }
    }

    void SetBias(Texture text, float bias)
    {
        text.mipMapBias = bias;
    }

    void ApplyBias(Texture text)
    {
        var bias = text.mipMapBias;
        string path = AssetDatabase.GetAssetPath(text);
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.mipMapBias = bias;
        AssetDatabase.ImportAsset(path);
    }

    void SelectionChanged()
    {
        var textures = Selection.GetFiltered(typeof(Texture), SelectionMode.Unfiltered);
        myObjs = new Object[textures.Length];

        biasMixed = false;
        var tempbias = 0f;
        for (int i = 0; i < textures.Length; i++)
		{
            if (i == 0) tempbias = ((Texture)textures[i]).mipMapBias;
            else if (((Texture)textures[i]).mipMapBias != tempbias) biasMixed = true;
			myObjs[i] = textures[i];
		}

        if (!biasMixed) bias = tempbias;

        Repaint();
    }
}
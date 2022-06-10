using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MassApplyAudioImport : EditorWindow
{
    private string sourceClipPath;
    private string folderPath;

    private List<string> audioClipPaths;

    [MenuItem("GlitchLibrary/Mass Apply Audio Import Settings")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        MassApplyAudioImport window = (MassApplyAudioImport)EditorWindow.GetWindow(typeof(MassApplyAudioImport));
        window.titleContent.text = "Mass audio import setting setter";
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy settings from this file:", EditorStyles.boldLabel);
        sourceClipPath = EditorGUILayout.TextField(sourceClipPath);
        GUILayout.Label("To all Audioclips in this folder (don't include Assets\\):", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField(folderPath);

        if (GUILayout.Button("APPLY"))
        {
            ApplySettings();
        }
    }

    private void ApplySettings()
    {
        //var path = Path.Combine(Application.dataPath, folderPath);
        //Debug.Log(path);
        audioClipPaths = new List<string>();
        recursiveDirCrawl(folderPath);

        var sourceImporter = AssetImporter.GetAtPath(Path.Combine("Assets", sourceClipPath)) as AudioImporter;

        foreach (var audioClipPath in audioClipPaths)
        {
            var audioClipImporter = AssetImporter.GetAtPath(audioClipPath) as AudioImporter;

            audioClipImporter.forceToMono = sourceImporter.forceToMono;
            audioClipImporter.loadInBackground = sourceImporter.loadInBackground;
            audioClipImporter.defaultSampleSettings = sourceImporter.defaultSampleSettings;
            audioClipImporter.preloadAudioData = sourceImporter.preloadAudioData;
                
            Debug.Log("copied settings into " + audioClipPath);

            audioClipImporter.SaveAndReimport();
        }
    }

    private void recursiveDirCrawl(string path)
    {
        var dir = new DirectoryInfo(Path.Combine(Application.dataPath, path));
        
        foreach (var file in dir.GetFiles())
        {
            var filePath = Path.Combine("Assets", Path.Combine(path, file.Name));
            var audioClip = AssetDatabase.LoadAssetAtPath(filePath, typeof(AudioClip));
            if (audioClip != null)
            {
                if (audioClipPaths == null) audioClipPaths = new List<string>();
                audioClipPaths.Add(filePath);
            }
        }

        foreach (DirectoryInfo subDir in dir.GetDirectories())
        {
            recursiveDirCrawl(Path.Combine(path, subDir.Name));
        }
    }
}
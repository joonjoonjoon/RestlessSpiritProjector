using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

public class ScriptCreator : Editor
{
    public static string templatePath = Path.Combine(ProjectWindowUtil.GetContainingFolder(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("ScriptCreator")[0])), "scripttemplates");
    public static string singletonScriptFilename = "81-C# Singleton-NewSingletonScript.cs.txt";
    public static string editorScriptFilename = "81-C# Editor-NewEditorScript.cs.txt";
    public static string interfaceScriptFilename = "81-C# Interface-NewInterfaceScript.cs.txt";
    public static string extensionScriptFilename = "81-C# Extension-NewExtensionScript.cs.txt";
    public static string propertyDrawerScriptFilename = "81-C# PropertyDrawer-NewPropertyDrawerScript.cs.txt";

    [MenuItem("Assets/Create/C# Singleton", false, 81)]
    public static void CreateSingletonScript()
    {
        CreateScriptAsset(Path.Combine(templatePath, singletonScriptFilename), Path.Combine(GetActiveFolderPath(), "NewSingletonScript.cs"));
    }

    [MenuItem("Assets/Create/C# EditorScript", false, 81)]
    public static void CreateEditorScript()
    {
        CreateScriptAsset(Path.Combine(templatePath, editorScriptFilename), Path.Combine(GetActiveFolderPath(), "NewEditorScript.cs"));
    }

    [MenuItem("Assets/Create/C# Interface", false, 81)]
    public static void CreateInterfaceScript()
    {
        CreateScriptAsset(Path.Combine(templatePath, interfaceScriptFilename), Path.Combine(GetActiveFolderPath(), "NewInterfaceScript.cs"));
    }

    [MenuItem("Assets/Create/C# Extension", false, 81)]
    public static void CreateExtensionScript()
    {
        CreateScriptAsset(Path.Combine(templatePath, extensionScriptFilename), Path.Combine(GetActiveFolderPath(), "NewExtensionScript.cs"));
    }

    [MenuItem("Assets/Create/C# PropertyDrawer", false, 81)]
    public static void CreatePropertyDrawerScript()
    {
        CreateScriptAsset(Path.Combine(templatePath, propertyDrawerScriptFilename), Path.Combine(GetActiveFolderPath(), "NewPropertyDrawerScript.cs"));
    }

    private static void CreateScriptAsset(string templatePath, string destName)
    {
        string extension = Path.GetExtension(destName);
        Texture2D icon;
        switch (extension)
        {
            case ".cs":
                icon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
                goto IL_105;
            case ".shader":
                icon = (EditorGUIUtility.IconContent("Shader Icon").image as Texture2D);
                goto IL_105;
        }
        icon = (EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D);
        IL_105:
        var scriptableObject = ScriptableObject.CreateInstance<DoCreateScriptAsset>();
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, scriptableObject, destName, icon, templatePath);
    }

    internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
        string fullPath = Path.GetFullPath(pathName);
        StreamReader streamReader = new StreamReader(resourceFile);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
        string scriptName = Regex.Replace(fileNameWithoutExtension, " ", string.Empty);
        text = Regex.Replace(text, "#SCRIPTNAME#", scriptName);
        string text3 = Regex.Replace(scriptName, "Editor", string.Empty);
        text = Regex.Replace(text, "#EDITNAME#", text3);
        string text4 = Regex.Replace(scriptName, "Extension", string.Empty);
        text = Regex.Replace(text, "#EXTENDEENAME#", text4);
        string text5 = Regex.Replace(scriptName, "Drawer", string.Empty);
        text = Regex.Replace(text, "#DRAWERNAME#", text5);

        if (char.IsUpper(scriptName, 0))
        {
            scriptName = char.ToLower(scriptName[0]) + scriptName.Substring(1);
            text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", scriptName);
        }
        else
        {
            scriptName = "my" + char.ToUpper(scriptName[0]) + scriptName.Substring(1);
            text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", scriptName);
        }
        bool encoderShouldEmitUTF8Identifier = true;
        bool throwOnInvalidBytes = false;
        UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }

    internal static string GetActiveFolderPath()
    {
        if (Selection.activeObject != null)
        {
            var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (AssetDatabase.IsValidFolder(selectedPath))
            {
                return selectedPath;
            }

            selectedPath = ProjectWindowUtil.GetContainingFolder(selectedPath);

            if (AssetDatabase.IsValidFolder(selectedPath))
            {
                return selectedPath;
            }
        }
        
        return "Assets";
    }
}

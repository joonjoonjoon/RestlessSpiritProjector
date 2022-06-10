using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class AssetBundleBuilder : ScriptableWizard {

    [MenuItem("Assets/Get AssetBundle names",false, 134)]
	static void GetNames ()
	{
		var names = AssetDatabase.GetAllAssetBundleNames();
		foreach (var name in names)
			Debug.Log ("AssetBundle: " + name);
	}

    [MenuItem("Assets/Build AssetBundles", false, 134)]
	static void BuildAllAssetBundles ()
	{
		//BuildPipeline.BuildAssetBundles ("Assets/StreamingAssets", BuildAssetBundleOptions.None, BuildTarget.Android);
		BuildPipeline.BuildAssetBundles ("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.iOS);
	}

	[MenuItem("GlitchLibrary/~ResetFontVersion",false, 134)]
	static void ResetFontVersion()
	{
		PlayerPrefs.SetInt("fontVersion", 0);
		PlayerPrefs.Save();
	}
}

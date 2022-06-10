using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;


public class CloudbuildVersion : MonoBehaviour {

    public TMPro.TextMeshProUGUI textbox;
    public string prefix = "Cloudbuild Version";
    public bool dumpManifest=false;
	void Start ()
	{
        textbox.text = prefix + " " + Application.version;

#if  UNITY_CLOUD_BUILD
        Debug.Log ("Checking Cloudbuild Manifest");
        
        object cloudbuildVersionValue = null;

        var manifest = (TextAsset) Resources.Load("UnityCloudBuildManifest.json");
        if (manifest != null)
        {
            var manifestDict = Json.Deserialize(manifest.text) as Dictionary<string,object>;

            if(dumpManifest)
            {
                foreach (var kvp in manifestDict)
                {
                    // Be sure to check for null values!
                    var value = (kvp.Value != null) ? kvp.Value.ToString() : "";
                    textbox.text += (string.Format("Key: {0}, Value: {1}", kvp.Key, value)) + "\n";
                }
            }
            else
            {
                manifestDict.TryGetValue("buildNumber",out cloudbuildVersionValue);
                if (cloudbuildVersionValue!=null)
                {
                    textbox.text = prefix + " " + cloudbuildVersionValue.ToString();    
                }   
            }   
        }
#endif
    }
}

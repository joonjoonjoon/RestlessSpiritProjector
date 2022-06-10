using UnityEngine;
using UnityEditor;
using System.Collections;


[InitializeOnLoad]
public class GlitchCompileTimeAnalyzer {

    static GlitchCompileTimeAnalyzer()
    {
        EditorApplication.update += Update;
        EditorApplication.playmodeStateChanged += StateChanged;
    }

    private static void StateChanged()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (EditorApplication.isPlaying)
            {
                var timeSince = EditorApplication.timeSinceStartup - EditorPrefs.GetFloat("recordedTime"); 
                Debug.Log("[analtime] Took " + GetTimeString(timeSince) + " to enter Play");
            }
        } 
    }
     
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void DidReloadScripts()
    {
        var timeSince = EditorApplication.timeSinceStartup - EditorPrefs.GetFloat("recordedTime");
        Debug.Log("[analtime] Took " + GetTimeString(timeSince) + " to Compile");
    } 

    private static void Update()
    {
        if(!EditorApplication.isCompiling && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // I couldn't figure out how to store stuff otherwise :/
            EditorPrefs.SetFloat("recordedTime", (float)EditorApplication.timeSinceStartup);
        }  
    }  

     
    private static string GetTimeString(double time)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(time);

        if(t.Minutes > 0)
            return string.Format("{0:D2}m:{1:D2}s",t.Minutes,t.Seconds);
        else
            return string.Format("{0:D2}s:{1:D3}ms", t.Seconds, t.Milliseconds);
    }


}

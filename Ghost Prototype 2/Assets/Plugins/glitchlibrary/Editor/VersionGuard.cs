using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class VersionGuard
{
    public static string targetVersion = "5.5.1f1";
    private static bool matchPartially = false;
    private static bool allowBeta = false;
    private static bool allowPatched = false;

    private static bool hasShownMatch;
    static VersionGuard()
    {
        if (Application.unityVersion != targetVersion)
        {
            if (matchPartially)
            {
                if(Application.unityVersion.Substring(0,targetVersion.Length) != targetVersion)
                {
                    if (!allowBeta && Application.unityVersion.Contains("b"))
                    {
                        Debug.Log("<color=red>VERSION MISMATCH:</color> you are on " + Application.unityVersion + " and Beta versions are not allowed.");
                    }
                    else if (!allowPatched && Application.unityVersion.Contains("p"))
                    {
                        Debug.Log("<color=red>VERSION MISMATCH:</color> you are on " + Application.unityVersion + " and Patch versions are not allowed.");
                    }
                    else
                    {
                        Debug.Log("<color=red>VERSION MISMATCH:</color> you are on " + Application.unityVersion + " and should be on " + targetVersion);
                    }
                }
            }
            else
            {
                Debug.Log("<color=red>VERSION MISMATCH:</color> you are on " + Application.unityVersion + " and should be on " + targetVersion);
            }
        }
        else
        {
            //Keep this disabled because it would be annoying, but good for you for being on the right version!
            //Debug.Log("<color=green>VERSION MATCH:</color> you are on " + Application.unityVersion + " and should be on " + targetVersion);
        }
        
    }
}
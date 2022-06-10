using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

#if UNITY_IOS && UNITY_CLOUD_BUILD
using UnityEditor.iOS.Xcode;
using System.IO;
#endif

public class CloudbuildDisableBitcode {

#if UNITY_IOS && UNITY_CLOUD_BUILD
    
    public static void OnPostprocessBuild(string path) 
    {
        string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        string nativeTarget = proj.TargetGuidByName(PBXProject.GetUnityTargetName());
        string testTarget = proj.TargetGuidByName(PBXProject.GetUnityTestTargetName());
        string[] buildTargets = new string[] { nativeTarget, testTarget };

        proj.SetBuildProperty(buildTargets, "ENABLE_BITCODE", "NO");
        File.WriteAllText(projPath, proj.WriteToString());
    }
#endif
}

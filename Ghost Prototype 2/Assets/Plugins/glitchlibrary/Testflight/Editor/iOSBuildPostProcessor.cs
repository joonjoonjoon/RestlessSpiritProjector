using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS && UNITY_CLOUD_BUILD
using UnityEditor.iOS.Xcode;
using System.IO;
#endif

// http://answers.unity3d.com/questions/1255132/why-does-apple-think-my-unity-project-is-trying-to.html#comment-1258517
public class iOSBuildPostProcessor
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // DISABLE CAMERA PERMISSION FOR iOS
        if (target == BuildTarget.iOS)
        {
             var targetfile = pathToBuiltProject + "/Classes/Preprocessor.h";
             var filecontents = System.IO.File.ReadAllText(targetfile);
             {
                 string seed = "#define UNITY_USES_WEBCAM 1"; // find this
                 string repl = "#define UNITY_USES_WEBCAM 0"; // replace with this
 
                 if (filecontents.Contains(seed))
                 {
                     Debug.Log("<b>iOSBuildPostProcessor</b> Removing faulty inclusion of CameraCapture");
                     filecontents = filecontents.Replace(seed, repl);
                 }
                 else
                 {
                     Debug.Log("Seed not found in file: " + seed);
                 }
             }
             System.IO.File.WriteAllText (targetfile, filecontents);
        }
    }


#if UNITY_IOS && UNITY_CLOUD_BUILD
    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

        if (buildTarget == BuildTarget.iOS) {
            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
       
            // Get root
            PlistElementDict rootDict = plist.root;
       
            // Change value of CFBundleVersion in Xcode plist
            var Key = "NSPhotoLibraryUsageDescription";
            var Value = "Save screenshots";
            rootDict.SetString(Key,Value);
       
            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
        
    }
#endif
}


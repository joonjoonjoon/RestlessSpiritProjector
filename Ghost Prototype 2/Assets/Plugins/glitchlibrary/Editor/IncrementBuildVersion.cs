/**
 * Copyright 2012 Calvin Rien
 * (http://the.darktable.com)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * https://gist.github.com/yourpalmark/6231952
 */


using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections.Generic;

public class IncrementBuildVersion : ScriptableObject
{
    [MenuItem("GlitchLibrary/Print Version Number", false, 99)]
    static void ShowVersion()
    {
        IncrementBuild(false, false, false, false);
    }

    [MenuItem("GlitchLibrary/Increment Build Version Number (_._.X)", false, 100)]
	static void IncrementBuild()
	{
        IncrementBuild(true, false, false, false);
    }

    [MenuItem("GlitchLibrary/Increment Minor Version Number (_.X._)", false, 101)]
    static void IncrementMinor()
    {
        IncrementBuild(false, true, false, false);
    }


    [MenuItem("GlitchLibrary/Increment Major Version Number (X._._)", false, 102)]
    static void IncrementMajor()
    {
        IncrementBuild(false, false, true, false);
    }


	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string buildPath)
	{
        // Increment Build for Android is called in BuildAndroid.sh

		//IncrementBuild(false, false, false, false);
	}
	
	public static void IncrementBuild(bool incBuild, bool incMinor, bool incMajor, bool incVersionCode)
	{
		var settingsPath = Path.GetDirectoryName(Application.dataPath);
		settingsPath = Path.Combine(settingsPath, "ProjectSettings");
		settingsPath = Path.Combine(settingsPath, "ProjectSettings.asset");
		
		if (!File.Exists(settingsPath))
		{
			Debug.LogError("Couldn't find project settings file.");
			return;
		}
		
		var lines = File.ReadAllLines(settingsPath);
		
		if (!lines[0].StartsWith("%YAML"))
		{
			Debug.LogError("Project settings file needs to be serialized as a text asset. (Check 'Project Settings->Editor')");
			return;
		}

        string pattern = "bundleVersion";
		string pattern2 = "AndroidBundleVersionCode";
		bool success = false;
		
		System.Version version = null;
		int versionCode = PlayerSettings.Android.bundleVersionCode;

        

		for (int i=0; i<lines.Length; i++)
		{
			var line = lines[i];

            if (line.Contains(pattern))
            {

                var match = line.Split(':');

                version = new System.Version(match[1].Split('#')[0]);

                var major = version.Major < 0 ? 0 : version.Major;
                var minor = version.Minor < 0 ? 0 : version.Minor;
                var build = version.Build < 0 ? 0 : version.Build;

                if (incMajor)
                {
                    major++;
                    minor = 0;
                    build = 0;
                }
                else if (incMinor)
                {
                    minor++;
                    build = 0;
                }
                else if (incBuild)
                {
                    build++;
                }

                version = new System.Version(major, minor, build);

                line = match[0] + ": " + version;

                lines[i] = line;
                success = true;
            }
			else if (line.Contains(pattern2))
			{
				var match = line.Split(':');
				if (incVersionCode)
				{
					versionCode++;
					line = match[0] + ": "+ versionCode;
					lines[i] = line;
				}
			}
		}

        PlayerSettings.bundleVersion = version.ToString();
        PlayerSettings.Android.bundleVersionCode = versionCode;

		if (!success)
		{
			Debug.Log("Couldn't find bundle version in ProjectSettings.asset");
			return;
		}
		
		File.WriteAllLines(settingsPath, lines);

		Debug.Log("Build version: " + version + " Version code: " + versionCode);
    }
}
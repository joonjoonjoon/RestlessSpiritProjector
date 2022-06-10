using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class ValueDebugger : EditorWindow 
{
	[MenuItem("GlitchLibrary/Value Debugger %1")]
	public static void Run()
	{
		EditorWindow.GetWindow<ValueDebugger>();
	}



	// Update is called once per frame
	void Update () 
	{
		if (EditorApplication.isPlaying && !EditorApplication.isPaused)
		{
			Repaint();
		}
	}

	void OnGUI()
	{
		if (EditorApplication.isPlaying) 
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Status: Running ");
			if (GUILayout.Button ("Clear")) 
			{
				DebugValues.Instance.GetValues().Clear();
			}
			GUILayout.EndHorizontal ();
		} 
		else 
		{
			GUILayout.Label ("Status: Waiting for editor playing");
			return;
		}

		foreach (var debugItem in DebugValues.Instance.GetValues()) 
		{
			//GUI.enabled = false;
			var value = debugItem.Value.Invoke();

			if(value == null)
			{
				EditorGUILayout.LabelField(debugItem.Key, "null");
				continue;
			}

			switch (value.GetType().ToString()) 
			{
			case "UnityEngine.Vector2":
				EditorGUILayout.Vector2Field(debugItem.Key, (Vector2)value);
				break;
			case "UnityEngine.Vector3":
				EditorGUILayout.Vector3Field(debugItem.Key, (Vector3)value);
				break;
			case "UnityEngine.Vector4":
				EditorGUILayout.Vector4Field(debugItem.Key, (Vector4)value);
				break;
			case "System.String":
				EditorGUILayout.LabelField(debugItem.Key, (string)value);
				break;
			case "System.Int32":
				EditorGUILayout.IntField(debugItem.Key, (int)value);
				break;
			case "System.Single":
				EditorGUILayout.FloatField(debugItem.Key, (float)value);
				break;
			case "System.Boolean":
				EditorGUILayout.Toggle(debugItem.Key, (bool)value);
				break;
			default:
				Debug.LogError("Value Debugger: object type not supported! "+value.GetType());
				Debug.Break();
				break;
			}

           	//GUI.enabled = true;
		}
	}
}

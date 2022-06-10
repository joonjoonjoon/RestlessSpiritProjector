using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoSingleton), editorForChildClasses: true)]
public class MonoSingletonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (target is MonoSingleton)
        {
            if (Application.isEditor && Application.isPlaying == false)
            {
                MonoSingleton singleton = target as MonoSingleton;    
                singleton.InstanceLivesInScene = true;
            }    
        }
    }
   
}

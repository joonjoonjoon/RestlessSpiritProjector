using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ScriptOrder(-1000)]
public class AutoStoredValueManager : MonoSingleton<AutoStoredValueManager>{

    List<MonoBehaviour> components;

    void Awake()
    {
        instance = this;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class StoredValueAttribute : Attribute
    {
        public StoredValueAttribute()
        {
        }
    }

    public static string GetPath(Transform transform)
    {
        var path = ":";
        while (transform != null)
        {
            path = transform.name + ":" + path;
            transform = transform.parent;
        }
        return path;
    }

    public static void Store(MonoBehaviour component)
    {
        var path = GetPath(component.transform);
        foreach (var field in component.GetType().GetFields())
        {
            var attributes = field.GetCustomAttributes(typeof(StoredValueAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                var id = path + field.Name;

                if (field.FieldType == typeof(float))
                {
                    PlayerPrefs.SetFloat(id, (float)field.GetValue(component));
                }
                else if (field.FieldType == typeof(string))
                {
                    PlayerPrefs.SetString(id, (string)field.GetValue(component));
                }
            }
        }
    }

    public static void Load(MonoBehaviour component)
    {
        var path = GetPath(component.transform);
        foreach (var field in component.GetType().GetFields())
        {
            var attributes = field.GetCustomAttributes(typeof(StoredValueAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                var id = path + field.Name;

                if (field.FieldType == typeof(float))
                {
                    var value = PlayerPrefs.GetFloat(id);
                    field.SetValue(component, value);
                }
                else if (field.FieldType == typeof(string))
                {
                    var value = PlayerPrefs.GetString(id);
                    field.SetValue(component, value);
                }
            }
        }
    }

    void Start()
    {
        components = new List<MonoBehaviour>();
        // scrub through list and load values
        var types = GetTypesWithAttribute();
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var type in types)
            {
                foreach (var component in root.GetComponentsInChildren(type, true))
                {
                    components.Add(component as MonoBehaviour);
                }
            }
        }

        var debugStr = "";
        foreach (var component in components)
        {
            Load(component);
            debugStr += component.name + ", ";
        }
        DebugExtension.Blip(this, "Auto loading: " + debugStr, Color.cyan);
    }


    bool quitting;

    void OnApplicationQuit()
    {
        quitting = true;
        ForceStore();
    }

    public static void ForceStore()
    {
        var debugStr = "";
        foreach (var component in instance.components)
        {
            Store(component);
            debugStr += component.name + ", ";
        }
        DebugExtension.Blip(instance, "Storing: " + debugStr, Color.cyan);
    }

    List<System.Type> GetTypesWithAttribute()
    {
        var types = new List<System.Type>();
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            foreach (Type type in assembly.GetTypes())
            {
                foreach (var field in type.GetFields())
                {
                    if (field.GetCustomAttributes(typeof(StoredValueAttribute), false).Length > 0)
                    {
                        if (!types.Contains(type))
                        {
                            types.Add(type);
                        }
                    }
                }
            }
        }

        return types;
    }
}

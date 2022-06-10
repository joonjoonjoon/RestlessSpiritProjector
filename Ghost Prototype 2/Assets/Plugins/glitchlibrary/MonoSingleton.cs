using UnityEngine;
using System.Collections;
using System;

public class MonoSingleton : MonoBehaviour
{
    [HideInInspector]
    public bool InstanceLivesInScene;

    [SerializeField]
    [InspectorNoteAttribute("MonoSingleton", NoteColor.yellow)] private bool _monoSingletonHeadline;
}

public class MonoSingleton<T> : MonoSingleton where T : MonoSingleton
{
    public static bool EnableWarnings = true;

    //The instance
    private static T _instance;

    //NOTE: using a bool here to avoid Unity's custom == null operation, everytime you use the instance.
    private static bool instantiated;

    //NOTE: Locking to avoid multiple instantiations.
    private static object _lock = new object();

    //NOTE: use this if you want singletons in your project to use the MonoSingletonAttribute to be able to disable persistent mode, on singletons.
    private static bool useAttribute = false;

    private static bool defaultIsPersistent = false;

    public static T instance
    {
        get
        {
            lock (_lock)
            {
                if (applicationIsQuitting)
                {
                    LogWarning("[MonoSingleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                if (!instantiated)
                {
                    var type = typeof(T);

                    if (useAttribute)
                    {
                        var attribute = Attribute.GetCustomAttribute(type, typeof(MonoSingletonAttribute)) as MonoSingletonAttribute;
                        if (attribute != null)
                        {
                            if (attribute.DebugSingletonInEditor)
                            {
                                #if UNITY_EDITOR

                                _instance = (T)FindObjectOfType(type);

                                if (_instance != null)
                                {
                                    Debug.LogError("[MonoSingleton] Found object of type: " + type +
                                        " in the scene, you should make sure to set the instance in " +
                                        "the awake instead for performance reasons.");
                                    
                                    instantiated = true;
                                }
                             
                                var typeObjects = FindObjectsOfType(type);
                                if (typeObjects.Length > 1)
                                {
                                    _instance = (T)typeObjects[0];

                                    Debug.LogError("[MonoSingleton] Something went really wrong " +
                                        " - there should never be more than 1 singleton!" +
                                        " Removing the extra ones...");
                                    for (int i = 1; i < typeObjects.Length; i++)
                                    {
                                        Destroy(typeObjects[i]);
                                    }
                                }    
                                #endif
                            }
                            

                            if (!instantiated)
                            {
                                if (_instance == null)
                                {
                                    GameObject singleton = new GameObject();
                                    _instance = singleton.AddComponent<T>();
                                    singleton.name = "(singleton) " + type.ToString();

                                    LogWarning("[MonoSingleton] An instance of " + type +
                                        " is needed in the scene, so '" + singleton +
                                        "' was created.");

                                    instantiated = true;
                                }
                            }

                            if (!instantiated)
                            {
                                Debug.LogError("[MonoSingleton] Something went wrong! " +
                                    "MonoSingleton not found/created for: \"" + type + "\"");
                            }
                            else
                            {
                                var log = "[MonoSingleton] Using instance: " +
                                          _instance.gameObject.name;

                                if (attribute.Persistent)
                                {
                                    DontDestroyOnLoad(_instance.gameObject);

                                    log += " with DontDestroyOnLoad.";
                                }

                                LogWarning(log);
                            }
                        }
                    }

                    if (!instantiated)
                    {
                        //Default instantiation, if attribute is not found.
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + type.ToString();

                        LogWarning("[MonoSingleton] An instance of " + type +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");

                        if (defaultIsPersistent || !_instance.InstanceLivesInScene)
                            DontDestroyOnLoad(singleton);

                        instantiated = true;
                    }
                }
                    
                return _instance;
            }
        }
        set
        {
            lock (_lock)
            {
                if (instantiated)
                {
                    if (_instance != value)
                    {
                        var type = typeof(T);

                        //Remove instance if it's set to null, to reset a singleton.
                        if (value == null)
                        {
                            LogWarning("[MonoSingleton] Removing instance of [" + type + "] " +
                                "on [" + _instance.name + "] due to the instance being set to null.");
                            
                            DestroyImmediate(_instance);
                            instantiated = false;
                        }
                        else if ((_instance as MonoSingleton<T>).InstanceLivesInScene == false && (value as MonoSingleton<T>).InstanceLivesInScene)
                        {
                            Debug.LogError("[MonoSingleton] Removing duplicate instance of [" + type + "] " +
                                "on [" + _instance.name + "]. BECAUSE IT IS NOT A SCENE INSTANCE!");

                            _instance.name = "FAILED MONOSINGLETON CREATION! [" + type + "]";
                            DestroyImmediate(_instance);
                            applicationIsQuitting = false;
                            _instance = value;
                        }
                        else
                        {
                            _instance = value;

                            //Debug.LogError("[MonoSingleton] Removing duplicate instance of [" + type + "] on [" + value.name + "].");

                            //DestroyImmediate(value);
                        }

                        //remove quitting flag, as we're not quitting...
                        applicationIsQuitting = false;
                    }
                }
                else
                {
                    _instance = value;

                    var type = typeof(T);

                    if (useAttribute)
                    {
                        var attribute = Attribute.GetCustomAttribute(type, typeof(MonoSingletonAttribute)) as MonoSingletonAttribute;
                        if (attribute != null)
                        {
                            if (_instance != null)
                            {
                                instantiated = true;
                            }
                            if (attribute.DebugSingletonInEditor)
                            {
                                #if UNITY_EDITOR
                                var typeObjects = FindObjectsOfType(type);
                                if (typeObjects.Length > 1)
                                {
                                    _instance = (T)typeObjects[0];

                                    Debug.LogError("[MonoSingleton] Something went really wrong " +
                                        " - there should never be more than 1 singleton!" +
                                        " Removing the extra ones...");
                                    for (int i = 1; i < typeObjects.Length; i++)
                                    {
                                        Destroy(typeObjects[i]);
                                    }
                                }    
                                #endif
                            }
                            if (!instantiated)
                            {
                                Debug.LogError("[MonoSingleton] Instance of " + type +
                                    " was set to null, don't do this!");
                            }
                            else
                            {
                                var log = "[MonoSingleton] Using instance: " +
                                          _instance.gameObject.name;

                                if (attribute.Persistent)
                                {
                                    DontDestroyOnLoad(_instance.gameObject);
                                    log += " with DontDestroyOnLoad.";
                                }

                                LogWarning(log); 
                            }
                        }
                    }

                    if (!instantiated && _instance != null)
                    {
                        LogWarning("[MonoSingleton] An instance of [" + type +
                            "] has been registered on [" + _instance.name + "].");

                        //Set persistence, on root elements, setting it on non-root elements is not allowed.
                        if (defaultIsPersistent || !_instance.InstanceLivesInScene)
                            DontDestroyOnLoad(_instance.gameObject);    

                        instantiated = true;
                    }
                }
            }
        }
    }

    private static bool applicationIsQuitting = false;

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    /// <summary>
    /// In ondestroy methods, to deregister events and prevent errors in the editor
    /// Check this for true before deregistering
    /// </summary>
    /// <returns></returns>
    public static bool IsInstantiated()
    {
        return instantiated && !applicationIsQuitting;
    }

    private static void LogWarning(string message)
    {
        if (EnableWarnings)
            Debug.Log(message);
    }
}

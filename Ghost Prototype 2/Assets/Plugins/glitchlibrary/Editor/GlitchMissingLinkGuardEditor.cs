using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GlitchMissingLinkGuard))]
public class GlitchMissingLinkGuardEditor : Editor {

    private bool dirty;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        var gp = target as GlitchMissingLinkGuard;
        foreach (var component in gp.gameObject.GetComponents<Component>())
        {
            if(component != null && component != gp && !(component is Transform) )
            {
                var compenentSo = new SerializedObject(component);
                var propertyIterator = compenentSo.GetIterator();

                while (propertyIterator.NextVisible(true))
                {
                    if(propertyIterator.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        var referencedObject = EditorUtility.InstanceIDToObject(propertyIterator.objectReferenceInstanceIDValue);
                        if(referencedObject == null)
                        {
                            RecoverValue(propertyIterator.objectReferenceInstanceIDValue, gp);
                        }
                        else
                        {
                            string name = referencedObject.name;
                            string componentName = component.GetType().ToString();
                            string property = propertyIterator.name;

                            GameObject gameobject = referencedObject as GameObject;
                            if(referencedObject is Component)
                            {
                                gameobject = (referencedObject as Component).gameObject;
                            }
                            else if(referencedObject is Transform)
                            {
                                gameobject = (referencedObject as Transform).gameObject;
                            }

                            if (gameobject != null)
                            {
                                Object parentObject = PrefabUtility.GetPrefabObject(gameobject);
                                string path = AssetDatabase.GetAssetPath(parentObject);
                                SaveValue(propertyIterator.objectReferenceInstanceIDValue, name, path, componentName, property, gp);

                            }

                        }
                    }
                }
            }
        }



        if (dirty) EditorUtility.SetDirty(target);
        dirty = false;

        
        serializedObject.ApplyModifiedProperties();

        if (gp.messages != null && gp.messages.Count > 0)
        {
            var result = "";
            foreach (var item in gp.messages)
            {
                result += item + "\n\n";
            }
            EditorGUILayout.HelpBox(result, MessageType.Warning, true);
        }
        gp.messages = new List<string>();
    }

    void Init(GlitchMissingLinkGuard gp)
    {
        if (gp.instanceIDs == null || gp.path == null)
        {
            gp.instanceIDs = new List<int>();
            gp.path = new List<string>();
            gp.properties = new List<string>();
            gp.component = new List<string>();
        }
    }

    void SaveValue(int instanceID, string name, string path, string componentName, string property, GlitchMissingLinkGuard gp)
    {
        Init(gp);
        var index = gp.instanceIDs.IndexOf(instanceID);
        if (index >= 0)
        {
            if (gp.path[index] != path)
            {
                gp.path[index] = path;
                gp.properties[index] = property;
                gp.component[index] = componentName;
                dirty = true;
            }
        }
        else
        {
            gp.instanceIDs.Add(instanceID);
            gp.path.Add(path);
            gp.properties.Add(property);
            gp.component.Add(componentName);

            dirty = true;
        }
    }

    void RecoverValue(int instanceID, GlitchMissingLinkGuard gp)
    {
        Init(gp);
        if (gp.messages == null) return;

        var index = gp.instanceIDs.IndexOf(instanceID);
        if (index >= 0)
        {
            gp.messages.Add("Missing reference pointing to:"
                + "\nComponent: " + gp.component[index]
                + "\nProperty: " + gp.properties[index]
                + "\nPath: " + gp.path[index]
            );

            if(gp.autoFix)
            {
                if(gp.path[index] == "")
                {
                    gp.messages.Add("AUTOFIX FAILED: No path, possibly this was never a prefab.");
                }
                else
                {
                    GameObject prefab = null;
                    try
                    {
                        prefab =AssetDatabase.LoadAssetAtPath(gp.path[index], typeof(GameObject)) as GameObject;
                    }
                    catch (UnityException)
                    {
                        gp.messages.Add("AUTOFIX FAILED: Path does not point to a prefab.");
                    }

                    if (prefab == null)
                    {
                        gp.messages.Add("AUTOFIX FAILED: Path does not point to a prefab.");
                    }
                    else
                    {
                        var component = gp.GetComponent(gp.component[index]);
                        var prop = component.GetType().GetField(gp.properties[index]);

                        prop.SetValue(component, prefab.GetComponent(prop.FieldType.Name));
                    }

                }

            }
        }
        else
        {
            gp.messages.Add("Missing reference, no info");
        }
    }
}

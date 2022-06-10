using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

[InitializeOnLoad]
internal class ReorderableListDrawerUpdater
{
    private static int lastActiveInstanceID;

    static ReorderableListDrawerUpdater()
    {
        EditorApplication.update += Update;
        EditorApplication.playmodeStateChanged += ReorderableListDrawer.CleanStates;
    }

    static void Update()
    {
        //Whenever we change selection, clear the states (to avoid dead object refs)
        if (lastActiveInstanceID != Selection.activeInstanceID)
        {
            lastActiveInstanceID = Selection.activeInstanceID;
            ReorderableListDrawer.CleanStates();
        }
    }
}

[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
public class ReorderableListDrawer : PropertyDrawer
{
    #region State stuff, needed to keep ReorderableList alive

    protected class State
    {
        internal ReorderableList reorderableList;

        public bool drawingElements;
    }

    private static Dictionary<string, ReorderableListDrawer.State> s_States = new Dictionary<string, ReorderableListDrawer.State>();

    public static void CleanStates()
    {
        s_States.Clear();
    }

    #endregion

    #region Fields

    private ReorderableListDrawer.State m_State;

    private string listName;
    private SerializedProperty serializedList;
    private SerializedObject serializedObject;
    private UnityEngine.Object target;

    #endregion

    #region List property helpers

    private int GetArrayIndex(SerializedProperty prop)
    {
        var startIndex = prop.propertyPath.LastIndexOf('[') + 1;
        var endIndex = prop.propertyPath.LastIndexOf(']');

        var indexStr = prop.propertyPath.Substring(startIndex, endIndex - startIndex);
        return  int.Parse(indexStr);
    }

    private SerializedProperty GetParentArray(SerializedProperty prop)
    {
        var indexParentString = prop.propertyPath.IndexOf(".Array.data[");
        string propertyPath = prop.propertyPath.Substring(0, indexParentString);

        return prop.serializedObject.FindProperty(propertyPath);
    }

    /// <summary>
    /// Ensures that our target and SerializedObject are always in sync.
    /// </summary>
    private void EnsureTargetObject()
    {
        // If the window was relaoded, our SerializedObject will be disposed, but our target will not be.
        try
        {
            //NOTE: serializedObject is not null when disposed, and if you access a member it'll throw an exception.
            serializedObject.Update();
        }
        catch (Exception)
        {
            //Only do this if serializedObject was disposed
            if (target != null)
            {
                serializedObject = new SerializedObject(target);
                serializedList = serializedObject.FindProperty(listName);
            }
        }
    }

    #endregion

    #region List Replacement Mode

    private void DrawReorderableList(Rect r)
    {
        if (this.m_State != null)
        {
            if (this.m_State.reorderableList != null)
            {
                int indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                this.m_State.reorderableList.DoList(r);
                EditorGUI.indentLevel = indentLevel;
            }
        }
    }

    /// <summary>
    /// Ensures we have a static ReorderableList for each obj + list.
    /// </summary>
    /// <returns>The state.</returns>
    /// <param name="prop">Property.</param>
    private ReorderableListDrawer.State GetState(SerializedProperty property)
    {
        if (!property.isArray)
        {
            var parentProp = GetParentArray(property);

            serializedList = parentProp;
            serializedObject = property.serializedObject;
            target = serializedObject.targetObject;
            listName = parentProp.name;
        }

        var key = target.name + serializedList.propertyPath;

        ReorderableListDrawer.State state = null;
       
        s_States.TryGetValue(key, out state);

        if (state == null)
        {
            state = new ReorderableListDrawer.State();

            state.reorderableList = CreateList(serializedObject, serializedList);
            s_States[key] = state;
        }

        return state;
    }

    private void RestoreState(SerializedProperty property)
    {
        m_State = this.GetState(property);
        m_State.reorderableList.serializedProperty = serializedList;
    }

    private ReorderableList CreateList(SerializedObject obj, SerializedProperty listProp)
    {
        bool dragable = true, header = true, add = true, remove = true;
        var list = new ReorderableList(obj, listProp, dragable, header, add, remove);

        list.onCanRemoveCallback += (inner) =>
        {
            return inner.count > 0;
        };
        
        list.drawHeaderCallback += drawHeader;
        list.drawElementCallback += this.drawElement;
        list.elementHeightCallback += this.getElementHeight;
        list.drawElementBackgroundCallback += this.drawElementBackground;

        return list;
    }

    private float getElementHeight(int index)
    {
        EnsureTargetObject();
        this.RestoreState(serializedList);
        this.m_State.drawingElements = true;        

        var height = EditorGUI.GetPropertyHeight(this.serializedList.GetArrayElementAtIndex(index), GUIContent.none, true);
        this.m_State.drawingElements = false;
        return height;
    }

    private void drawHeader(Rect rect)
    {
        var typeStr = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.Name : "";

        if (fieldInfo.FieldType.IsGenericType)
        {
            typeStr = fieldInfo.FieldType.GetGenericTypeDefinition().Name.TrimEnd('`', '1') + "<" + fieldInfo.FieldType.GetGenericArguments()[0] + ">";
        }

        EditorGUI.LabelField(rect, typeStr);
    }

    private void drawElementBackground(Rect rect, int index, bool active, bool focused)
    {
        EnsureTargetObject();
        this.RestoreState(serializedList);
        
        if (Event.current.type == EventType.Repaint)
        {
            this.m_State.drawingElements = true;

            rect.height = EditorGUI.GetPropertyHeight(this.serializedList.GetArrayElementAtIndex(index), GUIContent.none, true);
            //adjust for pixel spacing, and add a little extra height to the selection box
            rect.height += 4f;
            rect.y += 1;
            ReorderableList.defaultBehaviours.elementBackground.Draw(rect, false, active, active, focused);

            this.m_State.drawingElements = false;    
        }
    }

    private void drawElement(Rect rect, int index, bool active, bool focused)
    {
        EnsureTargetObject();
        this.RestoreState(serializedList);

        var property = this.serializedList.GetArrayElementAtIndex(index);

        property.serializedObject.Update(); // Needed for free good editor functionality

        this.m_State.drawingElements = true;

        if (index != 0)
        {
            //Slight y pos adjustment to compensate for the 2 pixel spacing
            rect.y += 2f;
        }
        rect.height = EditorGUI.GetPropertyHeight(property, GUIContent.none, true);

        //Move it slightly right, to avoid element expander and move handle overlapping.
        rect.width -= 10;
        rect.x += 10;

        //Causes a call back to OnGui, due to the attribute on the property...
        EditorGUI.PropertyField(rect, property, true);

        property.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality

        this.m_State.drawingElements = false;
    }

    #endregion

    #region Context Menu Mode

    private void CreateContextMenu(Rect position, SerializedProperty property)
    {
        Event currentEvent = Event.current;
        Rect contextRect = position;

        if (currentEvent.type == EventType.ContextClick)
        {
            Vector2 mousePos = currentEvent.mousePosition;
            if (contextRect.Contains(mousePos))
            {
                // Now create the menu, add items and show it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Dublicate Element"), false, AddElementCallback, property);
                menu.AddItem(new GUIContent("Remove Element"), false, RemoveElementCallback, property);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Move Element Up"), false, MoveElementUpCallback, property);
                menu.AddItem(new GUIContent("Move Element Down"), false, MoveElementDownCallback, property);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Move Element Top"), false, MoveElementTopCallback, property);
                menu.AddItem(new GUIContent("Move Element Bottom"), false, MoveElementBottomCallback, property);

                menu.ShowAsContext();
                currentEvent.Use();
            }
        }
    }

    private void AddElementCallback(object obj)
    {
        var index = GetArrayIndex(obj as SerializedProperty);
        var parentArray = GetParentArray(obj as SerializedProperty);

        parentArray.InsertArrayElementAtIndex(index);

        parentArray.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality
    }

    private void RemoveElementCallback(object obj)
    {
        var index = GetArrayIndex(obj as SerializedProperty);
        var parentArray = GetParentArray(obj as SerializedProperty);

        parentArray.DeleteArrayElementAtIndex(index);

        parentArray.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality
    }

    private void MoveElementUpCallback(object obj)
    {
        var index = GetArrayIndex(obj as SerializedProperty);
        var parentArray = GetParentArray(obj as SerializedProperty);

        if (index > 0)
        {
            parentArray.MoveArrayElement(index, index - 1);
            parentArray.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality
        }
    }

    private void MoveElementDownCallback(object obj)
    {
        var index = GetArrayIndex(obj as SerializedProperty);
        var parentArray = GetParentArray(obj as SerializedProperty);

        if (index < parentArray.arraySize - 1)
        {
            parentArray.MoveArrayElement(index, index + 1);
            parentArray.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality
        }
    }

    private void MoveElementTopCallback(object obj)
    {
        var index = GetArrayIndex(obj as SerializedProperty);
        var parentArray = GetParentArray(obj as SerializedProperty);

        if (index > 0)
        {
            parentArray.MoveArrayElement(index, 0);
            parentArray.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality
        }
    }

    private void MoveElementBottomCallback(object obj)
    {
        var index = GetArrayIndex(obj as SerializedProperty);
        var parentArray = GetParentArray(obj as SerializedProperty);

        if (index < parentArray.arraySize - 1)
        {
            parentArray.MoveArrayElement(index, parentArray.arraySize - 1);
            parentArray.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality
        }
    }

    #endregion

    #region Property Drawer overrides

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        this.RestoreState(property);

        //

        var rla = attribute as ReorderableListAttribute;

        if (rla.contextMenuOnlyMode)
        {
            property.serializedObject.Update(); // Needed for free good editor functionality

            //NOTE:Create context menu first, to avoid default PropertyField handling it!
            CreateContextMenu(position, property);

            var index = GetArrayIndex(property);

            //Put a tiny marker on list elements, to indicate they're different.
            Rect tiny = new Rect(position);
            tiny.width = 4f;
            tiny.height = 2f;
            tiny.y += -1f + position.height / 2f;
            EditorGUI.DrawRect(tiny, Color.white);

            //Draw default property Drawer for each child.
            EditorGUI.PropertyField(position, property, new GUIContent("Element " + index, "Right click to Reorder List"), true);    

            property.serializedObject.ApplyModifiedProperties(); // Needed for zero-hassle good editor functionality
        }
        else
        {
            if (m_State.drawingElements)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            else if (property.propertyPath.Contains("[0]"))
            {
                this.DrawReorderableList(position);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        this.RestoreState(property);

        var rla = attribute as ReorderableListAttribute;

        if (rla.contextMenuOnlyMode)
        {
            return EditorGUI.GetPropertyHeight(property, GUIContent.none, true); 
        }
            
        //On internal recursive calls, ignore overall size
        if (m_State.drawingElements)
        {
            //2f is 1 pixels top and bottom around each element
            float spacing = 2f;

            if (serializedList.GetArrayElementAtIndex(0) == property)
            {
                //first element for some reason needs an offset of 7, due to some internal ReorderableList things
                spacing = 7f;
            }

            return EditorGUI.GetPropertyHeight(property, GUIContent.none, true) + spacing; 
        }
        else if (property.propertyPath.Contains("[0]"))
        {
            //Only return full size for element 0
            return m_State.reorderableList.GetHeight();
        }

        //When not called from inside the ReorderableList and not element 0, then return 0f.
        return 0f;
    }

    #endregion
}

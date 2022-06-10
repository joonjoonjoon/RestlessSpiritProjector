using UnityEngine;
using System.Collections;
using System;

public class ReorderableListAttribute : PropertyAttribute
{
    public bool contextMenuOnlyMode;

    public ReorderableListAttribute(bool contextMenuOnlyMode = false)
    {
        this.contextMenuOnlyMode = contextMenuOnlyMode;
    }
}

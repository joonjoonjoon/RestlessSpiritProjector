using UnityEngine;
using System.Collections;
using System;

public class InspectorShowIfAttribute : PropertyAttribute
{
    public readonly string showIf;

    public InspectorShowIfAttribute(string showIf)
    {
        this.showIf = showIf;
    }
}

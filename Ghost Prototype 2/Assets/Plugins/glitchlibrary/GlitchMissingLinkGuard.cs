using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlitchMissingLinkGuard : MonoBehaviour {
    public bool autoFix;
    [HideInInspector]
    public List<int> instanceIDs;
    [HideInInspector]
    public List<string> path;
    [HideInInspector]
    public List<string> component;
    [HideInInspector]
    public List<string> properties;
    [HideInInspector]
    public List<string> messages;
}

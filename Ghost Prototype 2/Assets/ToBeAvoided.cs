using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToBeAvoided : MonoBehaviour {
    public float radius = 2;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white.SetA(0.5f); 
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, radius); 
#endif
    }
        
}

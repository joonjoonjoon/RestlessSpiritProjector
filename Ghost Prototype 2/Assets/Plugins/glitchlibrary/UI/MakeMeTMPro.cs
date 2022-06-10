using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class MakeMeTMPro : MonoBehaviour {

    void OnEnable()
    {
        Text t = GetComponent<Text>();
        string s = t.text;
        int fs = t.fontSize;
        Color c = t.color;
        DestroyImmediate(t);
        TMPro.TextMeshProUGUI u = gameObject.AddComponent<TMPro.TextMeshProUGUI>();
        u.text = s;
        u.raycastTarget = false;
        u.fontSize = fs;
        u.color = c;
        //u.alignment = t.alignment;
        DestroyImmediate(this);
    }
}

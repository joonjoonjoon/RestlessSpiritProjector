using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TilingAndOffset : MonoBehaviour {
    [Range(-1f, 1f)]
    public float tilingY;
    [Range(-1f, 1f)]
    public float offsetY;

    void Update()
    {
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", new Vector2(0, offsetY));
        GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", new Vector2(1, tilingY));
    }
}

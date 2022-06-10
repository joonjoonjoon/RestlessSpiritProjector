using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class snapper : MonoBehaviour
{
    public float width = 1;
    public float height = 1;
    public string snapTag;
    void LateUpdate()
    {
        if (!Application.isPlaying && width > 0 && height > 0)
        {
            try {

                foreach (var item in GameObject.FindGameObjectsWithTag(snapTag))
                {
                    var snapx = Mathf.Round(item.transform.position.x / width) * width;
                    var snapy = Mathf.Round(item.transform.position.y / height) * height;
                    var snapscalex = Mathf.Round(item.transform.localScale.x / width) * width;
                    var snapscaley = Mathf.Round(item.transform.localScale.y / height) * height;
                    
                    bool isEvenScaleX = Mathf.Round(snapscalex / width) % 2 == 0;
                    bool isEvenScaleY = Mathf.Round(snapscaley / height) % 2 == 0;

                    if (isEvenScaleX)
                    {


                        var floatval = item.transform.localScale.x / width;
                        var tipping = floatval- Mathf.Floor(floatval) > 0.5f;

                        Debug.Log("SNAPPING X " + (floatval - Mathf.Floor(floatval)));
                        snapx = (Mathf.Round(((item.transform.position.x) / width)) + (tipping?0.5f:-0.5f)) * width;
                    }
                    if (isEvenScaleY)
                    {
                        
                        var floatval = item.transform.localScale.y / height;
                        var tipping = floatval - Mathf.Floor(floatval) > 0.5f;

                        snapy = (Mathf.Round(((item.transform.position.y) / height)) + +(tipping ? 0.5f : -0.5f)) * height;
                    }

                    item.transform.position = new Vector3(snapx, snapy, item.transform.position.z);
                    item.transform.localScale = new Vector3(snapscalex, snapscaley, item.transform.position.z);
                }
            }
            catch(UnityException)
            {
                return;
            }
        }
    }
}
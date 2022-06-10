using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class GlitchLayoutGroup : MonoBehaviour
{
#if UNITY_EDITOR
    public float spacing;
    public bool preserveSizes;
    public float topPadding;
    public bool adjustParentHeight;
    public bool horizontal;
    public bool center;


    void Update()
    {
        if (!Application.isPlaying)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        float counter = topPadding;
        for (int i = 0; i < transform.childCount; i++)
        {
            //skip disabled objects
            if (!transform.GetChild(i).gameObject.activeSelf)
                continue;

            if (!horizontal)
            {
                float height = 0;
                if (preserveSizes)
                {
                    height = transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
                }
                transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -counter);
                if (i + 1 < transform.childCount)
                    counter += spacing + height;
            }
            else
            {
                float width = 0;
                if (preserveSizes)
                {
                    width = transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
                }
                transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(counter, 0);

                if (i+1<transform.childCount)
                    counter += spacing + width;
            }
        }


        if (center)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).gameObject.activeSelf)
                    continue;

                if (!horizontal)
                {
                    transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, counter/2f);
                }
                else
                {
                    transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition -= new Vector2(counter/2f, 0);
                }
            }
        }

        if (adjustParentHeight)
        {
            var size = transform.GetComponent<RectTransform>().sizeDelta;
            if (!horizontal)
            {
                transform.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, counter);
            }
            else
            {
                transform.GetComponent<RectTransform>().sizeDelta = new Vector2(counter, size.y);
            }
        }
    }
#endif
}

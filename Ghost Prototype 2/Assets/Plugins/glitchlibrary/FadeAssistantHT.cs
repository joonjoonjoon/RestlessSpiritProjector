using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

public class FadeAssistantHT {

    public static float duration = 0.3f;

    public static void Init()
    {
    }

    public static Sequence FadeIn(Object obj, float modifier=1f)
    {
        var s = new Sequence();
        s.Append(Fade(obj, 1, false, modifier));
        return s;
    }

    public static Sequence FadeOut(Object obj, float modifier = 1f)
    {
        var s = new Sequence();
        s.Append(Fade(obj, 0, true, modifier));
        return s;
    }

    private static Sequence Fade(Object obj, float alpha, bool disableButtons, float modifier = 1f)
    {
        Init();

        float alphaMax = 1;

        var s = new Sequence();


        if (obj is CanvasGroup)
        {
            var group = obj as CanvasGroup;

            alpha = Mathf.Clamp(alpha, 0, alphaMax);

            s.Insert(0,
                HOTween.To(group, duration * modifier, "alpha", alpha)
            );

            if (disableButtons)
            {
                group.interactable = false;
                group.blocksRaycasts = false;
            }
            else
            {
                group.interactable = true;
                group.blocksRaycasts = true;
            }

        }
        else if (obj is Transform)
        {
            var t = obj as Transform;

            // fade if canvas group
            var group = t.GetComponent<CanvasGroup>();
            if (group != null)
            {
                s.Insert(0,
                    Fade(group, alpha, disableButtons, modifier)
                );
                return s; // we don't want to cascade when we meet a canvas group, since that's what it's fore
            }
        }
        else if (obj is GameObject)
        {
            var g = obj as GameObject;
            s.Append(Fade(g.transform, alpha, disableButtons, modifier));
        }
        else
        {
            Debug.LogWarning("Couldn't fade object: " + obj + " cause I'm not sure what it is...", obj);
        }
        return s;
    }

}

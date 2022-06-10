using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;

public class FadeAssistant
{
    public static float defaultDuration = 0.3f;

    public static Sequence FadeIn(Object obj, float duration = -1f)
    {
        return Fade(obj, 1, false, duration);
    }

    public static Sequence FadeOut(Object obj, float duration = -1f)
    {
        return Fade(obj, 0, true, duration);
    }

    private static Sequence Fade(Object obj, float alpha, bool disableButtons, float duration)
    {
        var s = DOTween.Sequence();

        if (duration < 0)
        {
            duration = defaultDuration;
        }

        if (obj is CanvasGroup)
        {
            var group = obj as CanvasGroup;

            s.Insert(0, DOTween.To((x) => group.alpha = x, group.alpha, alpha, duration));

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
        else if (obj is IGlitchUIElement)
        {
            var t = obj as IGlitchUIElement;

            // fade if canvas group
            var group = t.GetCanvasGroup();
            if (group != null)
            {
                s.Insert(0, Fade(group, alpha, disableButtons, duration)
                );
                return s; 
            }
        }
        else if (obj is GlitchUIElement)
        {
            var t = obj as GlitchUIElement;

            // fade if canvas group
            var group = t.GetComponent<CanvasGroup>();
            if (group != null)
            {
                s.Insert(0, Fade(group, alpha, disableButtons, duration)
                );
                return s; 
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
                    Fade(group, alpha, disableButtons, duration)
                );
                return s; // we don't want to cascade when we meet a canvas group, since that's what it's fore
            }
        }
        else if (obj is GameObject)
        {
            var g = obj as GameObject;
            s.Append(Fade(g.transform, alpha, disableButtons, duration));
        }
        else
        {
            Debug.LogWarning("Couldn't fade object: " + obj + " cause I'm not sure what it is...", obj);
        }
        return s;
    }

}

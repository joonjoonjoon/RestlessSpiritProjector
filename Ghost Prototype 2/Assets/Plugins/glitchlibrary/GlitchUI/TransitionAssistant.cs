using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class TransitionAssistant : MonoBehaviour
{
    public static float defaultDuration = 0.3f;

    public static Dictionary<int, Vector2> positionDictionary;
    public static Dictionary<int, Vector3> sizeDictionary;

    public static void Init()
    {
        if (positionDictionary == null)
        {
            positionDictionary = new Dictionary<int, Vector2>();
        }

        if (sizeDictionary == null)
        {
            sizeDictionary = new Dictionary<int, Vector3>();
        }
    }

    public static Sequence ZoomIn(Object obj, bool scaleUp, float overshoot = 0f, float duration = -1f)
    {
        return Zoom(obj, scaleUp, false, overshoot, duration);
    }

    public static Sequence ZoomOut(Object obj, bool scaleUp, float duration = -1f)
    {
        return Zoom(obj, scaleUp, true, 0f, duration);
    }

    public static Sequence SlideIn(Object obj, Vector2 direction, float duration = -1f)
    {
        return Slide(obj, direction, false, duration);
    }

    public static Sequence SlideOut(Object obj, Vector2 direction, float duration = -1f)
    {
        return Slide(obj, direction, true, duration);
    }

    private static Sequence Zoom(Object obj, bool scaleUp, bool isZoomOut, float overshoot, float duration)
    {
        Init();

        Vector3 startScale = Vector3.one;
        bool found = sizeDictionary.TryGetValue(obj.GetHashCode(), out startScale);

        var s = DOTween.Sequence();

        if (duration < 0)
        {
            duration = defaultDuration;
        }
            
        if (obj is CanvasGroup)
        {
            var group = obj as CanvasGroup;

            if (!found)
            {
                startScale = group.GetComponent<RectTransform>().localScale;
                sizeDictionary.Add(obj.GetHashCode(), startScale);
            }

            var endScale = startScale;

            if (scaleUp)
            {
                endScale = startScale * 2f;    
            }
            else
            {
                //Don't scale texts to 0, that'll break rendering! So instead we just scale to 0.01f
                endScale = Vector3.one * 0.01f;
            }


            if (isZoomOut)
            {
                s.Insert(0, DOTween.To(() => group.GetComponent<RectTransform>().localScale, (x) => group.GetComponent<RectTransform>().localScale = x, endScale, duration));

                group.interactable = false;
                group.blocksRaycasts = false;
            }
            else
            {
                group.GetComponent<RectTransform>().localScale = startScale;
                s.Insert(0, DOTween.To(() => group.GetComponent<RectTransform>().localScale, (x) => group.GetComponent<RectTransform>().localScale = x, endScale, duration).From().SetEase(Ease.OutBack, overshoot).OnComplete(() =>
                        {
                            group.interactable = true;
                            group.blocksRaycasts = true;
                        }));
            }

        }
        else if (obj is IGlitchUIElement)
        {
            var t = obj as IGlitchUIElement;

            // fade if canvas group
            var group = t.GetCanvasGroup();
            if (group != null)
            {
                s.Insert(0, Zoom(obj, scaleUp, isZoomOut, overshoot, duration));
                return s; 
            }
        }
        else if (obj is GlitchUIElement)
        {
            var t = obj as GlitchUIElement;

            // fade if canvas group
            var group = t.GetCanvasGroup();
            if (group != null)
            {
                s.Insert(0, Zoom(obj, scaleUp, isZoomOut, overshoot, duration));
                return s; 
            }
        }
        // Laziness, you can dump in Transforms and Gameobjects as well as Canvasgroups
        else if (obj is Transform)
        {
            var t = obj as Transform;

            // fade if canvas group
            var group = t.GetComponent<CanvasGroup>();
            if (group != null)
            {
                s.Insert(0, Zoom(obj, scaleUp, isZoomOut, overshoot, duration));
                return s; 
            }
        }
        else if (obj is GameObject)
        {
            var g = obj as GameObject;

            // fade if canvas group
            var group = g.GetComponent<CanvasGroup>();
            if (group != null)
            {
                s.Insert(0, Zoom(obj, scaleUp, isZoomOut, overshoot, duration));
                return s; 
            }
        }

        return s;
    }


    private static Sequence Slide(Object obj, Vector2 direction, bool isSlideOut, float duration)
    {
        Init();

        Vector2 startPos = Vector2.zero;
        bool found = positionDictionary.TryGetValue(obj.GetHashCode(), out startPos);

        var s = DOTween.Sequence();

        if (duration < 0)
        {
            duration = defaultDuration;
        }

        if (obj is CanvasGroup)
        {
            var group = obj as CanvasGroup;

            var recttransform = group.GetComponent<RectTransform>();
            if (!found)
            {
                startPos = recttransform.anchoredPosition;
                positionDictionary.Add(obj.GetHashCode(), startPos);
            }

            var canvases = recttransform.GetComponentsInParent<Canvas>();
            var mainCanvas = canvases[0]; // this will only fail if you're transitioning a recttransform that doesn't live on a canvas... You probably don't want that.

            var mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();
            var width = mainCanvasRectTransform.rect.width;
            var height = mainCanvasRectTransform.rect.height;

            var endPos = new Vector2((width + startPos.x) * direction.x, (height + startPos.y) * direction.y);

            if (isSlideOut)
            {
                s.Insert(0, DOTween.To(() => group.GetComponent<RectTransform>().anchoredPosition, (x) => group.GetComponent<RectTransform>().anchoredPosition = x, endPos, duration));

                group.interactable = false;
                group.blocksRaycasts = false;
            }
            else
            {
                group.GetComponent<RectTransform>().anchoredPosition = startPos;
                s.Insert(0, DOTween.To(() => group.GetComponent<RectTransform>().anchoredPosition, (x) => group.GetComponent<RectTransform>().anchoredPosition = x, endPos, duration).From().OnComplete(() =>
                        {
                            group.interactable = true;
                            group.blocksRaycasts = true;
                        }));
            }

        }
        else if (obj is IGlitchUIElement)
        {
            var t = obj as IGlitchUIElement;

            // fade if canvas group
            var group = t.GetCanvasGroup();
            if (group != null)
            {
                s.Insert(0, Slide(group, direction, isSlideOut, duration)
                );
                return s; 
            }
        }
        else if (obj is GlitchUIElement)
        {
            var t = obj as GlitchUIElement;

            // fade if canvas group
            var group = t.GetCanvasGroup();
            if (group != null)
            {
                s.Insert(0, Slide(group, direction, isSlideOut, duration)
                );
                return s; 
            }
        }
        // Laziness, you can dump in Transforms and Gameobjects as well as Canvasgroups
        else if (obj is Transform)
        {
            var t = obj as Transform;

            // fade if canvas group
            var group = t.GetComponent<CanvasGroup>();
            if (group != null)
            {
                s.Insert(0,
                    Slide(group, direction, isSlideOut, duration)
                );
                return s; 
            }
        }
        else if (obj is GameObject)
        {
            var g = obj as GameObject;

            // fade if canvas group
            var group = g.GetComponent<CanvasGroup>();
            if (group != null)
            {
                s.Insert(0,
                    Slide(group, direction, isSlideOut, duration)
                );
                return s; 
            }
        }

        return s;
    }
}

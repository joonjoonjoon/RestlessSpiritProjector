using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public static class IGlitchUIElementExtension
{
    public static CanvasGroup GetCanvasGroup(this IGlitchUIElement element)
    {
        var mb = element as MonoBehaviour;
        if (mb != null)
        {
            var cg = mb.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = mb.gameObject.AddComponent<CanvasGroup>();
            }

            return cg;
        }

        var uib = element as UIBehaviour;
        if (uib != null)
        {
            var cg = uib.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = uib.gameObject.AddComponent<CanvasGroup>();
            }

            return cg;
        }

        Debug.LogError("Something went wrong, we should never get here!");
        return null;
    }

    public static void SetElementShowing(this IGlitchUIElement element, bool showing = true)
    {
        element.glitchUIElementState = new GlitchUIElementState { isShowing = showing, isTransitioning = false };
    }

    public static void Toggle(this IGlitchUIElement element, bool animate = true, GlitchUITransitionTypes transitionShow = GlitchUITransitionTypes.Fade, GlitchUITransitionTypes transitionHide = GlitchUITransitionTypes.Fade)
    {
        if (element.glitchUIElementState.isShowing)
        {
            element.OnToggle(transitionHide);
            Hide(element, animate, transitionHide);
        }
        else
        {
            element.OnToggle(transitionShow);
            Show(element, animate, transitionShow);
        }
    }

    public static void Hide(this IGlitchUIElement element, bool animate = true, GlitchUITransitionTypes transition = GlitchUITransitionTypes.Fade)
    {
        if (element.glitchUIElementState.isShowing)
        {
            element.OnHide(transition);
        }
        element.glitchUIElementState = new GlitchUIElementState { isShowing = false, isTransitioning = true };

        switch (transition)
        {
            case GlitchUITransitionTypes.Fade:
                DoFadeOutTransition(element, animate);
                break;
            case GlitchUITransitionTypes.SlideLeft:
                DoSlideOutTransition(element, Vector2.left, animate);
                break;
            case GlitchUITransitionTypes.SlideRight:
                DoSlideOutTransition(element, Vector2.right, animate);
                break;
            case GlitchUITransitionTypes.SlideUp:
                DoSlideOutTransition(element, Vector2.up, animate);
                break;
            case GlitchUITransitionTypes.SlideDown:
                DoSlideOutTransition(element, Vector2.down, animate);
                break;
            case GlitchUITransitionTypes.ZoomZero:
                DoZoomOutTransition(element, false, animate);
                DoFadeOutTransition(element, animate);
                break;
            case GlitchUITransitionTypes.Zoom10x:
                DoZoomOutTransition(element, true, animate);
                DoFadeOutTransition(element, animate);
                break;
        }
    }

    public static void Show(this IGlitchUIElement element, bool animate = true, GlitchUITransitionTypes transition = GlitchUITransitionTypes.Fade)
    {
        var wasShowing = element.glitchUIElementState.isShowing;
        element.glitchUIElementState = new GlitchUIElementState { isShowing = true, isTransitioning = true };

        switch (transition)
        {
            case GlitchUITransitionTypes.Fade:
                DoFadeInTransition(element, animate);
                break;
            case GlitchUITransitionTypes.SlideLeft:
                DoSlideInTransition(element, Vector2.left, animate);
                DoFadeInTransition(element, animate);
                break;
            case GlitchUITransitionTypes.SlideRight:
                DoSlideInTransition(element, Vector2.right, animate);
                DoFadeInTransition(element, animate);
                break;
            case GlitchUITransitionTypes.SlideUp:
                DoSlideInTransition(element, Vector2.up, animate);
                DoFadeInTransition(element, animate);
                break;
            case GlitchUITransitionTypes.SlideDown:
                DoSlideInTransition(element, Vector2.down, animate);
                DoFadeInTransition(element, animate);
                break;
            case GlitchUITransitionTypes.ZoomZero:
                DoZoomInTransition(element, false, animate);
                DoFadeInTransition(element, animate);
                break;
            case GlitchUITransitionTypes.Zoom10x:
                DoZoomInTransition(element, true, animate);
                DoFadeInTransition(element, animate);
                break;
        }

        // OnShow needs to happen here, because otherwise coRoutines that are called OnShow on children of the element
        // wont execute cause it's still disabled. -joon
        if (!wasShowing)
        {
            element.OnShow(transition);
        }

    }

    private static void DoFadeOutTransition(IGlitchUIElement element, bool animate = true)
    {
        var cg = GetCanvasGroup(element);

        FadeAssistant.FadeOut(cg, animate ? -1 : 0)
            .Play()
            .OnComplete(() =>
            {
                element.glitchUIElementState = new GlitchUIElementState { isShowing = element.glitchUIElementState.isShowing, isTransitioning = false };
                if (element.glitchUIElementState.isShowing == false)
                    cg.gameObject.SetActive(false);
            });
    }

    private static void DoFadeInTransition(IGlitchUIElement element, bool animate = true)
    {
        var cg = GetCanvasGroup(element);

        if (!cg.gameObject.activeSelf)
        {
            cg.gameObject.SetActive(true);
        }
        FadeAssistant.FadeIn(element.GetCanvasGroup(), animate ? -1 : 0)
            .Play()
            .OnComplete(() =>
            {
                element.glitchUIElementState = new GlitchUIElementState { isShowing = element.glitchUIElementState.isShowing, isTransitioning = false };
            });
    }

    private static void DoSlideOutTransition(IGlitchUIElement element, Vector2 direction, bool animate = true)
    {
        var cg = GetCanvasGroup(element);
        TransitionAssistant.SlideOut(cg, direction, animate ? -1 : 0)
            .Play()
            .OnComplete(() =>
            {
                element.glitchUIElementState = new GlitchUIElementState { isShowing = element.glitchUIElementState.isShowing, isTransitioning = false };
                if (element.glitchUIElementState.isShowing == false)
                    cg.gameObject.SetActive(false);
                 
            });
    }

    private static void DoSlideInTransition(IGlitchUIElement element, Vector2 direction, bool animate = true)
    {
        var cg = GetCanvasGroup(element);

        if (!cg.gameObject.activeSelf)
        {
            cg.gameObject.SetActive(true);
        }
        TransitionAssistant.SlideIn(element.GetCanvasGroup(), direction, animate ? -1 : 0)
            .Play()
            .OnComplete(() =>
            {
                element.glitchUIElementState = new GlitchUIElementState { isShowing = element.glitchUIElementState.isShowing, isTransitioning = false };
            });
    }

    private static void DoZoomOutTransition(IGlitchUIElement element, bool scaleUp, bool animate = true)
    {
        var cg = GetCanvasGroup(element);
        TransitionAssistant.ZoomOut(cg, scaleUp, animate ? -1 : 0)
            .Play()
            .OnComplete(() =>
            {
                element.glitchUIElementState = new GlitchUIElementState { isShowing = element.glitchUIElementState.isShowing, isTransitioning = false };
                if (element.glitchUIElementState.isShowing == false)
                    cg.gameObject.SetActive(false);
            });
    }

    private static void DoZoomInTransition(IGlitchUIElement element, bool scaleUp, bool animate = true)
    {
        var cg = GetCanvasGroup(element);

        if (!cg.gameObject.activeSelf)
        {
            cg.gameObject.SetActive(true);
        }
        TransitionAssistant.ZoomIn(element.GetCanvasGroup(), scaleUp, 5f, animate ? -1 : 0)
            .Play()
            .OnComplete(() =>
            {
                element.glitchUIElementState = new GlitchUIElementState { isShowing = element.glitchUIElementState.isShowing, isTransitioning = false };
            });
    }
}

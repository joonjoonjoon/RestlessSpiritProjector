using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

[Serializable]
public struct GlitchUITransition
{
    public GlitchUIElement element;
    public GlitchUITransitionTypes transition;
}

[RequireComponent(typeof(CanvasGroup))]
public class GlitchUIElement : MonoBehaviour, IGlitchUIElement
{
    [SerializeField]
    private bool HideOnAwake;

    private CanvasGroup canvasGroup;

    [SerializeField]
    [InspectorNote("Glitch UI Element State")]
    private GlitchUIElementState _glitchUIElementState;
    private GlitchUIElementAnimation _animation;

    public GlitchUIElementState glitchUIElementState
    {
        get
        {
            return _glitchUIElementState;
        }

        set
        {
            _glitchUIElementState = value;
        }
    }

    protected virtual void Awake()
    {
        if (!HideOnAwake)
        {
            //Avoid triggering the OnShow, when initializing
            this.SetElementShowing();
            this.Show(false);
        }
        else if (HideOnAwake && gameObject.activeSelf)
        {
            this.Hide(false);
        }
    }

    public virtual void OnShow()
    {
        //no implementation
    }

    public virtual void OnHide()
    {
        //no implementation
    }

    public virtual void OnToggle()
    {
        //no implementation
    }

    public virtual void OnShow(GlitchUITransitionTypes transition)
    {
        OnShow();
    }

    public virtual void OnHide(GlitchUITransitionTypes transition)
    {
        OnHide();
    }

    public virtual void OnToggle(GlitchUITransitionTypes transition)
    {
        OnToggle();
    }

    public bool isShowing { get { return glitchUIElementState.isShowing; } }

    public void DisableInteractions()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void EnableInteractions()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    
    public void Animate(GlitchUIElementAnimation.AnimationType type)
    {
        if(_animation == null)
        {
            _animation = gameObject.AddComponent<GlitchUIElementAnimation>();
        }
        _animation.Animate(type);
    }
}

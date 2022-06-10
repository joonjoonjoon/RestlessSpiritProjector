using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
[ExecuteInEditMode]
public class GlitchUIButton : UIBehaviour, IGlitchUIElement, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    #region IGlitchUIElement implementation

    [SerializeField]
    private bool HideOnAwake;

    [SerializeField]
    [InspectorNote("Glitch UI Element State")]
    private GlitchUIElementState _glitchUIElementState;

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

    #endregion


    [Range(0f, 1f)]
    public float pressPercentage = 1;
    [Range(0f, 1f)]
    public float heightPercentage = 0.1f;
    [Range(0f, 1f)]
    public float darkenBackgroundPercentage = 0.25f;
    [Range(0f, 1f)]
    public float darkenWhenPressedPercentage = 0.25f;
    public Color color;
    public TMPro.TextMeshProUGUI textStyle;
    public UnityEvent OnButtonDown;

    
    private float duration = 0.3f;
    private Image top;
    private Image bottom;
    private Tween moveTween;
    private bool isHovering;
    private bool isPressing;

    new void Awake()
    {
        base.Awake();

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

    new void OnEnable()
    {
        base.OnEnable();
        bottom = GetComponent<Image>();
        if (transform.childCount == 0) DebugExtension.BlipError(this, "GlitchUIButton without a top image found");
        else
            top = transform.GetChild(0).GetComponent<Image>();
        if (top == null) DebugExtension.BlipError(this, "GlitchUIButton without a top image found");
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            RefreshStyle();
            var text = GetComponentInChildren<GlitchUITmproStyle>();
            if (text != null)
            {
                text.style = textStyle;
            }
        }
    }

    public void SetTopImage(Sprite top)
    {
        this.top.sprite = top;
    }

    public void SetBottomImage(Sprite bottom)
    {
        this.bottom.sprite = bottom;
    }

    public void RefreshStyle()
    {
        // update colors
        if (bottom != null && top != null)
        {
            top.color = color;
            bottom.color = color.SetV(color.GetV() - darkenBackgroundPercentage);
        }

        // update height
        if (top != null)
        {
            top.rectTransform.anchorMax = top.rectTransform.anchorMax.withY(1 + heightPercentage);
            top.rectTransform.anchorMin = top.rectTransform.anchorMin.withY(0 + heightPercentage);
        }
    }

    public void Toggle(bool down)
    {
        if (down)
        {
            DoAnimateDown();
        }
        else
        {
            DoAnimateUp(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (isPressing)
        {
            DoAnimateDown();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (isPressing) DoAnimateUp(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DoAnimateDown();
        isPressing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isHovering)
        {
            if (isPressing && OnButtonDown != null) OnButtonDown.Invoke();

            DoAnimateUp();
        }
        isPressing = false;
    }

    public void DoAnimateDown()
    {
        if (moveTween != null) moveTween.Kill();
        // move
        moveTween = DOTween.To(
            (y) =>
            {
                top.rectTransform.anchorMin = top.rectTransform.anchorMin.withY(y - 1);
                top.rectTransform.anchorMax = top.rectTransform.anchorMax.withY(y);
            },
            top.rectTransform.anchorMax.y, 1 + (pressPercentage * heightPercentage),
            duration);

        // color
        DOTween.To(
            () => top.color,
            (c) =>
            {
                top.color = c;
            },
            color.SetV(color.GetV() - darkenWhenPressedPercentage),
            duration
        );
    }

    public void DoAnimateUp(bool execute = true)
    {
        if (moveTween != null) moveTween.Kill();
        // move
        moveTween = DOTween.To(
            (y) =>
            {
                top.rectTransform.anchorMin = top.rectTransform.anchorMin.withY(y - 1);
                top.rectTransform.anchorMax = top.rectTransform.anchorMax.withY(y);
            }, top.rectTransform.anchorMax.y, 1 + heightPercentage, duration);
        // color
        DOTween.To(
            () => top.color,
            (c) =>
            {
                top.color = c;
            },
            color,
            duration
        );
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
}

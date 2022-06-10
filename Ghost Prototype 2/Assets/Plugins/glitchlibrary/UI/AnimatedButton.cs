using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Holoville.HOTween;

[ExecuteInEditMode]
[RequireComponent(typeof(HoldDownButton))]
public class AnimatedButton : MonoBehaviour, IFullscreenPopupHandler  {

    public Image top;
    public Image bottom;
    public bool autoSetShadow;
    public UnityEvent OnAnimateUpCompleted;
    public bool isToggle;
    [Range(0f, 1f)]
    public float pressPercentage = 1;
    [Range(0f, 1f)]
    public float darkenBackgroundPercentage = 0.25f;
    [Range(0f, 1f)]
    public float darkenWhenPressedPercentage = 0.25f;
    private bool _isOn;
    public bool isOn {
        get {return _isOn;}
        set {
            if(value != _isOn)
            {
                SetToggle(value);
            }
            _isOn = value;
        }
    }
    public Image checkmark;
    public Color startcol;

    private HoldDownButton button;
    private bool isHovering;
    private bool isPressing; 
    private Vector2 startpos;
    private Tweener moveTweener;
    void Awake()
    {
        button = GetComponent<HoldDownButton>();
        button.OnPressEvent += Press;
        button.OnReleaseEvent += Release;
        button.OnEnterEvent += OnEnter;
        button.OnExitEvent += OnExit;

        if (top == null)
        {
            top = GetComponentInChildren<Image>();
        }
        if (bottom == null)
        {
            bottom   = GetComponent<Image>();
        }

        if (top != null)
        {
            startpos = top.rectTransform.anchoredPosition;
            startcol = top.color;
        }       
    }

    void Start()
    {
        if (autoSetShadow)
        {
            bottom.color = top.color.SetV(top.color.GetV() - darkenBackgroundPercentage);
        }
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            if (autoSetShadow && bottom !=null && top!=null)
            {
                bottom.color = top.color.SetV(top.color.GetV() - darkenBackgroundPercentage);
            }
        }
    }

    public void SetToggle(bool on)
    {
        _isOn = on;
        if (isToggle)
        {
            if (_isOn)
            {
                DoAnimateDown();
            }
            else
            {
                DoAnimateUp();
            }
        }
    }

    public void Press()
    {
        if(isToggle)
        {
            if (_isOn == true)
            {
                DoAnimateUp();
                _isOn = false;
            }
            else
            {
                DoAnimateDown();
                _isOn = true;
            }
        }
        else
        {
            DoAnimateDown();
        }

        isPressing = true;
    }

    public void DoAnimateDown()
    {
        if(isToggle && checkmark != null)
        {
            checkmark.enabled = true;
        }

        float heightDiff = bottom.rectTransform.sizeDelta.y * top.rectTransform.anchorMin.y;
        if (moveTweener != null) moveTweener.Kill();
        moveTweener = HOTween.To(top.rectTransform, 0.1f, new TweenParms()
            .Prop("anchoredPosition", startpos + Vector2.down * heightDiff * pressPercentage)

        );
        HOTween.To(top, 0.1f, new TweenParms()
            .Prop("color", startcol.SetV(startcol.GetV() - darkenWhenPressedPercentage))
        );
    }



    public void Release()
    {
        if(!isToggle)
        {
            if(isHovering)
                DoAnimateUp();
        }
        
        isPressing = false;
    }

    public void DoAnimateUp(bool execute=true)
    {
        if (isToggle && checkmark != null)
        {
            checkmark.enabled = false;
        }

        if (moveTweener != null) moveTweener.Kill();
        moveTweener = HOTween.To(top.rectTransform, 0.1f, new TweenParms()
            .Prop("anchoredPosition", startpos)
            .OnComplete(() => {
                if(execute)
                {
                    OnAnimateUpCompleted.Invoke(); 
                }
            })
        );
        HOTween.To(top, 0.1f, new TweenParms()
            .Prop("color", startcol)
        );
    }

    public void OnEnter()
    {
        isHovering = true;
        if(isPressing)
        {
            DoAnimateDown();
        }
    }

    public void OnExit()
    {
        isHovering = false;
        if(!isToggle && isPressing)
            DoAnimateUp(false);
    }

    
    public void OnPopupShow()
    {
        if(isPressing)
        {
            if (!isToggle)
            {
                Release();
            }
            else
            {
                SetToggle(!_isOn);
            }
        }
    }

    public void OnPopupHide()
    {
    }

    public void UpdateColor(Color color)
    {
        top.color = color;
        startcol = top.color;
        bottom.color = top.color.SetV(top.color.GetV() - 0.25f);
    }

}

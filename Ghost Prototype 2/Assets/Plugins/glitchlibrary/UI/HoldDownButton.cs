using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldDownButton : Button {
    private bool pressed;
    private bool wasPressed;
    public bool justPressed { get { return pressed && !wasPressed; } }
    public bool justReleased { get { return !pressed && wasPressed; } }

    private bool quePress;
    private bool queRelease;

    [InspectorNote("Events")]
    public bool _eventgroup;

    public UnityEvent OnPress;
    public Action OnPressEvent;

    public UnityEvent OnHold;
    public Action OnHoldEvent;

    public UnityEvent OnRelease;
    public Action OnReleaseEvent;

    public UnityEvent OnEnter;
    public Action OnEnterEvent;

    public UnityEvent OnExit;
    public Action OnExitEvent;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;
        if (eventData.pointerId <= 0)
            quePress = true;

        InvokeEvents(OnPress, OnPressEvent);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;
        if (eventData.pointerId <= 0)
            queRelease = true;

        InvokeEvents(OnRelease, OnReleaseEvent);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        InvokeEvents(OnEnter, OnEnterEvent);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        InvokeEvents(OnExit, OnExitEvent);
    }

    void Update()
    {
        if (pressed)
        {
            InvokeEvents(OnHold, OnHoldEvent);
        }
    }

    public void LateUpdate()
    {
        wasPressed = pressed;
        if (quePress)
        {
            pressed = true;
            quePress = false;
        }
        if (queRelease)
        {
            pressed = false;
            queRelease = false;
        }
    }

    void InvokeEvents(UnityEvent unityEvent, Action actionEvent)
    {
        if(unityEvent!=null) unityEvent.Invoke();
        if (actionEvent != null) actionEvent.Invoke();

    }

}

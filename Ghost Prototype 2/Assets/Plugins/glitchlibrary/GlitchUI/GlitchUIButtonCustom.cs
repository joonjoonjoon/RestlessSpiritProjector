using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
[ExecuteInEditMode]
public class GlitchUIButtonCustom : GlitchUIButton, IPointerDownHandler, IPointerUpHandler
{
    public Action OnButtonPressedEvent;
    public UnityEvent OnButtonPressed;
    public Action OnButtonReleasedEvent;
    public UnityEvent OnButtonReleased;

    public new void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (OnButtonPressed != null) OnButtonPressed.Invoke();
        if (OnButtonPressedEvent != null) OnButtonPressedEvent.Invoke();
        //DebugExtension.Blip("FUCK");
    }

    public new void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (OnButtonReleased != null) OnButtonReleased.Invoke();
        if (OnButtonReleasedEvent != null) OnButtonReleasedEvent.Invoke();
        //DebugExtension.Blip("FACK");
    }

}

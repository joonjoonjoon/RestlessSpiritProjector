using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class GlitchHoldDown : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public UnityEvent OnHoldUpdate;
    public Action OnHoldUpdateAction;

    private int tapCount;
    private float counter;

    public void Update()
    {
        if (tapCount > 0)
        {
            if (OnHoldUpdate != null) OnHoldUpdate.Invoke();
            if (OnHoldUpdateAction != null) OnHoldUpdateAction.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        tapCount--; 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        tapCount++;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPoint : MonoBehaviour
{
    public bool dragParent;
    public bool constrainX;
    public bool constrainY;

    public void OnMouseDrag()
    {
        DoDrag();
    }

    public void OnEventTriggerDrag()
    {
        DoDrag();
    }

    public void DoDrag()
    {
        var target = transform;
        if (dragParent) target = transform.parent;

        var newPos = target.position;

        var dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!constrainX) newPos.x = dragPos.x;
        if (!constrainY) newPos.y = dragPos.y;

        target.position = newPos;
    }

    
}
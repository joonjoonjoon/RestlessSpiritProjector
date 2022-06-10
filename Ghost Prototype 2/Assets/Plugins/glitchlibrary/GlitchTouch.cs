using UnityEngine;
using System.Collections.Generic;

public class GlitchTouch
{
    public const int MAXFINGERS = 11;

    private static Vector2[] positionHistory; //indexed by finger

    public float deltaTime;
    public int tapCount;
    public TouchPhase phase;
    public Vector2 deltaPosition;
    private Vector2 _deltaPositionCalculated;
    public int fingerId;
    public Vector2 position;
    public Vector2 rawPosition;

    public Vector2 deltaPositionCalculated
    {
        get
        {
            if (fingerId <= MAXFINGERS)
                return _deltaPositionCalculated;
            else
                return deltaPosition;
        }
    }

    public GlitchTouch()
    {

    }
    
    public GlitchTouch(Touch touch)
    {
        this.deltaPosition = touch.deltaPosition;
        this.deltaTime = touch.deltaTime;
        this.tapCount = touch.tapCount;
        this.phase = touch.phase;
        this.fingerId = touch.fingerId;
        this.position = touch.position;
        this.rawPosition = touch.rawPosition;

        if (positionHistory == null) positionHistory = new Vector2[MAXFINGERS];
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            this._deltaPositionCalculated = this.position - positionHistory[touch.fingerId];
            positionHistory[touch.fingerId] = touch.position;
        }
        else
        {
            positionHistory[touch.fingerId] = touch.position; ;
        }
    }

    public GlitchTouch(float deltaTime, int tapCount, TouchPhase phase, Vector2 deltaPosition, int fingerId, Vector2 position, Vector2 rawPosition)
    {
        this.deltaPosition = deltaPosition;
        this.deltaTime = deltaTime;
        this.tapCount = tapCount;
        this.phase = phase;
        this.fingerId = fingerId;
        this.position = position;
        this.rawPosition = rawPosition;

        if (positionHistory == null) positionHistory = new Vector2[MAXFINGERS];
        if (phase == TouchPhase.Moved || phase == TouchPhase.Stationary)
        {
            this._deltaPositionCalculated = this.position - positionHistory[fingerId];
            positionHistory[fingerId] = position;
        }
        else
        {
            positionHistory[fingerId] = position; ;
        }
    }

}
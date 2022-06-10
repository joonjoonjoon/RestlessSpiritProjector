using UnityEngine;
using System.Collections;
using System;

public class ScreenResizeEvent : MonoSingleton<ScreenResizeEvent>
{
    private static Action<Vector2> _resizeEvent;

    public static Action<Vector2> ResizeEvent
    {
        get
        {
            if (instance == null)
            {
                // should be initialized now, because it was touched
            }
            return _resizeEvent;
        }

        set
        {
            if (instance == null)
            {
                // should be initialized now, because it was touched
            }
            _resizeEvent = value;
            DoCheckAndUpdateDimensions();
        }

    }

    private static Vector2? dimensions;

    void Awake()
	{
		instance = this;
	}

    void Start()
    {
        DoCheckAndUpdateDimensions();
    }

    void Update()
    {
        DoCheckAndUpdateDimensions();
    }

    void OnLevelWasLoaded()
    {
        _resizeEvent.Invoke(dimensions.Value);
    }

    static void DoCheckAndUpdateDimensions()
    {
        var currentDimensions =new Vector2(Screen.width, Screen.height);

        if (dimensions == null)
        {
            dimensions = currentDimensions;
            if (_resizeEvent != null && dimensions != null)
                _resizeEvent.Invoke(dimensions.Value);
        }
        else
        {
            if (dimensions.Value != currentDimensions)
            {
                dimensions = currentDimensions;
                if (_resizeEvent != null && dimensions != null)
                {
                    _resizeEvent.Invoke(dimensions.Value);
                }
            }
        }
    }
}

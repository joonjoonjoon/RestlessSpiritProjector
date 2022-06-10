using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Mobile touches show up as Input.GetMouseButtonDown(0) but not vice versa. A mouse does not show up in Input.GetTouches.
// use InputHelper.GetTouches if you've implemented touch, and want the mouse to act like a touch...
[ScriptOrder(-1000)] //TODO: figure out better number...
[MonoSingleton(false, true)]
public class InputHelper : MonoSingleton<InputHelper>
{
    private static GlitchTouch lastFakeTouch;
    private static float lastFakeTouchTime;
    private static List<GlitchTouch> touches;
    private bool queueRelease;

    void Update()
    {
        touches = new List<GlitchTouch>();

        for (int i = 0; i < Input.touchCount; i++)
        {
            touches.Add(new GlitchTouch(Input.GetTouch(i)));
        }

#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL)
        if (queueRelease)
        {
            //Mouse tap received last frame, end touch this frame to simulate a tap.
            EndFakeTouch();
        }
        else if (Input.GetMouseButtonDown(0) && IsMouseWithinScreenBounds())
        {
            //Begin touch because mouse is within bounds
            BeginFakeTouch();

            //NOTE: (Down and Up in the same frame) == Mouse tap
            //Did we only recieve a tap? Schedule a release next frame!
            if (Input.GetMouseButtonUp(0))
            {
                queueRelease = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) || !IsMouseWithinScreenBounds())
        {
            //If the touch had previously been registered, and mouse up or mouse leaves screen bounds, end the fake touch.
            if (lastFakeTouch != null && lastFakeTouch.phase != TouchPhase.Ended)
            {
                EndFakeTouch();
            }
            else
            {
                lastFakeTouch = null;
            }
        }
        else if (Input.GetMouseButton(0) && IsMouseWithinScreenBounds())
        {
            //NOTE: TouchPhase.Cancelled is never used for FakeTouches, so can be ignored.
            //If the touch previously began, moved or stayed, update the fake touch.
            if (lastFakeTouch != null && lastFakeTouch.phase != TouchPhase.Ended)
            {
                if ((new Vector2(Input.mousePosition.x, Input.mousePosition.y) - lastFakeTouch.position).magnitude < 0.1f)
                {
                    StayFakeTouch();
                }
                else
                {
                    MoveFakeTouch();
                }    
            }
            else
            {
                //NOTE: For some reason GetMouseButton does not return true if OnMouseDown happens outside of screen.
                //Mouse was down outside screen bounds, meaning we should simulate a new touch, when it comes back!
                BeginFakeTouch();
            }
        }
        else
        {
            lastFakeTouch = null;
        }

        if (lastFakeTouch != null)
            touches.Add(lastFakeTouch);
#endif
    }

    #region FakeTouch helpers

    private void BeginFakeTouch()
    {
        if (lastFakeTouch == null)
            lastFakeTouch = new GlitchTouch();

        lastFakeTouch.phase = TouchPhase.Began;
        lastFakeTouch.deltaPosition = Vector2.zero;
        lastFakeTouch.deltaTime = Time.unscaledDeltaTime;
        lastFakeTouch.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        lastFakeTouch.fingerId = GlitchTouch.MAXFINGERS +1 ;
    }

    private void MoveFakeTouch()
    {
        if (lastFakeTouch == null)
            lastFakeTouch = new GlitchTouch();

        lastFakeTouch.phase = TouchPhase.Moved;
        Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        lastFakeTouch.deltaPosition = newPosition - lastFakeTouch.position;
        lastFakeTouch.deltaTime = Time.unscaledDeltaTime;
        lastFakeTouch.position = newPosition;
        lastFakeTouch.fingerId = GlitchTouch.MAXFINGERS + 1;
    }

    private void StayFakeTouch()
    {
        if (lastFakeTouch == null)
            lastFakeTouch = new GlitchTouch();

        lastFakeTouch.phase = TouchPhase.Stationary;
        Vector2 newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        lastFakeTouch.deltaPosition = Vector2.zero;
        lastFakeTouch.deltaTime = Time.unscaledDeltaTime;
        lastFakeTouch.position = newPosition;
        lastFakeTouch.fingerId = GlitchTouch.MAXFINGERS + 1;
    }

    private void EndFakeTouch()
    {
        if (lastFakeTouch == null)
            lastFakeTouch = new GlitchTouch();

        lastFakeTouch.phase = TouchPhase.Ended;
        //lastFakeTouch.deltaPosition = lastFakeTouch.deltaPosition;
        //lastFakeTouch.deltaTime = lastFakeTouch.deltaTime;
        //lastFakeTouch.position = lastFakeTouch.position;
        lastFakeTouch.fingerId = GlitchTouch.MAXFINGERS + 1;
        queueRelease = false;
    }

    private bool IsMouseWithinScreenBounds()
    {
        if (0 < Input.mousePosition.x && Input.mousePosition.x < Screen.width &&
            0 < Input.mousePosition.y && Input.mousePosition.y < Screen.height)
        {
            return true;
        }

        return false;
    }

    #endregion

    public static List<GlitchTouch> GetTouches()
    {
        if (instance == null)
            instance.Update(); // it's possible that only this static method is touched, and thus the singleton is never initialized. 

        if (touches == null) touches = new List<GlitchTouch>(); // for some reason it returns null on the first run through...

        return touches;      
    }

}

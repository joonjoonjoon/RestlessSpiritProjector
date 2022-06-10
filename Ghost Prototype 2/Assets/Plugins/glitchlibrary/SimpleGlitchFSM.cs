using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Glitch FSM
/// Use as a Regular FSM, by always calling SetState (will automatically run Start and End actions).
/// Use as a Stack FSM, by calling Push and Pop State (States need to be pop'ed manually, within the Start/End and Update functions)
/// </summary>
public class SimpleGlitchFSM
{
    private Stack<Action> stack = new Stack<Action>();

    /// <summary>
    /// Gets the current state.
    /// </summary>
    /// <returns>The current state.</returns>
    public Action GetCurrentState()
    {
        if (stack.Count == 0) return null;

        var state = stack.Peek();

        if (state == null) return null;

        return state;
    }

    /// <summary>
    /// Uses Reflection to get the name of the current state.
    /// </summary>
    /// <returns>The current state name.</returns>
    public string GetCurrentStateName()
    {
        if (stack.Count == 0) return "(non)";

        var state = stack.Peek();

        if (state == null) return "(null)";

        return state.Method.Name;
    }

    /// <summary>
    /// Updates the current state.
    /// </summary>
    public void RunStateUpdate()
    {       
        //No current states check
        if (stack.Count == 0) return;

        //Get the current state
        var state = stack.Peek();

        if (state != null)
        {
            //Execute the current state
            state();
        }
    }

    /// <summary>
    /// Sets the state. Clears the stack, but runs end action of current state.
    /// </summary>
    /// <param name="State">State action to be run.</param>
    /// <param name="Start">Start action to init state.</param>
    /// <param name="End">End action to clean up state.</param>
    public void SetState(Action State, Action Setup = null, Action Cleanup = null)
    {
        if (State == null)
        {
            Debug.LogError("State was null!");
            return;
        }

        //Remove current state
        if (stack.Count > 0) stack.Pop();

        //Check if currentState had an end action
        if (stack.Count > 0)
        {
            var end = stack.Pop();
            if (end != null)
            {
                end();
            }
        }

        //Remove any old stuff
        stack.Clear();

        //Push end action
        if (Cleanup != null) stack.Push(Cleanup);

        //Push state
        stack.Push(State);

        //Run start action
        if (Setup != null) Setup();
    }

    /// <summary>
    /// Pushs a new State and its Start/End onto the stack.
    /// Can be used for temporary states, that pop themselves when done!
    /// </summary>
    /// <param name="State">State action to be run. NOTE: Can contain Push/Pop state logic!</param>
    /// <param name="Start">Start action to init state. NOTE: Should contain a PopState call!</param>
    /// <param name="End">End action to clean up state. NOTE: Should contain a PopState call!</param>
    public void PushState(Action State, Action Setup = null, Action Cleanup = null)
    {
        if (State == null)
        {
            Debug.LogError("State was null!");
            return;
        }

        //Push end action
        if (Cleanup != null) stack.Push(Cleanup);

        //Push state
        stack.Push(State);

        //Push start action
        if (Setup != null) stack.Push(Setup);
    }

    /// <summary>
    /// Pops the top state.
    /// </summary>
    public void PopState()
    {
        //Remove current state
        if (stack.Count > 0) stack.Pop();
    }
}
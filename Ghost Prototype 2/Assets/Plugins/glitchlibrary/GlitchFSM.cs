using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Scripting;

/// <summary>
/// Glitch FSM attribute. Is used to reflection style bind the method, to a state and function in the FSM.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class GlitchFSMAttribute : PreserveAttribute
{
    public object enumValue;
    public GlitchFSMStateAction action;

    public GlitchFSMAttribute(object enumValue, GlitchFSMStateAction action)
    {
        this.enumValue = enumValue;
        this.action = action;
    }
}

/// <summary>
/// Glitch FSM state->action, defines which actions the FSM can handle for each state.
/// </summary>
public enum GlitchFSMStateAction
{
    Enter,
    Update,
    Exit,
}

public class GlitchFSM<TEnum> where TEnum : struct, IConvertible, IComparable
{
    private Stack<Action> stack;
    private Dictionary<KeyValuePair<TEnum, GlitchFSMStateAction>, Action> stateDictionary;
    private TEnum currentState;
    private TEnum lastState;

    #region Initialization

    public GlitchFSM(MonoBehaviour refToImplementation)
    {
        //Init
        stack = new Stack<Action>();
        stateDictionary = new Dictionary<KeyValuePair<TEnum, GlitchFSMStateAction>, Action>();
        currentState = default(TEnum);
        lastState = default(TEnum);

        //Reflection
        var type = refToImplementation.GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.GetCustomAttributes(typeof(GlitchFSMAttribute), false).Any());
        foreach (var method in methods)
        {
            VerifyMemberType(method);
            var action = MakeAction(method, refToImplementation);
            var attr = method.GetCustomAttributes(typeof(GlitchFSMAttribute), false)[0] as GlitchFSMAttribute;

            var kvp = new KeyValuePair<TEnum, GlitchFSMStateAction>((TEnum)attr.enumValue, attr.action);

            if (stateDictionary.ContainsKey(kvp))
            {
                throw new Exception("You've defined the same state/function pair twice: " + kvp.Key + "/" + kvp.Value);
            }
            else
            {
                stateDictionary.Add(kvp, action);    
            }
        }
            
        //Default state setup
        SetState(currentState);
    }

    private void VerifyMemberType(MethodInfo method)
    {
        if (method.GetParameters().Length > 0)
        {
            throw new Exception("Methods decorated with [GlitchFSMAttribute] must not have any parameters");
        }
    }

    /// <summary>
    /// Creates an action object that, when called, sets `Trigger` to itself and then moves to the next state.
    /// </summary>
    private Action MakeAction(MethodInfo method, object obj)
    {
        Action foo = null;
        foo = () =>
        {
            method.Invoke(obj, null);
        };
        return foo;
    }

    #endregion

    /// <summary>
    /// Gets the current state enum value.
    /// </summary>
    /// <returns>The current state enum.</returns>
    public TEnum GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Gets the current state enum value.
    /// </summary>
    /// <returns>The current state enum.</returns>
    public TEnum GetLastState()
    {
        return lastState;
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

    public TEnum SetState(TEnum state)
    {
        Action Setup = null, Update = null, Cleanup = null;

        var updateKey = new KeyValuePair<TEnum, GlitchFSMStateAction>(state, GlitchFSMStateAction.Update);
        if (stateDictionary.ContainsKey(updateKey))
        {
            Update = stateDictionary[updateKey];
        }

        if (Update == null)
        {
            //Debug.Log("No Update method for state: " + state);
            //NOTE: No update defined.
            Update = DoNothing;
        }

        var setupKey = new KeyValuePair<TEnum, GlitchFSMStateAction>(state, GlitchFSMStateAction.Enter);
        if (stateDictionary.ContainsKey(setupKey))
        {
            Setup = stateDictionary[setupKey];
        }

        var cleanupKey = new KeyValuePair<TEnum, GlitchFSMStateAction>(state, GlitchFSMStateAction.Exit);
        if (stateDictionary.ContainsKey(cleanupKey))
        {
            Cleanup = stateDictionary[cleanupKey];
        }

        lastState = currentState;
        currentState = state;

        SetState(Update, Setup, Cleanup);

        return currentState;
    }

    /// <summary>
    /// Sets the state. Clears the stack, but runs end action of current state.
    /// </summary>
    /// <param name="State">State action to be run.</param>
    /// <param name="Start">Start action to init state.</param>
    /// <param name="End">End action to clean up state.</param>
    private void SetState(Action State, Action Setup = null, Action Cleanup = null)
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

    public TEnum PushState(TEnum state)
    {
        Action Setup = null, Update = null, Cleanup = null;

        var updateKey = new KeyValuePair<TEnum, GlitchFSMStateAction>(state, GlitchFSMStateAction.Update);
        if (stateDictionary.ContainsKey(updateKey))
        {
            Update = stateDictionary[updateKey];
        }

        if (Update == null)
        {
            //Debug.Log("No Update method for state: " + state);
            //NOTE: No update defined.
            Update = DoNothing;
        }

        var setupKey = new KeyValuePair<TEnum, GlitchFSMStateAction>(state, GlitchFSMStateAction.Enter);
        if (stateDictionary.ContainsKey(setupKey))
        {
            Setup = stateDictionary[setupKey];
        }

        var cleanupKey = new KeyValuePair<TEnum, GlitchFSMStateAction>(state, GlitchFSMStateAction.Exit);
        if (stateDictionary.ContainsKey(cleanupKey))
        {
            Cleanup = stateDictionary[cleanupKey];
        }

        lastState = currentState;
        currentState = state;

        PushState(Update, Setup, Cleanup);

        return currentState;
    }

    /// <summary>
    /// Pushs a new State and its Start/End onto the stack.
    /// Can be used for temporary states, that pop themselves when done!
    /// </summary>
    /// <param name="State">State action to be run. NOTE: Can contain Push/Pop state logic!</param>
    /// <param name="Start">Start action to init state. NOTE: Should contain a PopState call!</param>
    /// <param name="End">End action to clean up state. NOTE: Should contain a PopState call!</param>
    private void PushState(Action State, Action Setup = null, Action Cleanup = null)
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
    public TEnum PopState()
    {
        //Remove current state
        if (stack.Count > 0) stack.Pop();

        //Check new state
        if (stack.Count > 0)
        {
            var state = stack.Peek();
            foreach (var item in stateDictionary)
            {
                if (item.Value == state)
                {
                    //Set current state to the TEnum value of the item
                    lastState = currentState;
                    currentState = item.Key.Key;
                    break;
                }
            }
        }
        else
        {
            //Set Default fallback state
            Debug.Log("FSM rollback to default state, was this intentional?");
            lastState = currentState;
            currentState = default(TEnum);
            SetState(currentState);
        }

        return currentState;
    }

    private void DoNothing()
    {
        //Used when a state has no Update defined.
    }

}

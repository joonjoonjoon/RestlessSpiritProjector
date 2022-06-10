using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[ScriptOrder(-100)]
public class DoOnStart : MonoBehaviour {
	public UnityEvent onAwake;
	public UnityEvent onStart;

	void Awake()
	{
	    onAwake.Invoke();
	}
	void Start () {
	    onStart.Invoke();
	}

    void Reset()
    {
#if UNITY_EDITOR
        onAwake = new UnityEvent();
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(onAwake, new UnityAction<bool>(gameObject.SetActive), false);
#endif
    }
}

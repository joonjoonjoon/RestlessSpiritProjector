using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class DebugValues
{
	private static DebugValues _instance;
	public static DebugValues Instance 
	{
		get 
		{
			if(_instance == null)
			{
				_instance = new DebugValues();
				_instance.values = new Dictionary<string, Func<object>>();
			}
			
			return _instance;
		}
	}

	private Dictionary<string, Func<object>> values;

	/// <summary>
	/// Gets the values. DON'T USE THIS FOR ADDING VALUES !
	/// </summary>
	/// <returns>The values.</returns>
	public Dictionary<string, Func<object>> GetValues()
	{
		return values;
	}

	
	public void AddLiveValue(string label, Func<object> value)
	{
		values[label] = value;
	}

	public void AddStaticValue(string label, Func<object> value)
	{
		var result = value.Invoke();

		values[label] = () => { return result; };
	}

	public void AddStaticValue(string label, object value)
	{
		values[label] = () => { return value; };
	}
}

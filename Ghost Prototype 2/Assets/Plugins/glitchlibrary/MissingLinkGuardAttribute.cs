using UnityEngine;
using System.Collections;
using System;

[AttributeUsage(AttributeTargets.Class)]
public class MissingLinkGuardAttribute : Attribute {

	public MissingLinkGuardAttribute ()
	{
		//TODO: save values like autoFix
		//TODO: find all objects tagget with this an save stuff for them in the editor script...
	}
}

using UnityEngine;
using System.Collections;

public static class CoroutineUtil
{
	//NOTE: USED BY TIMER FOR MENU DELAY....
	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
	
//	public static IEnumerator WaitForUnscaledSeconds(float time)
//	{
//		float start = Time.unscaledTime;
//		while (Time.unscaledTime < start + time)
//		{
//			yield return null;
//		}
//	}
}
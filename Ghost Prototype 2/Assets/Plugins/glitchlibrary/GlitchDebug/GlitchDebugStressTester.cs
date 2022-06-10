using UnityEngine;
using System.Collections;

public class GlitchDebugStressTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (DoRandomStuff());
	}
	
	IEnumerator DoRandomStuff() {
		while(true)
		{
			var r = Random.Range(0,10);
			string level = "zero";
			switch(r)
			{
			case 0:
				level =GlitchDebug.WARNING;
				break;
			case 1:
				level =GlitchDebug.INFORMATION;
				break;
			case 2:
				level =GlitchDebug.IMPORTANT;
				break;
			case 3:
				level =GlitchDebug.ERROR;
				break;
			}

			GlitchDebug.Log("[" + level + "] Oh no! Something happened! "  , level);

			yield return new WaitForSeconds(Random.Range(0.1f,0.5f));
		}
	}
}

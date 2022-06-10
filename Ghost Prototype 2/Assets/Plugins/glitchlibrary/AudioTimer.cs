using UnityEngine;

public class AudioTimer : MonoBehaviour {

	public AudioClip[] audio_;
	public float[] offset;

	public float GetOffset (AudioSource a){
		return GetOffset (a.clip);
	}

	public float GetOffset (AudioClip a){
		for (int i = 0; i < audio_.Length; i++) {
			if (audio_ [i] == a) {
				return offset [i];
			}
		}
		return 0;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSount : MonoBehaviour {

    private AudioSource source;
    public float lerpspeed=3;
    public float freq = 0.1f;
    public float offset=0.2f;
    public bool mute;
    public bool off;

    void Start () {
        source = GetComponent<AudioSource>();
        source.Play();
        offset = Random.Range(0, 100);
        freq = Random.Range(0.1f, 0.2f);
        source.volume = 0;
    }
	
	void Update () {
        var dest = Mathf.Sin((Time.time + offset) * freq) / 2f + 0.5f; ;
        if (mute) dest = 0.05f;
        if (off) dest = 0f;

        source.volume = Mathf.Lerp(source.volume, dest, lerpspeed * Time.deltaTime);
	}
}

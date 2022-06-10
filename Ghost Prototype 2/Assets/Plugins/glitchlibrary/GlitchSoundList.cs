using UnityEngine;
using System.Collections;

[System.Serializable]
public class GlitchSoundList
{
    public AudioClip[] clips;
    [Range(0, 2)]
    public float pitchMin = 1;
    [Range(0, 2)]
    public float pitchMax = 1;
    [Range(0, 2)]
    public float volumeMin = 1;
    [Range(0, 2)]
    public float volumeMax = 1;

    public float barLength;

    public float volume
    {
        get { return Random.Range(volumeMin, volumeMax); }
    }

    public float pitch
    {
        get { return Random.Range(pitchMin, pitchMax); }
    }

    public AudioClip randomClip
    {
        get { return clips[Random.Range(0, clips.Length)]; }
    }
}
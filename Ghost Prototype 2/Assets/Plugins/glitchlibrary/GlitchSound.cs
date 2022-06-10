using UnityEngine;
using System.Collections;

public enum GlitchSoundNotes
{
    C,
    Cs,
    D,
    Ds,
    E,
    F,
    Fs,
    G,
    Gs,
    A,
    As,
    B

}

[System.Serializable]
public class GlitchSound
{
    public AudioClip clip;
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
}



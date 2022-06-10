using UnityEngine;
using System.Collections;


[System.Serializable]
public class GlitchSoundTonal
{
    public AudioClip clip;
    public GlitchSoundScaleOfNotes scale;
    
    [Range(0, 2)]
    public float volumeMin = 1;
    [Range(0, 2)]
    public float volumeMax = 1;

    public float volume
    {
        get { return Random.Range(volumeMin, volumeMax); }
    }
}